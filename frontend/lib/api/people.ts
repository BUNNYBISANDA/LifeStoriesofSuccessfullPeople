import { publicApiClient } from "./publicClient";
import type { Person, PersonDetail } from "@/types";

export function getPeople(params?: { field?: string; tag?: string }) {
  const search = new URLSearchParams(params as Record<string, string>).toString();
  return publicApiClient.get<Person[]>(`/api/people${search ? `?${search}` : ""}`);
}

export function getPersonBySlug(slug: string) {
  return publicApiClient.get<PersonDetail>(`/api/people/${slug}`);
}
