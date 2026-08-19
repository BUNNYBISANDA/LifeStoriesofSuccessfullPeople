import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import type { Lesson } from "@/types";

const categoryLabels: Record<string, string> = {
  failure: "Failure",
  passion: "Passion",
  "hard-work": "Hard Work",
  mindset: "Mindset",
};

export function LessonCard({ lesson }: { lesson: Lesson }) {
  return (
    <Card>
      <CardContent className="space-y-3 pt-6">
        <Badge variant="outline">{categoryLabels[lesson.category] ?? lesson.category}</Badge>
        <p className="text-sm leading-relaxed">{lesson.text}</p>
      </CardContent>
    </Card>
  );
}
