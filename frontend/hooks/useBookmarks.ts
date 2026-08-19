"use client";

import { useCallback, useEffect, useState } from "react";
import { addBookmark, getBookmarks, removeBookmark } from "@/lib/api/bookmarks";
import type { Bookmark } from "@/types";
import { useAuth } from "./useAuth";

export function useBookmarks() {
  const { user } = useAuth();
  const [bookmarks, setBookmarks] = useState<Bookmark[]>([]);
  const [loading, setLoading] = useState(true);

  const refresh = useCallback(async () => {
    if (!user) {
      setBookmarks([]);
      setLoading(false);
      return;
    }
    setLoading(true);
    const data = await getBookmarks();
    setBookmarks(data);
    setLoading(false);
  }, [user]);

  useEffect(() => {
    if (!user) {
      queueMicrotask(() => {
        setBookmarks([]);
        setLoading(false);
      });
      return;
    }
    queueMicrotask(() => setLoading(true));
    getBookmarks()
      .then(setBookmarks)
      .finally(() => setLoading(false));
  }, [user]);

  const add = useCallback(
    async (chapterId: string) => {
      const bookmark = await addBookmark(chapterId);
      setBookmarks((prev) => [...prev, bookmark]);
    },
    []
  );

  const remove = useCallback(async (id: string) => {
    await removeBookmark(id);
    setBookmarks((prev) => prev.filter((b) => b.id !== id));
  }, []);

  return { bookmarks, loading, add, remove, refresh };
}
