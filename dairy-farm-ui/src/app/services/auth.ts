import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { API_URL } from './api';

const API = 'http://localhost:5298/api';   // <-- change to your API port

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(private http: HttpClient) {}

  login(username: string, password: string) {
    return this.http.post<any>(`${API}/auth/login`, { username, password }).pipe(
      tap(res => {
        localStorage.setItem('token', res.token);
        localStorage.setItem('role', res.role);
        localStorage.setItem('fullName', res.fullName);
      })
    );
  }

  logout() { localStorage.clear(); }
  get token() { return localStorage.getItem('token'); }
  get role() { return localStorage.getItem('role'); }
  get isLoggedIn() { return !!this.token; }
  get fullName() { return localStorage.getItem('fullName') ?? ''; }
  get isAdmin() { return this.role === 'Admin'; }
}