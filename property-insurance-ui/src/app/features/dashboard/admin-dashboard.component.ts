import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  PolicyRequestsService,
  PolicyRequest,
} from '../../core/policy-requests.service';
import { AdminService, AgentSummary } from '../../core/admin.service';
import { NotificationsService } from '../../core/notifications.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <section class="max-w-6xl mx-auto px-4 py-8 md:py-10">
      <div class="flex items-center justify-between gap-4 mb-6">
        <div>
          <h1 class="text-xl md:text-2xl font-semibold text-slate-900 dark:text-slate-50">
            Admin Dashboard
          </h1>
          <p class="text-xs md:text-sm text-slate-500 mt-1 dark:text-slate-400">
            Assign agents to policy requests and approve policies.
          </p>
        </div>
      </div>

      <div class="rounded-xl border border-slate-200 bg-white/80 p-4 mb-6 text-[11px] text-slate-600 space-y-1.5 dark:border-slate-800 dark:bg-slate-900/60 dark:text-slate-300">
        <p>
          Review newly created policy requests, assign agents, and finalize approvals
          once customers confirm their purchase.
        </p>
      </div>

      <div class="flex items-center justify-between mb-3 text-xs text-slate-700 dark:text-slate-200">
        <span class="font-semibold">Pending policy requests</span>
        <button
          type="button"
          class="rounded-full border border-slate-300 px-3 py-1 text-[11px] hover:bg-slate-100 dark:border-slate-700 dark:hover:bg-slate-900"
          (click)="loadPending()"
        >
          Refresh
        </button>
      </div>

      <div *ngIf="pendingLoading" class="text-[11px] text-slate-500 mb-3 dark:text-slate-400">
        Loading pending requests…
      </div>
      <div *ngIf="pendingError" class="text-[11px] text-rose-600 mb-3 dark:text-rose-300">
        {{ pendingError }}
      </div>

      <div
        *ngIf="!pendingLoading && pendingRequests.length === 0 && !pendingError"
        class="text-[11px] text-slate-500 mb-4 dark:text-slate-400"
      >
        No pending requests waiting for admin.
      </div>

      <div class="grid gap-3 md:grid-cols-2">
        <div
          *ngFor="let req of pendingRequests"
          class="rounded-xl border border-slate-200 bg-white/90 p-3 text-[11px] text-slate-700 space-y-1.5 dark:border-slate-800 dark:bg-slate-900/70 dark:text-slate-200"
        >
          <div class="flex items-center justify-between">
            <span class="font-semibold text-slate-900 dark:text-slate-50">
              Request #{{ req.id }}
            </span>
            <span class="rounded-full bg-slate-100 px-2 py-0.5 text-[10px] text-slate-600 dark:bg-slate-800 dark:text-slate-300">
              {{ req.status }}
            </span>
          </div>
          <div class="text-slate-600 dark:text-slate-300">
            Plan:
            <span class="font-medium">
              {{ req.plan?.planName || ('Plan ' + req.planId) }}
            </span>
          </div>
          <div class="text-slate-500 dark:text-slate-400">
            Customer ID: <span>{{ req.customerId }}</span>
          </div>
          <div class="text-slate-500 dark:text-slate-400">
            Agent ID:
            <span>{{ req.agentId || 'Not assigned' }}</span>
          </div>

          <div class="mt-2 flex items-center gap-2">
            <select
              class="w-44 rounded border border-slate-300 bg-white px-2 py-1 text-[11px] text-slate-700 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-50"
              [(ngModel)]="agentSelections[req.id]"
            >
              <option [ngValue]="0">Select agent</option>
              <option *ngFor="let agent of agents" [ngValue]="agent.id">
                {{ agent.fullName }}
              </option>
            </select>
            <button
              type="button"
              class="rounded bg-emerald-500 px-3 py-1 text-[11px] font-semibold text-slate-950 hover:bg-emerald-400"
              (click)="assignAgent(req.id)"
            >
              Assign agent
            </button>
          </div>

          <div class="mt-2 border-t border-slate-200 pt-2 flex items-center gap-2 dark:border-slate-800">
            <button
              type="button"
              class="rounded border border-slate-300 px-3 py-1 text-[11px] text-slate-700 hover:bg-slate-100 dark:border-slate-700 dark:text-slate-100 dark:hover:bg-slate-900"
              (click)="approveAfterCustomer(req.id)"
            >
              Final approve (after customer confirm)
            </button>
          </div>
        </div>
      </div>
    </section>
  `,
})
export class AdminDashboardComponent implements OnInit {
  private readonly policyRequests = inject(PolicyRequestsService);
  private readonly adminService = inject(AdminService);
  private readonly notifications = inject(NotificationsService);

  pendingRequests: PolicyRequest[] = [];
  pendingLoading = false;
  pendingError = '';
  agents: AgentSummary[] = [];
  agentSelections: Record<number, number> = {};

  ngOnInit(): void {
    this.loadPending();
    this.loadAgents();
  }

  loadAgents(): void {
    this.adminService.getAgents().subscribe({
      next: (agents) => {
        this.agents = agents;
      },
    });
  }

  loadPending(): void {
    this.pendingLoading = true;
    this.pendingError = '';
    this.policyRequests.getAdminPending().subscribe({
      next: (requests) => {
        this.pendingRequests = requests;
        this.pendingLoading = false;
      },
      error: (err) => {
        this.pendingError = this.extractError(err);
        this.pendingLoading = false;
      },
    });
  }

  assignAgent(requestId: number): void {
    const agentId = this.agentSelections[requestId] || 0;
    if (!agentId) {
      this.pendingError = 'Please select an agent before assigning.';
      return;
    }

    this.policyRequests.assignAgent(requestId, agentId).subscribe({
      next: () => {
        this.notifications.show({
          title: 'Agent assigned',
          detail: `Request #${requestId} assigned successfully.`,
          type: 'success',
        });
        this.loadPending();
      },
      error: (err) => {
        this.pendingError = this.extractError(err);
      },
    });
  }

  approveAfterCustomer(requestId: number): void {
    this.policyRequests.adminApprove(requestId).subscribe({
      next: () => {
        this.notifications.show({
          title: 'Policy approved',
          detail: `Request #${requestId} approved successfully.`,
          type: 'success',
        });
        this.loadPending();
      },
      error: (err) => {
        this.pendingError = this.extractError(err);
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
}

