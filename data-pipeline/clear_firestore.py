"""Delete all documents in the content collections (people, chapters, lessons,
quotes) before re-seeding, so re-running the pipeline doesn't create duplicates.
Leaves user-owned collections (users, bookmarks, readingProgress, highlights) alone.

Usage:
    python clear_firestore.py path/to/service-account.json
"""

import sys

import firebase_admin
from firebase_admin import credentials, firestore

COLLECTIONS = ["people", "chapters", "lessons", "quotes"]


def main():
    if len(sys.argv) != 2:
        print("Usage: python clear_firestore.py <path-to-service-account.json>")
        sys.exit(1)

    cred = credentials.Certificate(sys.argv[1])
    firebase_admin.initialize_app(cred)
    db = firestore.client()

    for name in COLLECTIONS:
        docs = list(db.collection(name).stream())
        for doc in docs:
            doc.reference.delete()
        print(f"Cleared {len(docs)} docs from {name}")


if __name__ == "__main__":
    main()
