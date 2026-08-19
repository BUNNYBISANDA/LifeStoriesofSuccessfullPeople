import { apiClient } from "./client";
import type { Bookmark } from "@/types";

export function getBookmarks() {
  return apiClient.get<Bookmark[]>("/api/bookmarks");
}

export function addBookmark(chapterId: string) {
  return apiClient.post<Bookmark>("/api/bookmarks", { chapterId });
}

export function removeBookmark(id: string) {
  return apiClient.delete<void>(`/api/bookmarks/${id}`);
}
