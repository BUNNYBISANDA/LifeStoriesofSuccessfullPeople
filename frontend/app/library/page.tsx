import { PersonCard } from "@/components/cards/PersonCard";
import { getPeople } from "@/lib/api/people";

export default async function LibraryPage() {
  const people = await getPeople().catch(() => []);

  return (
    <div className="mx-auto max-w-6xl px-4 py-12">
      <h1 className="mb-8 text-3xl font-bold">Library</h1>

      {people.length === 0 ? (
        <p className="text-muted-foreground">No stories yet — check back soon.</p>
      ) : (
        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {people.map((person) => (
            <PersonCard key={person.id} person={person} />
          ))}
        </div>
      )}
    </div>
  );
}
