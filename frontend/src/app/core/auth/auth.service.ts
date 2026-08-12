import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { API_BASE } from '../config/api';
import { AuthResponse, LoginRequest, RegisterRequest, UserDto } from '../models/auth.models';

const TOKEN_KEY = 'mf_token';
const USER_KEY = 'mf_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${API_BASE}/api/auth`;

  private readonly _user = signal<UserDto | null>(this.restoreUser());
  readonly currentUser = this._user.asReadonly();
  readonly isAuthenticated = computed(() => this._user() !== null);

  register(body: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/register`, body).pipe(tap((r) => this.setSession(r)));
  }

  login(body: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/login`, body).pipe(tap((r) => this.setSession(r)));
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this._user.set(null);
  }

  get token(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  private setSession(response: AuthResponse): void {
    localStorage.setItem(TOKEN_KEY, response.accessToken);
    localStorage.setItem(USER_KEY, JSON.stringify(response.user));
    this._user.set(response.user);
  }

  private restoreUser(): UserDto | null {
    const token = localStorage.getItem(TOKEN_KEY);
    const user = localStorage.getItem(USER_KEY);
    if (!token || !user) return null;
    try {
      return JSON.parse(user) as UserDto;
    } catch {
      return null;
    }
  }
}
