import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
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
    this._token.set(null);
    this._user.set(null);
    this.router.navigate(['/login']);
  }

  // Private helpers

  private persist(res: AuthResponse): void {
    const lang = (res.language ?? 'en') as SupportedLanguage;
    const user: CurrentUser = {
      userId: res.userId,
      email: res.email,
      displayName: res.displayName,
      language: lang,
    };
    localStorage.setItem(TOKEN_KEY, res.token);
    localStorage.setItem(USER_KEY, JSON.stringify(user));
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
