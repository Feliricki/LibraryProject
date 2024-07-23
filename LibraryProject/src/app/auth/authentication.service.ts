import { Injectable, WritableSignal, signal, Signal } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable, tap, pipe } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { LoginRequest } from "./login-request";
import { LoginResult } from "./login-result";
import {SignupRequest} from "./signup-request";
import {SignupResult} from "./signup-result";


@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {

  token: string = "";
  readonly tokenKey: string = "tokenKey";
  readonly isLibrarianKey: string = "isLibrarian";

  private _authenticated: WritableSignal<boolean> = signal(false);
  public authenticated: Signal<boolean> = this._authenticated.asReadonly();

  private _isLibrarian: WritableSignal<boolean> = signal(false);

  private _currentUser: WritableSignal<null | string> = signal(null);
  public currentUser: Signal<null | string> = this._currentUser.asReadonly();

  private curTimeout?: number = undefined;

  constructor(private http: HttpClient) { }


  isAuthenticated(): boolean {
    const authenticated = this.getToken() !== null;
    return authenticated;
  }

  isLibrarian(): boolean {
    return localStorage.getItem(this.isLibrarianKey) !== null;
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private setToken(token: string | undefined) {
    if (!token) return;
    localStorage.setItem(this.tokenKey, token);
    this.token = token;
  }

  private setRole(result: SignupResult | LoginResult){
    const isLibrarian = result.role === "Librarian";
    this._isLibrarian.set(isLibrarian);
    if (isLibrarian){
      localStorage.setItem(this.isLibrarianKey, "true");
    }
  }

  // NOTE: The loginRequest and signupRequest need to be validated in the form component.
  login(request: LoginRequest): Observable<LoginResult> {
    let url = environment.baseUrl + "api/Account/Login";
    return this.http.post<LoginResult>(url, request).pipe(
      tap(res => {
        if (res.success){
          this.setToken(res.token);
          this.setRole(res);
          this._authenticated.set(true);
          this._currentUser.set(request.username);

          // this.curTimeout = setTimeout(() => {
          //   this.logout();
          // }, 1000 * 60 * 30);
        }
      })
    );
  }

  signup(request: SignupRequest): Observable<SignupResult> {
    let url = environment.baseUrl + "api/Account/Signup";
    return this.http.post<SignupResult>(url, request).pipe(
      tap(res => {
        if (res.success){
          this.setToken(res.token);
          this.setRole(res);
          this._authenticated.set(true);
          this._currentUser.set(request.username);

          // this.curTimeout = setTimeout(() => {
          //   this.logout();
          // }, 1000 * 60 * 30);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.isLibrarianKey);
    this._authenticated.set(false);
    this._isLibrarian.set(false);
  }

}
