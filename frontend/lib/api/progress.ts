import { apiClient } from "./client";
import type { ReadingProgress } from "@/types";

export function getAllProgress() {
  return apiClient.get<ReadingProgress[]>("/api/progress");
}

export function getProgressForChapter(chapterId: string) {
  return apiClient.get<ReadingProgress>(`/api/progress/${chapterId}`);
}

export function upsertProgress(
  chapterId: string,
  data: { percentComplete: number; lastPositionBlockIndex: number }
) {
  return apiClient.put<ReadingProgress>(`/api/progress/${chapterId}`, data);
}
