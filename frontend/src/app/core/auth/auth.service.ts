import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, throwError } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import {
  AuthResponse,
  CurrentUser,
  GoogleAuthRequest,
  LoginRequest,
  RegisterRequest,
} from './models/user.model';
import { TranslationService, SupportedLanguage } from '../i18n/translation.service';

const TOKEN_KEY = 'hp_token';
const USER_KEY = 'hp_user';
const REFRESH_KEY = 'hp_refresh_token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly _token = signal<string | null>(localStorage.getItem(TOKEN_KEY));
  private readonly _user = signal<CurrentUser | null>(this.loadUser());

  private readonly ts = inject(TranslationService);

  /** Publicly readable signals */
  readonly token = this._token.asReadonly();
  readonly user = this._user.asReadonly();
  readonly isLoggedIn = computed(() => this._token() !== null);

  constructor(private http: HttpClient, private router: Router) {
    const stored = this._user();
    if (stored?.language) {
      this.ts.setLanguage(stored.language as SupportedLanguage);
    }
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/register`, request)
      .pipe(tap(res => this.persist(res)));
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/login`, request)
      .pipe(tap(res => this.persist(res)));
  }

  googleAuth(request: GoogleAuthRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/google`, request)
      .pipe(tap(res => this.persist(res)));
  }

  /** Exchange the stored refresh token for a new access token (+ rotated refresh token). */
  refresh(): Observable<string> {
    const rt = localStorage.getItem(REFRESH_KEY);
    if (!rt) return throwError(() => new Error('no_refresh_token'));

    return this.http
      .post<{ token: string; refreshToken: string }>(
        `${environment.apiUrl}/auth/refresh`,
        { refreshToken: rt },
      )
      .pipe(
        tap(res => {
          localStorage.setItem(TOKEN_KEY, res.token);
          this._token.set(res.token);
          localStorage.setItem(REFRESH_KEY, res.refreshToken);
        }),
        map(res => res.token),
      );
  }

  updateLanguage(lang: SupportedLanguage): void {
    const current = this._user();
    if (!current) return;
    const updated: CurrentUser = { ...current, language: lang };
    localStorage.setItem(USER_KEY, JSON.stringify(updated));
    this._user.set(updated);
    this.ts.setLanguage(lang);
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    localStorage.removeItem(REFRESH_KEY);
    this._token.set(null);
    this._user.set(null);
    this.router.navigate(['/login']);
  }

  verifyEmail(token: string): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/auth/verify-email`, { token });
  }

  resendVerification(email: string): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/auth/resend-verification`, { email });
  }

  forgotPassword(email: string): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/auth/forgot-password`, { email });
  }

  resetPassword(token: string, newPassword: string): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/auth/reset-password`, { token, newPassword });
  }

  changePassword(currentPassword: string, newPassword: string): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/auth/change-password`, { currentPassword, newPassword });
  }

  changeEmail(newEmail: string, currentPassword: string | null): Observable<void> {
    return this.http.put<void>(`${environment.apiUrl}/auth/change-email`, { newEmail, currentPassword });
  }

  confirmEmailChange(token: string): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/auth/confirm-email-change`, { token });
  }

  // Private helpers

  private persist(res: AuthResponse): void {
    const lang = (res.language ?? 'en') as SupportedLanguage;
    const user: CurrentUser = {
      userId: res.userId,
      email: res.email,
      displayName: res.displayName,
      language: lang,
      hasPassword: res.hasPassword,
    };
    localStorage.setItem(TOKEN_KEY, res.token);
    localStorage.setItem(USER_KEY, JSON.stringify(user));
    if (res.refreshToken) {
      localStorage.setItem(REFRESH_KEY, res.refreshToken);
    }
    this._token.set(res.token);
    this._user.set(user);
    this.ts.setLanguage(lang);
  }

  private loadUser(): CurrentUser | null {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as CurrentUser;
    } catch {
      return null;
    }
  }
}
