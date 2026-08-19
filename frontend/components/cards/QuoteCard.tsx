import { Quote as QuoteMark } from "lucide-react";
import type { Quote } from "@/types";

export function QuoteCard({ quote }: { quote: Quote }) {
  return (
    <blockquote className="glow-candle relative rounded-lg border border-border/70 bg-card px-8 py-8 text-center shadow-cozy">
      <QuoteMark
        className="mx-auto mb-3 size-6 text-accent-foreground/40"
        strokeWidth={1.5}
        aria-hidden
      />
      <p className="font-heading text-xl leading-snug text-foreground italic sm:text-2xl">
        {quote.text}
      </p>
      {quote.context ? (
        <footer className="mt-4 text-sm text-muted-foreground">— {quote.context}</footer>
      ) : null}
    </blockquote>
  );
}
