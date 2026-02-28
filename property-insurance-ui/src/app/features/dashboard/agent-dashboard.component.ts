import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  PolicyRequestsService,
  CalculateRiskResponse,
} from '../../core/policy-requests.service';
import { NotificationsService } from '../../core/notifications.service';

@Component({
  selector: 'app-agent-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <section class="max-w-5xl mx-auto px-4 py-8 md:py-10">
      <div class="flex items-center justify-between gap-4 mb-6">
        <div>
          <h1 class="text-xl md:text-2xl font-semibold text-slate-900 dark:text-slate-50">
            Agent Dashboard
          </h1>
          <p class="text-xs md:text-sm text-slate-500 mt-1 dark:text-slate-400">
            Work on assigned policy requests, send property forms, and calculate premiums.
          </p>
        </div>
      </div>

      <div class="grid gap-4 md:grid-cols-2 mb-6">
        <div class="rounded-xl border border-slate-200 bg-white/90 p-4 text-[11px] text-slate-700 space-y-2 dark:border-slate-800 dark:bg-slate-900/60 dark:text-slate-200">
          <h2 class="text-xs font-semibold text-slate-900 dark:text-slate-200 mb-1">
            Send property form to customer
          </h2>
          <p class="text-slate-500 dark:text-slate-400">
            Notify the customer to fill property details once the request is assigned.
          </p>
          <div class="flex items-center gap-2 mt-2">
            <input
              #sendFormRequestId
              type="number"
              min="1"
              class="w-28 rounded border border-slate-300 bg-white px-2 py-1 text-[11px] text-slate-700 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-50"
              placeholder="Request ID"
            />
            <button
              type="button"
              class="rounded bg-emerald-500 px-3 py-1 text-[11px] font-semibold text-slate-950 hover:bg-emerald-400"
              (click)="sendForm(+sendFormRequestId.value)"
            >
              Send form
            </button>
          </div>
        </div>

        <div class="rounded-xl border border-slate-200 bg-white/90 p-4 text-[11px] text-slate-700 space-y-2 dark:border-slate-800 dark:bg-slate-900/60 dark:text-slate-200">
          <h2 class="text-xs font-semibold text-slate-900 dark:text-slate-200 mb-1">
            Calculate risk, premium & commission
          </h2>
          <p class="text-slate-500 dark:text-slate-400">
            Estimate premium and commission after receiving property details.
          </p>
          <div class="flex items-center gap-2 mt-2">
            <input
              #riskRequestId
              type="number"
              min="1"
              class="w-28 rounded border border-slate-300 bg-white px-2 py-1 text-[11px] text-slate-700 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-50"
              placeholder="Request ID"
            />
            <button
              type="button"
              class="rounded bg-sky-500 px-3 py-1 text-[11px] font-semibold text-slate-950 hover:bg-sky-400"
              (click)="calculateRisk(+riskRequestId.value)"
            >
              Calculate
            </button>
          </div>

          <div *ngIf="lastRisk" class="mt-3 border-t border-slate-200 pt-2 space-y-1 text-[11px] dark:border-slate-800">
            <div class="text-slate-600 dark:text-slate-300">
              Plan:
              <span class="font-medium">{{ lastRisk.planName }}</span>
            </div>
            <div class="text-slate-600 dark:text-slate-300">
              Risk score:
              <span class="font-medium">{{ lastRisk.riskScore }}</span>
            </div>
            <div class="text-slate-600 dark:text-slate-300">
              Total premium:
              <span class="font-medium">{{ lastRisk.totalPremium | number: '1.2-2' }}</span>
            </div>
            <div class="text-slate-600 dark:text-slate-300">
              Installments:
              <span class="font-medium">
                {{ lastRisk.installmentCount }} × {{ lastRisk.installmentAmount | number: '1.2-2' }}
              </span>
            </div>
            <div class="text-slate-600 dark:text-slate-300">
              Agent commission:
              <span class="font-medium">{{ lastRisk.agentCommissionAmount | number: '1.2-2' }}</span>
            </div>
          </div>
        </div>
      </div>

      <div class="rounded-xl border border-slate-200 bg-white/90 p-4 text-[11px] text-slate-700 dark:border-slate-800 dark:bg-slate-900/60 dark:text-slate-200">
        <div class="flex items-center justify-between">
          <span class="font-semibold">Commission earned (approved policies)</span>
          <span class="text-emerald-600 dark:text-emerald-300">
            {{ totalCommission | number: '1.2-2' }}
          </span>
        </div>
        <div class="flex items-center gap-2 mt-2">
          <button
            type="button"
            class="rounded-full border border-slate-300 px-2 py-1 text-[10px] hover:bg-slate-100 dark:border-slate-700 dark:hover:bg-slate-900"
            (click)="loadApprovedCommission()"
          >
            Refresh
          </button>
          <span class="text-[10px] text-slate-500 dark:text-slate-400">
            Commission updates after admin approval.
          </span>
        </div>
      </div>

      <div *ngIf="errorMessage" class="mt-3 rounded-xl border border-rose-500/60 bg-rose-500/10 p-3 text-[11px] text-rose-100">
        {{ errorMessage }}
      </div>
    </section>
  `,
})
export class AgentDashboardComponent implements OnInit {
  private readonly policies = inject(PolicyRequestsService);
  private readonly notifications = inject(NotificationsService);

  lastRisk: CalculateRiskResponse | null = null;
  errorMessage = '';
  totalCommission = 0;

  ngOnInit(): void {
    this.loadApprovedCommission();
  }

  sendForm(requestId: number): void {
    if (!requestId) {
      return;
    }
    this.errorMessage = '';
    this.policies.sendForm(requestId).subscribe({
      next: () => {
        this.notifications.show({
          title: 'Form sent',
          detail: `Request #${requestId} sent to customer.`,
          type: 'success',
        });
      },
      error: (err) => {
        this.errorMessage = this.extractError(err);
      },
    });
  }

  calculateRisk(requestId: number): void {
    if (!requestId) {
      return;
    }
    this.errorMessage = '';
    this.policies.calculateRisk(requestId).subscribe({
      next: (res) => {
        this.lastRisk = res;
        this.notifications.show({
          title: 'Risk calculated',
          detail: 'Premium and commission calculated.',
          type: 'success',
        });
      },
      error: (err) => {
        this.errorMessage = this.extractError(err);
      },
    });
  }

  private extractError(err: any): string {
    if (err?.error && typeof err.error === 'string') {
      return err.error;
    }
    if (err?.error?.title) {
      return err.error.title;
    }
    return 'Something went wrong while processing the request.';
  }

  loadApprovedCommission(): void {
    this.policies.getAgentApproved().subscribe({
      next: (requests) => {
        this.totalCommission = requests.reduce(
          (sum, req) => sum + (req.agentCommissionAmount || 0),
          0,
        );
      },
      error: (err) => {
        this.errorMessage = this.extractError(err);
      },
    });
  }
}

