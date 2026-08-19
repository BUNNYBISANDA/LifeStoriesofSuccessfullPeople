import { publicApiClient } from "./publicClient";
import type { Lesson, Quote } from "@/types";

export function getLessons(category?: string) {
  const search = category ? `?category=${encodeURIComponent(category)}` : "";
  return publicApiClient.get<Lesson[]>(`/api/lessons${search}`);
}

export function getFeaturedLessons() {
  return publicApiClient.get<Lesson[]>("/api/lessons/featured");
}

export function getRandomQuote() {
  return publicApiClient.get<Quote>("/api/quotes/random");
}
