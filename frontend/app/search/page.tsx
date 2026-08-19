"use client";

import { useState } from "react";
import { Input } from "@/components/ui/input";
import { PersonCard } from "@/components/cards/PersonCard";
import { LessonCard } from "@/components/cards/LessonCard";
import { apiClient } from "@/lib/api/client";
import type { Person, Lesson } from "@/types";

interface SearchResult {
  people: Person[];
  lessons: Lesson[];
}

export default function SearchPage() {
  const [query, setQuery] = useState("");
  const [results, setResults] = useState<SearchResult | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleChange(value: string) {
    setQuery(value);
    if (!value.trim()) {
      setResults(null);
      return;
    }
    setLoading(true);
    try {
      const data = await apiClient.get<SearchResult>(`/api/search?q=${encodeURIComponent(value)}`);
      setResults(data);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="mx-auto max-w-4xl px-4 py-12">
      <h1 className="mb-6 text-3xl font-bold">Search</h1>
      <Input
        placeholder="Search people, lessons..."
        value={query}
        onChange={(e) => handleChange(e.target.value)}
        className="mb-8"
      />

      {loading ? <p className="text-muted-foreground">Searching...</p> : null}

      {results && results.people.length > 0 ? (
        <section className="mb-8">
          <h2 className="mb-4 text-xl font-semibold">People</h2>
          <div className="grid gap-4 sm:grid-cols-2">
            {results.people.map((person) => (
              <PersonCard key={person.id} person={person} />
            ))}
          </div>
        </section>
      ) : null}

      {results && results.lessons.length > 0 ? (
        <section>
          <h2 className="mb-4 text-xl font-semibold">Lessons</h2>
          <div className="grid gap-4 sm:grid-cols-2">
            {results.lessons.map((lesson) => (
              <LessonCard key={lesson.id} lesson={lesson} />
            ))}
          </div>
        </section>
      ) : null}

      {results && results.people.length === 0 && results.lessons.length === 0 ? (
        <p className="text-muted-foreground">No results found.</p>
      ) : null}
    </div>
  );
}
