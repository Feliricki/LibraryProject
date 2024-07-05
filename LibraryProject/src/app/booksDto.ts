export interface BooksDto {
  title: string;
  bookCoverUrl: string;
  description: string;
  author: string;
  publisher: string;
  available: boolean;
  daysUntilAvailable: number;
  publicationDate: Date;
  category: string;
  isbn: number;
  pageCount: number;
}
