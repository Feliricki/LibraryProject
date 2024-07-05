export interface LoginResult {
  success: boolean;
  message: string;
  role?: "User" | "Librarian";
  token?: string;
}
