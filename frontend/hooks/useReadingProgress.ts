"use client";

import { useCallback, useEffect, useState } from "react";
import { getProgressForChapter, upsertProgress } from "@/lib/api/progress";
import type { ReadingProgress } from "@/types";
import { useAuth } from "./useAuth";

export function useReadingProgress(chapterId: string) {
  const { user } = useAuth();
  const [progress, setProgress] = useState<ReadingProgress | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!user) {
      queueMicrotask(() => {
        setProgress(null);
        setLoading(false);
      });
      return;
    }
    queueMicrotask(() => setLoading(true));
    getProgressForChapter(chapterId)
      .then(setProgress)
      .catch(() => setProgress(null))
      .finally(() => setLoading(false));
  }, [user, chapterId]);

  const update = useCallback(
    async (percentComplete: number, lastPositionBlockIndex: number) => {
      if (!user) return;
      const updated = await upsertProgress(chapterId, { percentComplete, lastPositionBlockIndex });
      setProgress(updated);
    },
    [user, chapterId]
  );

  return { progress, loading, update };
}
