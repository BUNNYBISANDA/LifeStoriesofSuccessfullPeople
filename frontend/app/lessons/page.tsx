import { LessonCard } from "@/components/cards/LessonCard";
import { getLessons } from "@/lib/api/lessons";

const categories = ["failure", "passion", "hard-work", "mindset"];

export default async function LessonsPage({ searchParams }: PageProps<"/lessons">) {
  const { category } = await searchParams;
  const activeCategory = typeof category === "string" ? category : undefined;

  const lessons = await getLessons(activeCategory).catch(() => []);

  return (
    <div className="mx-auto max-w-4xl px-4 py-12">
      <h1 className="mb-6 text-3xl font-bold">Lessons</h1>

      <div className="mb-8 flex flex-wrap gap-2">
        <a
          href="/lessons"
          className={`rounded-full border px-3 py-1 text-sm ${!activeCategory ? "bg-foreground text-background" : ""}`}
        >
          All
        </a>
        {categories.map((cat) => (
          <a
            key={cat}
            href={`/lessons?category=${cat}`}
            className={`rounded-full border px-3 py-1 text-sm ${activeCategory === cat ? "bg-foreground text-background" : ""}`}
          >
            {cat}
          </a>
        ))}
      </div>

      {lessons.length === 0 ? (
        <p className="text-muted-foreground">No lessons found.</p>
      ) : (
        <div className="grid gap-4 sm:grid-cols-2">
          {lessons.map((lesson) => (
            <LessonCard key={lesson.id} lesson={lesson} />
          ))}
        </div>
      )}
    </div>
  );
}
