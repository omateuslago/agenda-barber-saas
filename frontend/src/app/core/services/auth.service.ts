import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

interface LoginRequest {
  email: string;
  password: string;
}

interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}

interface AuthResponse {
  token: string;
  fullName: string;
  email: string;
}

interface UserMeResponse {
  id: number;
  fullName: string;
  email: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly tokenKey = 'barber_saas_token';

  constructor(private http: HttpClient) {}

  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/Auth/login`, data)
      .pipe(
        tap(response => {
          localStorage.setItem(this.tokenKey, response.token);
        })
      );
  }

  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/Auth/register`, data)
      .pipe(
        tap(response => {
          localStorage.setItem(this.tokenKey, response.token);
        })
      );
  }

  me(): Observable<UserMeResponse> {
    return this.http.get<UserMeResponse>(`${environment.apiUrl}/Auth/me`);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
  }
}