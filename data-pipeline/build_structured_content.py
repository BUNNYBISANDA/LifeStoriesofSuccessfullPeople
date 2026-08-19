"""Tailored structuring pass for "Dad's Life Lessons" — turns output/raw_text.json
(pages 5-16, one profile per page) into output/structured_content.json matching the
Firestore schema: people / chapters / lessons / quotes.

Unlike structure_content.py's generic one-page-per-person template, this parses the
book's actual section headings (The journey / The struggle / How the person responded /
What success looked like / Lessons for you / IMPORTANT note / Try this yourself) and
skips front-matter/back-matter pages that aren't person profiles.

Usage:
    python build_structured_content.py
"""

import json
import re
from pathlib import Path

RAW_PATH = Path(__file__).parent / "output" / "raw_text.json"
OUTPUT_PATH = Path(__file__).parent / "output" / "structured_content.json"

FIELD_BY_NAME = {
    "Cristiano Ronaldo": "Sports",
    "Lionel Messi": "Sports",
    "Jack Ma": "Business",
    "Bill Gates": "Business",
    "Barack Obama": "Politics",
    "Mike Tyson": "Sports",
    "Lance Armstrong": "Sports",
    "Novak Djokovic": "Sports",
    "Serena Williams": "Sports",
    "MS Dhoni": "Sports",
    "Virat Kohli": "Sports",
    "Sachin Tendulkar": "Sports",
}

# Portrait photos, sourced from each person's Wikipedia page (Wikimedia Commons,
# freely licensed) via the Wikipedia REST summary API's default 330px thumbnail —
# Wikimedia's on-demand thumbnailer rejects some other requested widths with a 400.
IMAGE_BY_NAME = {
    "Cristiano Ronaldo": "https://upload.wikimedia.org/wikipedia/commons/thumb/2/26/Cristiano_Ronaldo_Croatia_v_Portugal_2_July_2026-075_%28cropped%29.jpg/330px-Cristiano_Ronaldo_Croatia_v_Portugal_2_July_2026-075_%28cropped%29.jpg",
    "Lionel Messi": "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c8/Leo_Messi_Argentina_v_Egypt_7_July_2026-1.jpg/330px-Leo_Messi_Argentina_v_Egypt_7_July_2026-1.jpg",
    "Jack Ma": "https://upload.wikimedia.org/wikipedia/commons/thumb/6/6d/20th_Anniversary_Schwab_Foundation_Gala_Dinner_%2844887783681%29_%28cropped%29.jpg/330px-20th_Anniversary_Schwab_Foundation_Gala_Dinner_%2844887783681%29_%28cropped%29.jpg",
    "Bill Gates": "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d9/Bill_Gates_at_the_European_Commission_-_P067383-987995_%28cropped%29_5.jpg/330px-Bill_Gates_at_the_European_Commission_-_P067383-987995_%28cropped%29_5.jpg",
    "Barack Obama": "https://upload.wikimedia.org/wikipedia/commons/thumb/8/8d/President_Barack_Obama.jpg/330px-President_Barack_Obama.jpg",
    "Mike Tyson": "https://upload.wikimedia.org/wikipedia/commons/thumb/e/ee/Mike_Tyson_Photo_Op_GalaxyCon_Austin_2023.jpg/330px-Mike_Tyson_Photo_Op_GalaxyCon_Austin_2023.jpg",
    "Lance Armstrong": "https://upload.wikimedia.org/wikipedia/commons/thumb/4/47/Lance_Armstrong_%28Tour_Down_Under_2009%29.jpg/330px-Lance_Armstrong_%28Tour_Down_Under_2009%29.jpg",
    "Novak Djokovic": "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d1/Novak_Djokovic_Paris_2024_Olympic_Games_%28cropped%29.jpg/330px-Novak_Djokovic_Paris_2024_Olympic_Games_%28cropped%29.jpg",
    "Serena Williams": "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2f/Guests_at_the_2026_Met_Gala_209_%28cropped%29.jpg/330px-Guests_at_the_2026_Met_Gala_209_%28cropped%29.jpg",
    "MS Dhoni": "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d5/MS_Dhoni_%28Prabhav_%2723_-_RiGI_2023%29.jpg/330px-MS_Dhoni_%28Prabhav_%2723_-_RiGI_2023%29.jpg",
    "Virat Kohli": "https://upload.wikimedia.org/wikipedia/commons/thumb/e/ef/Virat_Kohli_during_the_India_vs_Aus_4th_Test_match_at_Narendra_Modi_Stadium_on_09_March_2023.jpg/330px-Virat_Kohli_during_the_India_vs_Aus_4th_Test_match_at_Narendra_Modi_Stadium_on_09_March_2023.jpg",
    "Sachin Tendulkar": "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3e/The_cricket_legend_Sachin_Tendulkar_at_the_Oval_Maidan_in_Mumbai_During_the_Duke_and_Duchess_of_Cambridge_Visit%2826271019082%29.jpg/330px-The_cricket_legend_Sachin_Tendulkar_at_the_Oval_Maidan_in_Mumbai_During_the_Duke_and_Duchess_of_Cambridge_Visit%2826271019082%29.jpg",
}

HEADINGS = [
    "The journey",
    "The struggle",
    "How the person responded",
    "What success looked like",
    "Lessons for you",
    "IMPORTANT",  # matches "IMPORTANT — LEARN THE PRINCIPLE, NOT THE PERSON"
    "Try this yourself",
]


def slugify(text: str) -> str:
    text = text.lower().strip()
    text = re.sub(r"[^a-z0-9]+", "-", text)
    return text.strip("-")


def category_for_theme(theme: str) -> str:
    t = theme.lower()
    if "reject" in t or "fail" in t:
        return "failure"
    if "curiosity" in t or "passion" in t:
        return "passion"
    if "practice" in t or "discipline" in t or "standards" in t or "fitness" in t or "repetition" in t or "mastery" in t:
        return "hard-work"
    return "mindset"


def split_sections(body: str) -> dict[str, str]:
    """Split page body into {heading: text} using HEADINGS as delimiters."""
    positions = []
    for heading in HEADINGS:
        idx = body.find(heading)
        if idx != -1:
            positions.append((idx, heading))
    positions.sort()

    sections: dict[str, str] = {}
    for i, (idx, heading) in enumerate(positions):
        start = idx + len(heading)
        end = positions[i + 1][0] if i + 1 < len(positions) else len(body)
        text = body[start:end].strip()
        text = text.lstrip(":").strip()
        # Drop the "— LEARN THE PRINCIPLE, NOT THE PERSON" subheading line that
        # immediately follows the "IMPORTANT" delimiter, keeping only the note itself.
        if heading == "IMPORTANT":
            text = re.sub(r"^—[^\n]*\n", "", text).strip()
        sections[heading] = text
    return sections


def flatten(text: str) -> str:
    """Collapse PDF line-wrap newlines within a prose block into spaces."""
    return " ".join(line.strip() for line in text.split("\n") if line.strip())


def bullets_to_sentences(text: str) -> list[str]:
    """Bullet lines start with a bullet char; split into a clean list of strings."""
    lines = [ln.strip(" •→.").rstrip(".") for ln in text.split("\n")]
    return [ln for ln in lines if ln]


def bullets_to_prose(text: str) -> str:
    """Join bullet points into a single readable sentence-per-clause paragraph."""
    sentences = bullets_to_sentences(text)
    return " ".join(f"{s}." for s in sentences)


def parse_person_page(text: str) -> dict:
    lines = text.split("\n")
    # Heading line looks like "5\n1. Cristiano Ronaldo" -> find "N. Name"
    name = None
    order = None
    theme = ""
    for i, line in enumerate(lines):
        m = re.match(r"^(\d+)\.\s+(.+)$", line.strip())
        if m:
            order = int(m.group(1))
            name = m.group(2).strip()
            if i + 1 < len(lines) and lines[i + 1].startswith("Lesson theme:"):
                theme = lines[i + 1].replace("Lesson theme:", "").strip()
            break

    if not name:
        return None

    body = "\n".join(lines[lines.index(f"{order}. {name}") + 1 :])
    sections = split_sections(body)

    journey = flatten(sections.get("The journey", ""))
    struggle_bullets = bullets_to_sentences(sections.get("The struggle", ""))
    response_bullets = bullets_to_sentences(sections.get("How the person responded", ""))
    outcome = flatten(sections.get("What success looked like", ""))
    lesson_bullets = bullets_to_sentences(sections.get("Lessons for you", ""))
    important_note = flatten(sections.get("IMPORTANT", ""))
    try_bullets = bullets_to_sentences(sections.get("Try this yourself", ""))

    slug = slugify(name)
    tags = [t.strip() for t in theme.split("+") if t.strip()]

    content_blocks = [
        {"type": "paragraph", "content": journey},
        {"type": "paragraph", "content": "The struggle: " + bullets_to_prose(sections.get("The struggle", ""))},
        {"type": "paragraph", "content": "How they responded: " + bullets_to_prose(sections.get("How the person responded", ""))},
        {"type": "paragraph", "content": "What success looked like: " + outcome},
        {"type": "quote", "content": important_note},
        {"type": "paragraph", "content": "Lessons for you: " + bullets_to_prose(sections.get("Lessons for you", ""))},
        {"type": "paragraph", "content": "Try this yourself: " + bullets_to_prose(sections.get("Try this yourself", ""))},
    ]
    content_blocks = [b for b in content_blocks if b["content"].strip()]

    word_count = sum(len(b["content"].split()) for b in content_blocks)

    return {
        "person": {
            "slug": slug,
            "name": name,
            "era": "21st century",
            "field": FIELD_BY_NAME.get(name, ""),
            "summary": theme,
            "imageUrl": IMAGE_BY_NAME.get(name, ""),
            "failureCount": len(struggle_bullets),
            "tags": tags,
        },
        "chapter": {
            "personSlug": slug,
            "slug": f"{slug}-story",
            "title": name,
            "order": order,
            "estimatedReadMinutes": max(1, word_count // 200),
            "contentBlocks": content_blocks,
        },
        "lessons": [
            {
                "personSlug": slug,
                "chapterSlug": f"{slug}-story",
                "text": lesson,
                "category": category_for_theme(theme),
                "isFeatured": i == 0,
            }
            for i, lesson in enumerate(lesson_bullets)
        ],
        "quote": {
            "personSlug": slug,
            "text": lesson_bullets[0] if lesson_bullets else important_note,
            "context": f"From “{name}”, Dad's Life Lessons",
        },
    }


def main():
    if not RAW_PATH.exists():
        raise SystemExit(f"Missing {RAW_PATH} — run extract_pdf.py first.")

    raw_pages = json.loads(RAW_PATH.read_text(encoding="utf-8"))
    # Person profiles live on pages 5-16 in this book; other pages (front/back
    # matter, rules lists, challenge tables) contain numbered lists that would
    # otherwise false-match the "N. Name" heading pattern.
    raw_pages = [p for p in raw_pages if 5 <= p["page"] <= 16]

    people = []
    chapters = []
    lessons = []
    quotes = []

    for page in raw_pages:
        parsed = parse_person_page(page["text"])
        if parsed is None:
            continue
        people.append(parsed["person"])
        chapters.append(parsed["chapter"])
        lessons.extend(parsed["lessons"])
        quotes.append(parsed["quote"])

    people.sort(key=lambda p: next(c["order"] for c in chapters if c["personSlug"] == p["slug"]))
    chapters.sort(key=lambda c: c["order"])

    structured = {"people": people, "chapters": chapters, "lessons": lessons, "quotes": quotes}

    OUTPUT_PATH.write_text(json.dumps(structured, indent=2), encoding="utf-8")
    print(f"Wrote {len(people)} people, {len(chapters)} chapters, {len(lessons)} lessons, {len(quotes)} quotes -> {OUTPUT_PATH}")


if __name__ == "__main__":
    main()
