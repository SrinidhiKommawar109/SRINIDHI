import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private baseUrl = 'https://localhost:7078/api/Auth';

  constructor(private http: HttpClient) { }

  login(email: string, password: string) {
    return this.http.post(`${this.baseUrl}/login`, { email, password });
  }
  register(email: string, password: string, role: string) {

    const payload = {
      email: email,
      password: password,
      role: role
    };

    return this.http.post('https://localhost:7078/api/Auth/register', payload);
  }
}