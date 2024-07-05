export interface SignupResult {
  success: boolean;
  message: string;
  role?: "User" | "Librarian";
  token?: string;
}
