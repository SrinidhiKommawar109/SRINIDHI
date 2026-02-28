import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface AgentSummary {
  id: number;
  fullName: string;
  email: string;
}

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/Admin`;

  getAgents(): Observable<AgentSummary[]> {
    return this.http.get<AgentSummary[]>(this.baseUrl + '/agents');
  }
}

