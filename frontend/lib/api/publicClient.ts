const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5000";

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const res = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers: { "Content-Type": "application/json", ...options.headers },
  });

  if (!res.ok) {
    const message = await res.text().catch(() => res.statusText);
    throw new Error(`API ${res.status}: ${message}`);
  }

  return res.json() as Promise<T>;
}

/**
 * For public, unauthenticated endpoints only. Safe to call from Server
 * Components — unlike `apiClient`, it never touches the Firebase client SDK,
 * which can't run during server-side rendering/build.
 */
export const publicApiClient = {
  get: <T>(path: string) => request<T>(path, { method: "GET" }),
};
