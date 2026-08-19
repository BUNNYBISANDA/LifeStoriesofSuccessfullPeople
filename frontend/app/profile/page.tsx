"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useAuth } from "@/hooks/useAuth";
import { apiClient } from "@/lib/api/client";
import type { UserProfile } from "@/types";

export default function ProfilePage() {
  const { user, loading: authLoading } = useAuth();
  const [profile, setProfile] = useState<UserProfile | null>(null);

  useEffect(() => {
    if (!user) return;
    apiClient.get<UserProfile>("/api/users/me").then(setProfile).catch(() => setProfile(null));
  }, [user]);

  if (!authLoading && !user) {
    return (
      <div className="mx-auto max-w-md px-4 py-20 text-center">
        <h1 className="mb-4 text-2xl font-bold">Log in to see your profile</h1>
        <Link href="/login">
          <Button>Log in</Button>
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-xl px-4 py-12">
      <h1 className="mb-6 text-3xl font-bold">Profile</h1>

      <Card>
        <CardHeader>
          <CardTitle>{profile?.displayName || user?.email}</CardTitle>
        </CardHeader>
        <CardContent className="space-y-2 text-sm text-muted-foreground">
          <p>Email: {profile?.email ?? user?.email}</p>
          <p>Reading streak: {profile?.readingStreak ?? 0} days</p>
          <p>
            Joined:{" "}
            {profile ? new Date(profile.joinedAt).toLocaleDateString() : "—"}
          </p>
        </CardContent>
      </Card>
    </div>
  );
}
