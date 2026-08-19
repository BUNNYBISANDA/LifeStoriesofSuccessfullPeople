"use client";

import Link from "next/link";
import { BookMarked } from "lucide-react";
import { useAuth } from "@/hooks/useAuth";
import { logout } from "@/lib/firebase/auth";
import { Button } from "@/components/ui/button";

const links = [
  { href: "/library", label: "Library" },
  { href: "/lessons", label: "Lessons" },
  { href: "/search", label: "Search" },
];

export function Navbar() {
  const { user, loading } = useAuth();

  return (
    <header className="border-b bg-card/60 backdrop-blur-sm">
      <nav className="mx-auto flex h-16 max-w-6xl items-center justify-between px-4">
        <Link href="/" className="flex items-center gap-2 font-heading text-lg font-semibold">
          <BookMarked className="size-5 text-primary" strokeWidth={1.75} />
          Success Lessons
        </Link>

        <div className="hidden gap-6 md:flex">
          {links.map((link) => (
            <Link key={link.href} href={link.href} className="text-sm text-muted-foreground hover:text-foreground">
              {link.label}
            </Link>
          ))}
        </div>

        <div className="flex items-center gap-3">
          {loading ? null : user ? (
            <>
              <Link href="/bookmarks" className="text-sm text-muted-foreground hover:text-foreground">
                Bookmarks
              </Link>
              <Link href="/profile" className="text-sm text-muted-foreground hover:text-foreground">
                Profile
              </Link>
              <Button variant="outline" size="sm" onClick={() => logout()}>
                Log out
              </Button>
            </>
          ) : (
            <>
              <Link href="/login">
                <Button variant="ghost" size="sm">Log in</Button>
              </Link>
              <Link href="/register">
                <Button size="sm">Sign up</Button>
              </Link>
            </>
          )}
        </div>
      </nav>
    </header>
  );
}
