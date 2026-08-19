"use client";

import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useAuth } from "@/hooks/useAuth";
import { useBookmarks } from "@/hooks/useBookmarks";

export default function BookmarksPage() {
  const { user, loading: authLoading } = useAuth();
  const { bookmarks, loading, remove } = useBookmarks();

  if (!authLoading && !user) {
    return (
      <div className="mx-auto max-w-md px-4 py-20 text-center">
        <h1 className="mb-4 text-2xl font-bold">Log in to see your bookmarks</h1>
        <Link href="/login">
          <Button>Log in</Button>
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-3xl px-4 py-12">
      <h1 className="mb-6 text-3xl font-bold">Bookmarks</h1>

      {loading ? <p className="text-muted-foreground">Loading...</p> : null}

      {!loading && bookmarks.length === 0 ? (
        <p className="text-muted-foreground">No bookmarks yet. Start reading and save chapters you like.</p>
      ) : null}

      <div className="space-y-3">
        {bookmarks.map((bookmark) => (
          <Card key={bookmark.id}>
            <CardHeader className="flex-row items-center justify-between">
              <CardTitle className="text-base">Chapter {bookmark.chapterId}</CardTitle>
              <Button variant="ghost" size="sm" onClick={() => remove(bookmark.id)}>
                Remove
              </Button>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-muted-foreground">
                Saved {new Date(bookmark.createdAt).toLocaleDateString()}
              </p>
            </CardContent>
          </Card>
        ))}
      </div>
    </div>
  );
}
