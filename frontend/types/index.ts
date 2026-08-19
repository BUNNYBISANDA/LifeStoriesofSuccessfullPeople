export interface Person {
  id: string;
  name: string;
  slug: string;
  era: string;
  field: string;
  summary: string;
  imageUrl: string;
  failureCount: number;
  tags: string[];
}

export interface ChapterSummary {
  id: string;
  title: string;
  slug: string;
  order: number;
  estimatedReadMinutes: number;
}

export interface PersonDetail {
  person: Person;
  chapters: ChapterSummary[];
}

export type ContentBlockType = "paragraph" | "quote" | "image" | "stat";

export interface ContentBlock {
  type: ContentBlockType;
  content: string;
}

export interface ChapterDetail {
  id: string;
  personId: string;
  title: string;
  slug: string;
  order: number;
  contentBlocks: ContentBlock[];
  estimatedReadMinutes: number;
}

export type LessonCategory = "failure" | "passion" | "hard-work" | "mindset";

export interface Lesson {
  id: string;
  personId: string;
  chapterId: string;
  text: string;
  category: LessonCategory;
  isFeatured: boolean;
}

export interface Quote {
  id: string;
  personId: string;
  text: string;
  context: string;
}

export interface Bookmark {
  id: string;
  chapterId: string;
  createdAt: string;
}

export interface ReadingProgress {
  chapterId: string;
  percentComplete: number;
  lastPositionBlockIndex: number;
  completed: boolean;
  updatedAt: string;
}

export interface Highlight {
  id: string;
  chapterId: string;
  blockIndex: number;
  selectedText: string;
  note: string;
  createdAt: string;
}

export interface UserProfile {
  uid: string;
  displayName: string;
  email: string;
  joinedAt: string;
  readingStreak: number;
}
