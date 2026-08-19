"""Turn output/raw_text.json into structured content matching the Firestore schema:
people / chapters / lessons / quotes.

This step is semi-manual by design — splitting a PDF's raw text into people,
chapters, and pull-quotes needs a human judgment call. This script gives you a
starting structure to edit by hand (or extend with your own heuristics/LLM call)
before running seed_firestore.py.

Usage:
    python structure_content.py
"""

import json
import re
from pathlib import Path

RAW_PATH = Path(__file__).parent / "output" / "raw_text.json"
OUTPUT_PATH = Path(__file__).parent / "output" / "structured_content.json"


def slugify(text: str) -> str:
    text = text.lower().strip()
    text = re.sub(r"[^a-z0-9]+", "-", text)
    return text.strip("-")


def build_template(raw_pages: list[dict]) -> dict:
    """Produces one placeholder person/chapter per page as a starting point.

    Edit the resulting JSON by hand: merge pages into chapters, split chapters
    across people, and pull out featured lessons/quotes.
    """
    people = []
    chapters = []
    lessons = []
    quotes = []

    for page in raw_pages:
        person_name = f"Person from page {page['page']}"  # TODO: replace with real name
        person_slug = slugify(person_name)

        people.append(
            {
                "slug": person_slug,
                "name": person_name,
                "era": "",
                "field": "",
                "summary": "",
                "imageUrl": "",
                "failureCount": 0,
                "tags": [],
            }
        )

        chapters.append(
            {
                "personSlug": person_slug,
                "slug": f"{person_slug}-chapter-1",
                "title": "Chapter 1",
                "order": 1,
                "estimatedReadMinutes": max(1, len(page["text"].split()) // 200),
                "contentBlocks": [
                    {"type": "paragraph", "content": page["text"]},
                ],
            }
        )

    return {
        "people": people,
        "chapters": chapters,
        "lessons": lessons,
        "quotes": quotes,
    }


def main():
    if not RAW_PATH.exists():
        raise SystemExit(f"Missing {RAW_PATH} — run extract_pdf.py first.")

    raw_pages = json.loads(RAW_PATH.read_text(encoding="utf-8"))
    structured = build_template(raw_pages)

    OUTPUT_PATH.write_text(json.dumps(structured, indent=2), encoding="utf-8")
    print(f"Wrote template structure ({len(structured['people'])} people) -> {OUTPUT_PATH}")
    print("Edit this file by hand before running seed_firestore.py.")


if __name__ == "__main__":
    main()
