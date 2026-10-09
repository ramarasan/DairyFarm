import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

// >>> Change the port to the one shown by "dotnet run" (Now listening on: http://localhost:XXXX)
export const API_URL = 'http://localhost:5298/api';

export const errMsg = (e: any): string => e?.error?.message ?? 'Something went wrong';

export const today = (): string => {
  const d = new Date();
  return new Date(d.getTime() - d.getTimezoneOffset() * 60000).toISOString().slice(0, 10);
};

@Injectable({ providedIn: 'root' })
export class Api {
  private http = inject(HttpClient);

  get<T = any>(path: string, params?: Record<string, any>) {
    let p = new HttpParams();
    if (params) {
      for (const k of Object.keys(params)) {
        const v = params[k];
        if (v !== null && v !== undefined && v !== '') p = p.set(k, v);
      }
    }
    return this.http.get<T>(`${API_URL}/${path}`, { params: p });
  }
  post<T = any>(path: string, body: any) { return this.http.post<T>(`${API_URL}/${path}`, body); }
  put<T = any>(path: string, body: any = {}) { return this.http.put<T>(`${API_URL}/${path}`, body); }
  delete<T = any>(path: string) { return this.http.delete<T>(`${API_URL}/${path}`); }
}
