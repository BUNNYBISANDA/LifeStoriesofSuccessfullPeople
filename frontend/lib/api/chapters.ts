import { publicApiClient } from "./publicClient";
import type { ChapterDetail } from "@/types";

export function getChapterBySlug(slug: string) {
  return publicApiClient.get<ChapterDetail>(`/api/chapters/${slug}`);
}
