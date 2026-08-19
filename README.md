# Success Lessons Platform

An interactive reading platform that transforms a PDF anthology of biographies —
hard work, passion, failures, and lessons — into a searchable, bookmarkable
reading experience with progress tracking.

- **Frontend**: `frontend/` — Next.js 16, TypeScript, Tailwind CSS, shadcn/ui
- **Backend**: `backend/SuccessLessons.Api/` — .NET 8 Web API
- **Data pipeline**: `data-pipeline/` — Python scripts to turn the source PDF into Firestore-ready JSON
- **Firebase config**: `firebase/` — Firestore security rules and indexes

See [`docs/PLANNING.md`](docs/PLANNING.md) for the full architecture, schema, API routes, and development phases.

## Quick start

### Backend

```bash
cd backend
dotnet restore
dotnet run --project SuccessLessons.Api
```

Copy your Firebase service account key locally and set `Firebase:ProjectId` /
`Firebase:CredentialsPath` in `backend/SuccessLessons.Api/appsettings.Development.json`
(or via environment variables) before running — the API needs Admin SDK
credentials to talk to Firestore and verify Firebase Auth tokens.

### Frontend

```bash
cd frontend
cp .env.local.example .env.local   # fill in your Firebase web app config
npm install
npm run dev
```

### Data pipeline

```bash
cd data-pipeline
python -m venv venv
venv\Scripts\activate
pip install -r requirements.txt
python extract_pdf.py path/to/source.pdf
python structure_content.py        # edit output/structured_content.json by hand
python seed_firestore.py path/to/service-account.json
```
