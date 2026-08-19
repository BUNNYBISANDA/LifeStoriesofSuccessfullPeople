import Image from "next/image";
import Link from "next/link";
import { notFound } from "next/navigation";
import { BookOpen } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { getPersonBySlug } from "@/lib/api/people";

export default async function PersonPage({ params }: PageProps<"/library/[personSlug]">) {
  const { personSlug } = await params;

  const detail = await getPersonBySlug(personSlug).catch(() => null);
  if (!detail) notFound();

  const { person, chapters } = detail;

  return (
    <div className="mx-auto max-w-3xl px-4 py-12">
      <div className="mb-10 flex flex-col gap-6 sm:flex-row sm:items-start">
        {person.imageUrl ? (
          <div className="relative aspect-[3/4] w-32 shrink-0 overflow-hidden rounded-lg shadow-cozy-lg ring-1 ring-border/60 sm:w-40">
            <Image
              src={person.imageUrl}
              alt={person.name}
              fill
              sizes="160px"
              className="object-cover object-top grayscale-[15%]"
              priority
            />
          </div>
        ) : null}
        <div>
          <p className="text-xs tracking-widest text-muted-foreground uppercase">
            {person.field} · {person.era}
          </p>
          <h1 className="mb-3 text-4xl font-bold">{person.name}</h1>
          <div className="mb-4 flex flex-wrap gap-2">
            {person.tags.map((tag) => (
              <Badge key={tag} variant="secondary" className="font-normal">
                {tag}
              </Badge>
            ))}
          </div>
          <p className="leading-relaxed text-muted-foreground italic">{person.summary}</p>
        </div>
      </div>

      <h2 className="mb-4 flex items-center gap-2 text-2xl font-semibold">
        <BookOpen className="size-5 text-accent-foreground/70" strokeWidth={1.5} />
        Chapters
      </h2>
      <div className="space-y-3">
        {chapters.map((chapter) => (
          <Link key={chapter.id} href={`/chapters/${chapter.slug}`}>
            <Card className="transition-all duration-300 ease-out hover:-translate-y-1 hover:shadow-cozy-lg">
              <CardHeader>
                <CardTitle className="text-lg">{chapter.title}</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-sm text-muted-foreground">
                  {chapter.estimatedReadMinutes} min read
                </p>
              </CardContent>
            </Card>
          </Link>
        ))}
      </div>
    </div>
  );
}
