import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Invoice {
  id: number;
  policyRequestId: number;
  invoiceNumber: string;
  generatedDate: string;
  totalPremium: number;
  installmentAmount: number;
  installmentCount: number;
  claimAmount: number;
  planName: string;
}

@Injectable({
  providedIn: 'root',
})
export class InvoicesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/Invoices`;

  getMyInvoices(): Observable<Invoice[]> {
    return this.http.get<Invoice[]>(this.baseUrl + '/customer/invoices');
  }
}

