"use client";

import { useEffect, useState } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Progress } from "@/components/ui/progress";
import { ContentBlockRenderer } from "./ContentBlockRenderer";
import { useAuth } from "@/hooks/useAuth";
import { useReadingProgress } from "@/hooks/useReadingProgress";
import { useBookmarks } from "@/hooks/useBookmarks";
import type { ChapterDetail } from "@/types";

export function ReaderView({ chapter }: { chapter: ChapterDetail }) {
  const { user } = useAuth();
  const { progress, update } = useReadingProgress(chapter.id);
  const { bookmarks, add, remove } = useBookmarks();
  const [visibleIndex, setVisibleIndex] = useState(0);

  const bookmark = bookmarks.find((b) => b.chapterId === chapter.id);
  const percentComplete = Math.round(((visibleIndex + 1) / chapter.contentBlocks.length) * 100);

  useEffect(() => {
    if (!user) return;
    const timeout = setTimeout(() => {
      update(percentComplete, visibleIndex);
    }, 1500);
    return () => clearTimeout(timeout);
  }, [percentComplete, visibleIndex, user, update]);

  async function toggleBookmark() {
    if (!user) {
      toast("Log in to save bookmarks");
      return;
    }
    if (bookmark) {
      await remove(bookmark.id);
      toast("Bookmark removed");
    } else {
      await add(chapter.id);
      toast("Bookmark added");
    }
  }

  return (
    <div className="bg-secondary/40 py-10">
      <article className="mx-auto max-w-2xl rounded-lg border border-border/60 bg-card px-6 py-10 shadow-cozy-lg sm:px-12">
        <div className="mb-6 flex items-center justify-between gap-4">
          <Progress value={progress?.percentComplete ?? percentComplete} className="h-1.5" />
          <Button variant="outline" size="sm" onClick={toggleBookmark}>
            {bookmark ? "Bookmarked" : "Bookmark"}
          </Button>
        </div>

        <p className="mb-1 text-xs tracking-[0.2em] text-muted-foreground uppercase">Chapter</p>
        <h1 className="mb-2 text-3xl font-bold">{chapter.title}</h1>
        <p className="mb-8 text-sm text-muted-foreground">
          {chapter.estimatedReadMinutes} min read
        </p>

        <div className="reader-content">
          {chapter.contentBlocks.map((block, index) => (
            <div
              key={index}
              className={index === 0 ? "reader-dropcap" : undefined}
              onMouseEnter={() => setVisibleIndex((prev) => Math.max(prev, index))}
            >
              <ContentBlockRenderer block={block} />
            </div>
          ))}
        </div>
      </article>
    </div>
  );
}
