# Success Lessons Platform — Project Plan

An interactive reading platform that transforms a PDF about successful people — their hard work, passion, failures, and lessons — into a searchable, bookmarkable, progress-tracked web experience.

---

## 1. Architecture

```
┌─────────────────────┐        HTTPS/JSON        ┌──────────────────────┐
│   Next.js Frontend   │ ───────────────────────▶ │   .NET Web API        │
│  (Vercel)            │ ◀─────────────────────── │   (Render/Azure)      │
│  TS + Tailwind +     │                            │  Controllers /       │
│  shadcn/ui           │      Firebase ID token     │  Services / DTOs     │
└──────────┬───────────┘      (Bearer auth)          └───────────┬─────────┘
           │                                                     │
           │ Firebase Auth SDK (client)                          │ Firebase Admin SDK
           ▼                                                     ▼
   ┌───────────────────┐                              ┌────────────────────┐
   │  Firebase Auth      │◀───────── verifies token ──│  Firestore (via     │
   │  (email/pwd, Google) │                             │  Admin SDK)         │
   └───────────────────┘                              └────────────────────┘
```

**Flow**
1. PDF content is parsed once (offline script) into structured JSON — chapters, people profiles, lessons, quotes — and seeded into Firestore.
2. Next.js renders the reading UI (chapters, profiles, search, bookmarks, progress) using data fetched from the .NET API.
3. Firebase Auth (client SDK) handles signup/login; the ID token is sent as a Bearer token to the .NET API.
4. .NET API verifies the token server-side with the Firebase Admin SDK, then reads/writes Firestore via the Admin SDK (server owns all writes — Firestore security rules lock down direct client writes).
5. User-specific data (bookmarks, highlights, reading progress, notes) is written through the API, not directly from the client, so business rules stay centralized.

**Why this split:** Firestore's native SDKs are great for direct client reads, but routing everything through a .NET API gives you a resume-relevant "real backend" (validation, DTOs, business logic, potential future SQL migration) while Firebase covers auth + a serverless-friendly document store.

---

## 2. Folder Structure

```
success-lessons-platform/
├── frontend/                          # Next.js app
│   ├── app/
│   │   ├── (marketing)/
│   │   │   └── page.tsx               # Landing page
│   │   ├── (auth)/
│   │   │   ├── login/page.tsx
│   │   │   └── register/page.tsx
│   │   ├── library/
│   │   │   ├── page.tsx               # Browse all people/chapters
│   │   │   └── [personSlug]/page.tsx  # Individual profile/story
│   │   ├── chapters/
│   │   │   └── [chapterSlug]/page.tsx # Chapter reading view
│   │   ├── lessons/
│   │   │   └── page.tsx               # All extracted lessons/quotes
│   │   ├── bookmarks/page.tsx
│   │   ├── profile/page.tsx           # Account + reading stats
│   │   ├── search/page.tsx
│   │   ├── layout.tsx
│   │   └── globals.css
│   ├── components/
│   │   ├── ui/                        # shadcn/ui generated components
│   │   ├── reader/                    # ReaderView, ProgressBar, Highlighter
│   │   ├── cards/                     # PersonCard, LessonCard, QuoteCard
│   │   └── layout/                    # Navbar, Sidebar, Footer
│   ├── lib/
│   │   ├── firebase/
│   │   │   ├── client.ts              # Firebase client init
│   │   │   └── auth.ts                # Auth helper hooks
│   │   ├── api/
│   │   │   ├── client.ts              # fetch wrapper w/ auth header
│   │   │   ├── people.ts
│   │   │   ├── chapters.ts
│   │   │   ├── lessons.ts
│   │   │   └── bookmarks.ts
│   │   └── utils.ts
│   ├── hooks/
│   │   ├── useAuth.ts
│   │   ├── useBookmarks.ts
│   │   └── useReadingProgress.ts
│   ├── types/
│   │   └── index.ts                   # Shared TS types/interfaces
│   ├── public/
│   ├── .env.local
│   ├── next.config.ts
│   ├── tailwind.config.ts
│   └── package.json
│
├── backend/                           # .NET Web API
│   ├── SuccessLessons.Api/
│   │   ├── Controllers/
│   │   │   ├── PeopleController.cs
│   │   │   ├── ChaptersController.cs
│   │   │   ├── LessonsController.cs
│   │   │   ├── BookmarksController.cs
│   │   │   ├── ProgressController.cs
│   │   │   └── SearchController.cs
│   │   ├── Services/
│   │   │   ├── IFirestoreService.cs
│   │   │   ├── FirestoreService.cs
│   │   │   ├── PeopleService.cs
│   │   │   ├── BookmarkService.cs
│   │   │   └── ProgressService.cs
│   │   ├── Models/                    # Firestore document models
│   │   │   ├── Person.cs
│   │   │   ├── Chapter.cs
│   │   │   ├── Lesson.cs
│   │   │   ├── Bookmark.cs
│   │   │   └── ReadingProgress.cs
│   │   ├── DTOs/
│   │   │   ├── PersonDto.cs
│   │   │   ├── ChapterDto.cs
│   │   │   └── BookmarkRequestDto.cs
│   │   ├── Middleware/
│   │   │   └── FirebaseAuthMiddleware.cs
│   │   ├── Config/
│   │   │   └── FirebaseOptions.cs
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── SuccessLessons.Api.csproj
│   ├── SuccessLessons.Tests/
│   │   └── (unit tests per service/controller)
│   └── SuccessLessons.sln
│
├── data-pipeline/                     # One-time / repeatable PDF → JSON → Firestore
│   ├── extract_pdf.py                 # PDF text extraction (pdfplumber/PyMuPDF)
│   ├── structure_content.py           # Split into people/chapters/lessons
│   ├── seed_firestore.py              # Upload structured JSON to Firestore
│   └── output/
│       └── structured_content.json
│
├── firebase/
│   ├── firestore.rules
│   └── firestore.indexes.json
│
├── docs/
│   └── PLANNING.md                    # this file
│
└── README.md
```

---

## 3. Database Collections (Firestore)

```
people/{personId}
  - name: string
  - slug: string
  - era: string                 // e.g. "20th century"
  - field: string                // e.g. "Business", "Science", "Sports"
  - summary: string
  - imageUrl: string
  - failureCount: number
  - tags: string[]
  - createdAt: timestamp

chapters/{chapterId}
  - personId: string (ref)
  - title: string
  - slug: string
  - order: number
  - contentBlocks: [{ type: "paragraph"|"quote"|"image"|"stat", content: string }]
  - estimatedReadMinutes: number
  - createdAt: timestamp

lessons/{lessonId}
  - personId: string (ref)
  - chapterId: string (ref)
  - text: string
  - category: string             // "failure", "passion", "hard-work", "mindset"
  - isFeatured: boolean

quotes/{quoteId}
  - personId: string (ref)
  - text: string
  - context: string

users/{uid}                      // mirrors Firebase Auth user, extra profile data
  - displayName: string
  - email: string
  - joinedAt: timestamp
  - readingStreak: number

bookmarks/{bookmarkId}
  - uid: string (ref)
  - chapterId: string (ref)
  - createdAt: timestamp

readingProgress/{uid}_{chapterId}
  - uid: string (ref)
  - chapterId: string (ref)
  - percentComplete: number
  - lastPositionBlockIndex: number
  - completed: boolean
  - updatedAt: timestamp

highlights/{highlightId}
  - uid: string (ref)
  - chapterId: string (ref)
  - blockIndex: number
  - selectedText: string
  - note: string
  - createdAt: timestamp
```

**Indexes needed:** `lessons` by `category` + `isFeatured`; `chapters` by `personId` + `order`; `readingProgress` by `uid` + `updatedAt`.

**Firestore security rules:** deny all direct client writes except `users/{uid}` self-profile fields; everything else (bookmarks, progress, highlights) written only via the .NET API using the Admin SDK service account, which bypasses rules by design — rules exist as the client-side safety net.

---

## 4. API Routes (.NET Web API)

```
Auth
  (No custom auth endpoints — Firebase Auth handles signup/login client-side.
   All routes below require `Authorization: Bearer <Firebase ID token>` except where marked public.)

GET    /api/people                     [public]  List all people (filter by field/tag)
GET    /api/people/{slug}              [public]  Person detail + their chapters
GET    /api/chapters/{slug}            [public]  Chapter content
GET    /api/lessons                    [public]  List lessons (filter by category)
GET    /api/lessons/featured           [public]  Featured/curated lessons
GET    /api/quotes/random              [public]  Random quote (for landing page)
GET    /api/search?q=                  [public]  Full-text-ish search across people/chapters/lessons

GET    /api/bookmarks                  [auth]    Current user's bookmarks
POST   /api/bookmarks                  [auth]    Add bookmark { chapterId }
DELETE /api/bookmarks/{id}             [auth]    Remove bookmark

GET    /api/progress                   [auth]    All progress for current user
GET    /api/progress/{chapterId}       [auth]    Progress for one chapter
PUT    /api/progress/{chapterId}       [auth]    Upsert progress { percentComplete, lastPositionBlockIndex }

GET    /api/highlights/{chapterId}     [auth]    Highlights/notes for a chapter
POST   /api/highlights                 [auth]    Create highlight
DELETE /api/highlights/{id}            [auth]    Remove highlight

GET    /api/users/me                   [auth]    Current user profile + stats
PATCH  /api/users/me                   [auth]    Update display name etc.

GET    /api/admin/people               [auth+admin]  Admin CRUD for seeding/editing content
POST   /api/admin/people
PUT    /api/admin/people/{id}
DELETE /api/admin/people/{id}
(mirror for chapters/lessons — optional, only if you want an in-app CMS
 instead of the data-pipeline scripts)
```

---

## 5. Frontend Pages

| Route | Purpose |
|---|---|
| `/` | Landing page — hero, featured people, random quote, CTA to sign up |
| `/login`, `/register` | Firebase Auth flows |
| `/library` | Grid/list of all people, filterable by field/era/tag |
| `/library/[personSlug]` | Person profile — bio, failure highlights, list of chapters |
| `/chapters/[chapterSlug]` | Immersive reader — content blocks, progress bar, highlight-to-note, bookmark button |
| `/lessons` | Browse all extracted lessons, filter by category (failure, passion, hard work, mindset) |
| `/search` | Search results across people/chapters/lessons |
| `/bookmarks` | Signed-in user's saved chapters |
| `/profile` | Account settings, reading streak, completed chapters, stats dashboard |

**Key UI components:** `ReaderView` (paginated/scroll content renderer with block types), `ProgressRing`, `HighlightPopover` (select text → save note), `PersonCard`, `LessonCard`, `QuoteCarousel`, `CategoryFilterBar`.

---

## 6. Development Phases

**Phase 0 — Setup (½–1 day)**
- Init Next.js (TS, Tailwind, shadcn/ui), .NET Web API project, Firebase project (Auth + Firestore), Git repo, Vercel + Render/Azure projects.

**Phase 1 — Content Pipeline (1–2 days)**
- Extract PDF text (pdfplumber/PyMuPDF), manually/semi-automatically split into people/chapters/lessons/quotes JSON, write `seed_firestore.py` to upload.

**Phase 2 — Backend Core (2–3 days)**
- Firestore service wrapper, models/DTOs, public read endpoints (people, chapters, lessons, search), Firebase Admin token verification middleware.

**Phase 3 — Frontend Reading Experience (3–4 days)**
- Landing, library, person profile, chapter reader pages wired to the API; responsive Tailwind/shadcn layout.

**Phase 4 — Auth + Personalization (2–3 days)**
- Firebase Auth client integration, protected routes, bookmarks, reading progress, highlights — both API endpoints and frontend hooks/UI.

**Phase 5 — Search & Polish (1–2 days)**
- Search endpoint + page, empty/loading/error states, animations, SEO metadata, accessibility pass.

**Phase 6 — Deploy & Harden (1 day)**
- Firestore security rules, environment secrets, deploy frontend to Vercel, backend to Render/Azure, CORS config, smoke test end-to-end.

**Phase 7 — Stretch goals (optional)**
- Reading streak gamification, admin CMS UI for content editing, dark mode, text-to-speech for chapters, PDF export of personal notes.

---

## 7. Setup Commands

```bash
# --- Frontend ---
npx create-next-app@latest frontend --typescript --tailwind --eslint --app
cd frontend
npx shadcn@latest init
npx shadcn@latest add button card input dialog avatar badge progress tabs
npm install firebase
npm install -D @types/node

# --- Backend ---
dotnet new webapi -n SuccessLessons.Api -o backend/SuccessLessons.Api
cd backend/SuccessLessons.Api
dotnet add package FirebaseAdmin
dotnet add package Google.Cloud.Firestore
dotnet add package Swashbuckle.AspNetCore
dotnet new xunit -n SuccessLessons.Tests -o ../SuccessLessons.Tests
cd .. && dotnet new sln -n SuccessLessons
dotnet sln add SuccessLessons.Api/SuccessLessons.Api.csproj SuccessLessons.Tests/SuccessLessons.Tests.csproj

# --- Data pipeline (Python) ---
cd data-pipeline
python -m venv venv
venv\Scripts\activate          # Windows
pip install pymupdf pdfplumber firebase-admin

# --- Firebase ---
npm install -g firebase-tools
firebase login
firebase init firestore        # creates firestore.rules / firestore.indexes.json

# --- Run locally ---
# frontend
npm run dev                    # http://localhost:3000
# backend
dotnet run --project backend/SuccessLessons.Api   # https://localhost:5001

# --- Deploy ---
vercel --prod                  # frontend (run inside frontend/)
# backend: connect Render/Azure to the repo, set build cmd `dotnet publish`, 
# start cmd `dotnet SuccessLessons.Api.dll`, add env vars for Firebase service account
```

---

## 8. CV-Ready Project Description

> **Success Lessons Platform** — Full-stack web app that transforms a PDF anthology of biographies into an interactive reading platform. Built a Next.js/TypeScript/Tailwind/shadcn frontend consuming a custom .NET Web API backend, with Firebase Authentication and Firestore as the persistence layer. Designed a document-based schema for people, chapters, lessons, and quotes; built a Python data pipeline to parse and structure raw PDF text into Firestore-ready JSON. Implemented server-verified Firebase token authentication in .NET middleware, RESTful endpoints for content delivery and user-specific state (bookmarks, reading progress, highlights), and a responsive reading UI with progress tracking and full-text search. Deployed frontend on Vercel and backend on Render/Azure with CI-ready environment configuration.

**One-liner (resume bullet):**
> Built and deployed a full-stack reading platform (Next.js/TypeScript, .NET Web API, Firebase Auth/Firestore) that parses PDF content into a structured, searchable library with per-user bookmarks and reading-progress tracking.

**Skills demonstrated:** REST API design, Firebase Admin SDK token verification, NoSQL schema design, Next.js App Router, component-driven UI with shadcn/ui, data pipeline scripting (Python + PDF parsing), cloud deployment (Vercel/Render/Azure), auth flows, and cross-service integration.
