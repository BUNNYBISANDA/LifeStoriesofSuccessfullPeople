"""Upload output/structured_content.json to Firestore.

Requires a service account key JSON (Firebase Console > Project Settings >
Service Accounts > Generate new private key). Never commit this file.

Usage:
    python seed_firestore.py path/to/service-account.json
"""

import json
import sys
from pathlib import Path

import firebase_admin
from firebase_admin import credentials, firestore

STRUCTURED_PATH = Path(__file__).parent / "output" / "structured_content.json"


def main():
    if len(sys.argv) != 2:
        print("Usage: python seed_firestore.py <path-to-service-account.json>")
        sys.exit(1)

    if not STRUCTURED_PATH.exists():
        raise SystemExit(f"Missing {STRUCTURED_PATH} — run structure_content.py first.")

    cred = credentials.Certificate(sys.argv[1])
    firebase_admin.initialize_app(cred)
    db = firestore.client()

    data = json.loads(STRUCTURED_PATH.read_text(encoding="utf-8"))

    person_slug_to_id = {}
    for person in data["people"]:
        slug = person.pop("slug")
        person["slug"] = slug
        person["createdAt"] = firestore.SERVER_TIMESTAMP
        doc_ref = db.collection("people").document()
        doc_ref.set(person)
        person_slug_to_id[slug] = doc_ref.id
    print(f"Seeded {len(person_slug_to_id)} people")

    chapter_slug_to_id = {}
    for chapter in data["chapters"]:
        person_slug = chapter.pop("personSlug")
        chapter["personId"] = person_slug_to_id[person_slug]
        chapter["createdAt"] = firestore.SERVER_TIMESTAMP
        doc_ref = db.collection("chapters").document()
        doc_ref.set(chapter)
        chapter_slug_to_id[chapter["slug"]] = doc_ref.id
    print(f"Seeded {len(chapter_slug_to_id)} chapters")

    lesson_count = 0
    for lesson in data.get("lessons", []):
        person_slug = lesson.pop("personSlug", None)
        chapter_slug = lesson.pop("chapterSlug", None)
        lesson["personId"] = person_slug_to_id.get(person_slug, "")
        lesson["chapterId"] = chapter_slug_to_id.get(chapter_slug, "")
        db.collection("lessons").document().set(lesson)
        lesson_count += 1
    print(f"Seeded {lesson_count} lessons")

    quote_count = 0
    for quote in data.get("quotes", []):
        person_slug = quote.pop("personSlug", None)
        quote["personId"] = person_slug_to_id.get(person_slug, "")
        db.collection("quotes").document().set(quote)
        quote_count += 1
    print(f"Seeded {quote_count} quotes")

    print("Done.")


if __name__ == "__main__":
    main()
