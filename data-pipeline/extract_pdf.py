"""Extract raw text from the source PDF, one entry per page, into output/raw_text.json.

Usage:
    python extract_pdf.py path/to/source.pdf
"""

import json
import sys
from pathlib import Path

import fitz  # PyMuPDF

OUTPUT_PATH = Path(__file__).parent / "output" / "raw_text.json"


def extract(pdf_path: str) -> list[dict]:
    doc = fitz.open(pdf_path)
    pages = []
    for page_number, page in enumerate(doc, start=1):
        text = page.get_text("text").strip()
        if text:
            pages.append({"page": page_number, "text": text})
    return pages


def main():
    if len(sys.argv) != 2:
        print("Usage: python extract_pdf.py <path-to-pdf>")
        sys.exit(1)

    pdf_path = sys.argv[1]
    pages = extract(pdf_path)

    OUTPUT_PATH.parent.mkdir(parents=True, exist_ok=True)
    OUTPUT_PATH.write_text(json.dumps(pages, indent=2), encoding="utf-8")

    print(f"Extracted {len(pages)} pages -> {OUTPUT_PATH}")


if __name__ == "__main__":
    main()
