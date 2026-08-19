import Image from "next/image";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import type { Person } from "@/types";

export function PersonCard({ person }: { person: Person }) {
  return (
    <Link href={`/library/${person.slug}`} className="group">
      <Card className="h-full overflow-hidden py-0 shadow-sm ring-border/60 transition-all duration-200 group-hover:-translate-y-0.5 group-hover:shadow-md">
        {person.imageUrl ? (
          <div className="relative aspect-[3/4] w-full overflow-hidden bg-muted">
            <Image
              src={person.imageUrl}
              alt={person.name}
              fill
              sizes="(min-width: 1024px) 20vw, (min-width: 640px) 33vw, 50vw"
              className="object-cover object-top grayscale-[15%] transition-transform duration-300 group-hover:scale-105"
            />
            <div className="absolute inset-x-0 bottom-0 h-16 bg-gradient-to-t from-black/50 to-transparent" />
          </div>
        ) : null}
        <CardHeader className="pt-4">
          <CardTitle className="text-lg">{person.name}</CardTitle>
          <p className="text-xs tracking-wide text-muted-foreground uppercase">
            {person.field} · {person.era}
          </p>
        </CardHeader>
        <CardContent className="space-y-3 pb-4">
          <p className="line-clamp-2 text-sm text-muted-foreground italic">{person.summary}</p>
          <div className="flex flex-wrap gap-1.5">
            {person.tags.map((tag) => (
              <Badge key={tag} variant="secondary" className="font-normal">
                {tag}
              </Badge>
            ))}
          </div>
        </CardContent>
      </Card>
    </Link>
  );
}
