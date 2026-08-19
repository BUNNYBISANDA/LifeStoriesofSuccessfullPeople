import Link from "next/link";
import { Button } from "@/components/ui/button";
import { PersonCard } from "@/components/cards/PersonCard";
import { QuoteCard } from "@/components/cards/QuoteCard";
import { getPeople } from "@/lib/api/people";
import { getRandomQuote } from "@/lib/api/lessons";
import type { Person, Quote } from "@/types";

export default async function HomePage() {
  const [people, quote] = await Promise.allSettled([getPeople(), getRandomQuote()]);

  const featuredPeople: Person[] = people.status === "fulfilled" ? people.value.slice(0, 6) : [];
  const featuredQuote: Quote | null = quote.status === "fulfilled" ? quote.value : null;

  return (
    <div className="mx-auto max-w-6xl px-4 py-16">
      <section className="mb-16 text-center">
        <p className="mb-3 text-xs tracking-[0.2em] text-muted-foreground uppercase">
          A reading room for hard-won lessons
        </p>
        <h1 className="mb-4 text-4xl font-bold tracking-tight sm:text-5xl">
          Success Lessons Platform
        </h1>
        <p className="mx-auto mb-8 max-w-2xl text-lg text-muted-foreground">
          Stories of hard work, passion, and failure from history&apos;s most successful people —
          turned into an interactive reading experience.
        </p>
        <div className="flex justify-center gap-4">
          <Link href="/library">
            <Button size="lg">Explore the Library</Button>
          </Link>
          <Link href="/lessons">
            <Button size="lg" variant="outline">Browse Lessons</Button>
          </Link>
        </div>
      </section>

      {featuredQuote ? (
        <section className="mx-auto mb-16 max-w-xl">
          <QuoteCard quote={featuredQuote} />
        </section>
      ) : null}

      {featuredPeople.length > 0 ? (
        <section>
          <div className="mb-6 flex items-center gap-4">
            <h2 className="text-2xl font-semibold whitespace-nowrap">Featured Stories</h2>
            <div className="h-px flex-1 bg-border" />
          </div>
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {featuredPeople.map((person) => (
              <PersonCard key={person.id} person={person} />
            ))}
          </div>
        </section>
      ) : null}
    </div>
  );
}
