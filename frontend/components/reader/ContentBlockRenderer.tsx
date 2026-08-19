import type { ContentBlock } from "@/types";

export function ContentBlockRenderer({ block }: { block: ContentBlock }) {
  switch (block.type) {
    case "quote":
      return (
        <blockquote className="my-6 border-l-2 pl-4 italic text-muted-foreground">
          {block.content}
        </blockquote>
      );
    case "stat":
      return (
        <p className="my-6 rounded-md bg-muted p-4 text-center text-lg font-semibold">
          {block.content}
        </p>
      );
    case "image":
      // eslint-disable-next-line @next/next/no-img-element
      return <img src={block.content} alt="" className="my-6 w-full rounded-md" />;
    case "paragraph":
    default:
      return <p className="my-4 leading-relaxed">{block.content}</p>;
  }
}
