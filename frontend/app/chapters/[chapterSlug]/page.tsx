import { notFound } from "next/navigation";
import { ReaderView } from "@/components/reader/ReaderView";
import { getChapterBySlug } from "@/lib/api/chapters";

export default async function ChapterPage({ params }: PageProps<"/chapters/[chapterSlug]">) {
  const { chapterSlug } = await params;

  const chapter = await getChapterBySlug(chapterSlug).catch(() => null);
  if (!chapter) notFound();

  return <ReaderView chapter={chapter} />;
}
