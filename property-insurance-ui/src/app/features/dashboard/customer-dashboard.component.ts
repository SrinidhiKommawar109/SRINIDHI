import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  PolicyRequestsService,
  PolicyRequest,
  PropertyPlan,
  SubmitPropertyPayload,
} from '../../core/policy-requests.service';
import { ClaimsService, CreateClaimPayload } from '../../core/claims.service';
import { InvoicesService, Invoice } from '../../core/invoices.service';
import { NotificationsService } from '../../core/notifications.service';

@Component({
  selector: 'app-customer-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <section class="max-w-5xl mx-auto px-4 py-8 md:py-10">
      <div class="flex items-center justify-between gap-4 mb-6">
        <div>
          <h1 class="text-xl md:text-2xl font-semibold text-slate-900 dark:text-slate-50">
            Customer Dashboard
          </h1>
          <p class="text-xs md:text-sm text-slate-500 mt-1 dark:text-slate-400">
            Browse property plans, create policy requests, submit property details, and view invoices.
          </p>
        </div>
      </div>

      <div class="grid gap-4 md:grid-cols-2 mb-6">
        <div class="rounded-xl border border-slate-200 bg-white/90 p-4 text-[11px] text-slate-700 space-y-2 dark:border-slate-800 dark:bg-slate-900/60 dark:text-slate-200">
          <h2 class="text-xs font-semibold text-slate-900 dark:text-slate-200 mb-1">
            Create policy request
          </h2>
          <p class="text-slate-500 dark:text-slate-400">
            Choose a plan to start your policy request.
          </p>
          <div class="grid gap-2 sm:grid-cols-2">
            <div
              *ngFor="let plan of plans"
              class="rounded-lg border border-slate-200 bg-white px-3 py-2 text-[11px] text-slate-700 dark:border-slate-800 dark:bg-slate-900 dark:text-slate-200"
            >
              <div class="font-semibold text-slate-900 dark:text-slate-100">
                {{ plan.planName }}
              </div>
              <div class="text-slate-500 dark:text-slate-400">
                Base premium: {{ plan.basePremium | number: '1.2-2' }}
              </div>
              <div class="text-slate-500 dark:text-slate-400">
                Commission rate: {{ plan.agentCommission | number: '1.2-2' }}
              </div>
              <button
                type="button"
                class="mt-2 rounded bg-emerald-500 px-3 py-1 text-[11px] font-semibold text-slate-950 hover:bg-emerald-400"
                (click)="createRequestForPlan(plan.id)"
              >
                Request this plan
              </button>
            </div>
          </div>
          <div *ngIf="createMessage" class="text-[11px] text-emerald-600 dark:text-emerald-200">
            {{ createMessage }}
          </div>
        </div>

        <div class="rounded-xl border border-slate-200 bg-white/90 p-4 text-[11px] text-slate-700 space-y-2 dark:border-slate-800 dark:bg-slate-900/60 dark:text-slate-200">
          <h2 class="text-xs font-semibold text-slate-900 dark:text-slate-200 mb-1">
            Submit property details
          </h2>
          <input
            type="number"
            min="1"
            class="w-full rounded border border-slate-300 bg-white px-2 py-1 text-[11px] text-slate-700 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-50"
            placeholder="Policy request ID"
            [(ngModel)]="submitRequestId"
          />
          <input
            type="text"
            class="w-full rounded border border-slate-300 bg-white px-2 py-1 text-[11px] text-slate-700 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-50"
            placeholder="Property address"
            [(ngModel)]="submitPayload.propertyAddress"
          />
          <input
            type="number"
            class="w-full rounded border border-slate-300 bg-white px-2 py-1 text-[11px] text-slate-700 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-50"
            placeholder="Property value"
            [(ngModel)]="submitPayload.propertyValue"
          />
          <input
            type="number"
            class="w-full rounded border border-slate-300 bg-white px-2 py-1 text-[11px] text-slate-700 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-50"
            placeholder="Property age (years)"
            [(ngModel)]="submitPayload.propertyAge"
          />
          <button
            type="button"
            class="rounded bg-sky-500 px-3 py-1 text-[11px] font-semibold text-slate-950 hover:bg-sky-400"
            (click)="submitProperty()"
          >
            Submit details
          </button>
          <div *ngIf="submitMessage" class="text-[11px] text-emerald-600 dark:text-emerald-200">
            {{ submitMessage }}
          </div>
        </div>
      </div>

      <div class="grid gap-4 md:grid-cols-2 mb-6">
        <div class="rounded-xl border border-slate-200 bg-white/90 p-4 text-[11px] text-slate-700 space-y-2 dark:border-slate-800 dark:bg-slate-900/60 dark:text-slate-200">
          <div class="flex items-center justify-between">
            <h2 class="text-xs font-semibold text-slate-900 dark:text-slate-200">My policy requests</h2>
            <button
              type="button"
              class="rounded-full border border-slate-300 px-2 py-1 text-[10px] hover:bg-slate-100 dark:border-slate-700 dark:hover:bg-slate-900"
              (click)="loadMyRequests()"
            >
              Refresh
            </button>
          </div>
          <div *ngIf="requestsLoading" class="text-[11px] text-slate-500 dark:text-slate-400">
            Loading requests…
          </div>
          <div *ngIf="!requestsLoading && myRequests.length === 0" class="text-[11px] text-slate-500 dark:text-slate-400">
            No requests yet.
          </div>
          <div *ngFor="let req of myRequests" class="border-t border-slate-200 pt-2 dark:border-slate-800">
            <div class="flex items-center justify-between">
              <span class="text-slate-900 dark:text-slate-50 font-semibold">#{{ req.id }}</span>
              <span class="text-[10px] text-slate-500 dark:text-slate-300">{{ req.status }}</span>
            </div>
            <div class="text-slate-500 dark:text-slate-400">
              Plan: {{ req.plan?.planName || ('Plan ' + req.planId) }}
            </div>
          </div>
        </div>

        <div class="rounded-xl border border-slate-200 bg-white/90 p-4 text-[11px] text-slate-700 space-y-2 dark:border-slate-800 dark:bg-slate-900/60 dark:text-slate-200">
          <h2 class="text-xs font-semibold text-slate-900 dark:text-slate-200 mb-1">
            Confirm purchase
          </h2>
          <input
            type="number"
            min="1"
            class="w-full rounded border border-slate-300 bg-white px-2 py-1 text-[11px] text-slate-700 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-50"
            placeholder="Policy request ID"
            [(ngModel)]="buyRequestId"
          />
          <button
            type="button"
            class="rounded bg-emerald-500 px-3 py-1 text-[11px] font-semibold text-slate-950 hover:bg-emerald-400"
            (click)="confirmPurchase()"
          >
            Confirm purchase
          </button>
          <div *ngIf="buyMessage" class="text-[11px] text-emerald-600 dark:text-emerald-200">
            {{ buyMessage }}
          </div>
        </div>
      </div>

      <div class="grid gap-4 md:grid-cols-2">
        <div class="rounded-xl border border-slate-200 bg-white/90 p-4 text-[11px] text-slate-700 space-y-2 dark:border-slate-800 dark:bg-slate-900/60 dark:text-slate-200">
          <h2 class="text-xs font-semibold text-slate-900 dark:text-slate-200 mb-1">
            File a claim
          </h2>
          <input
            type="number"
            min="1"
            class="w-full rounded border border-slate-300 bg-white px-2 py-1 text-[11px] text-slate-700 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-50"
            placeholder="Policy request ID"
            [(ngModel)]="claimPayload.policyRequestId"
          />
          <input
            type="text"
            class="w-full rounded border border-slate-300 bg-white px-2 py-1 text-[11px] text-slate-700 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-50"
            placeholder="Property address"
            [(ngModel)]="claimPayload.propertyAddress"
          />
          <input
            type="number"
            class="w-full rounded border border-slate-300 bg-white px-2 py-1 text-[11px] text-slate-700 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-50"
            placeholder="Property value"
            [(ngModel)]="claimPayload.propertyValue"
          />
          <input
            type="number"
            class="w-full rounded border border-slate-300 bg-white px-2 py-1 text-[11px] text-slate-700 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-50"
            placeholder="Property age"
            [(ngModel)]="claimPayload.propertyAge"
          />
          <button
            type="button"
            class="rounded bg-rose-500 px-3 py-1 text-[11px] font-semibold text-slate-950 hover:bg-rose-400"
            (click)="fileClaim()"
          >
            Submit claim
          </button>
          <div *ngIf="claimMessage" class="text-[11px] text-emerald-600 dark:text-emerald-200">
            {{ claimMessage }}
          </div>
        </div>

        <div class="rounded-xl border border-slate-200 bg-white/90 p-4 text-[11px] text-slate-700 space-y-2 dark:border-slate-800 dark:bg-slate-900/60 dark:text-slate-200">
          <div class="flex items-center justify-between">
            <h2 class="text-xs font-semibold text-slate-900 dark:text-slate-200">Invoices</h2>
            <button
              type="button"
              class="rounded-full border border-slate-300 px-2 py-1 text-[10px] hover:bg-slate-100 dark:border-slate-700 dark:hover:bg-slate-900"
              (click)="loadInvoices()"
            >
              Refresh
            </button>
          </div>
          <div *ngIf="invoicesLoading" class="text-[11px] text-slate-500 dark:text-slate-400">
            Loading invoices…
          </div>
          <div *ngIf="!invoicesLoading && invoices.length === 0" class="text-[11px] text-slate-500 dark:text-slate-400">
            No invoices yet. Claims pending approval will appear here.
          </div>
          <div *ngFor="let invoice of invoices" class="border-t border-slate-200 pt-2 dark:border-slate-800">
            <div class="flex items-center justify-between">
              <span class="text-slate-900 dark:text-slate-50 font-semibold">{{ invoice.invoiceNumber }}</span>
              <span class="text-[10px] text-slate-500 dark:text-slate-300">{{ invoice.planName }}</span>
            </div>
            <div class="text-slate-500 dark:text-slate-400">
              Total premium: {{ invoice.totalPremium | number: '1.2-2' }}
            </div>
            <div class="text-slate-500 dark:text-slate-400">
              Installment: {{ invoice.installmentAmount | number: '1.2-2' }} × {{ invoice.installmentCount }}
            </div>
          </div>
        </div>
      </div>
    </section>
  `,
})
export class CustomerDashboardComponent implements OnInit {
  private readonly policies = inject(PolicyRequestsService);
  private readonly claims = inject(ClaimsService);
  private readonly invoicesService = inject(InvoicesService);
  private readonly notifications = inject(NotificationsService);

  plans: PropertyPlan[] = [];
  createMessage = '';

  myRequests: PolicyRequest[] = [];
  requestsLoading = false;

  submitRequestId = 0;
  submitPayload: SubmitPropertyPayload = {
    propertyAddress: '',
    propertyValue: 0,
    propertyAge: 0,
  };
  submitMessage = '';

  buyRequestId = 0;
  buyMessage = '';

  claimPayload: CreateClaimPayload = {
    policyRequestId: 0,
    propertyAddress: '',
    propertyValue: 0,
    propertyAge: 0,
  };
  claimMessage = '';

  invoices: Invoice[] = [];
  invoicesLoading = false;

  ngOnInit(): void {
    this.loadPlans();
    this.loadMyRequests();
    this.loadInvoices();
  }

  loadPlans(): void {
    this.policies.getAllPlans().subscribe({
      next: (plans) => {
        this.plans = plans;
      },
    });
  }

  createRequestForPlan(planId: number): void {
    if (!planId) {
      return;
    }
    this.createMessage = '';
    this.policies.createRequest(planId).subscribe({
      next: (msg) => {
        this.createMessage = msg;
        this.notifications.show({
          title: 'Request created',
          detail: 'Your policy request has been submitted.',
          type: 'success',
        });
        this.loadMyRequests();
      },
      error: (err) => {
        this.createMessage = this.extractError(err);
      },
    });
  }

  loadMyRequests(): void {
    this.requestsLoading = true;
    this.policies.getMyRequests().subscribe({
      next: (requests) => {
        this.myRequests = requests;
        this.requestsLoading = false;
      },
      error: () => {
        this.requestsLoading = false;
      },
    });
  }

  submitProperty(): void {
    if (!this.submitRequestId) {
      return;
    }
    this.submitMessage = '';
    this.policies
      .submitProperty(this.submitRequestId, this.submitPayload)
      .subscribe({
        next: (msg) => {
          this.submitMessage = msg;
          this.notifications.show({
            title: 'Details submitted',
            detail: 'Property details sent to the assigned agent.',
            type: 'success',
          });
          this.loadMyRequests();
        },
        error: (err) => {
          this.submitMessage = this.extractError(err);
        },
      });
  }

  confirmPurchase(): void {
    if (!this.buyRequestId) {
      return;
    }
    this.buyMessage = '';
    this.policies.buyPolicy(this.buyRequestId).subscribe({
      next: (msg) => {
        this.buyMessage = msg;
        this.notifications.show({
          title: 'Purchase confirmed',
          detail: 'Waiting for admin approval.',
          type: 'info',
        });
        this.loadMyRequests();
      },
      error: (err) => {
        this.buyMessage = this.extractError(err);
      },
    });
  }

  fileClaim(): void {
    if (!this.claimPayload.policyRequestId) {
      return;
    }
    this.claimMessage = '';
    this.claims.createClaim(this.claimPayload).subscribe({
      next: (msg) => {
        this.claimMessage = msg;
        this.notifications.show({
          title: 'Claim submitted',
          detail: 'Your claim is now pending review.',
          type: 'success',
        });
        this.loadInvoices();
      },
      error: (err) => {
        this.claimMessage = this.extractError(err);
      },
    });
  }

  loadInvoices(): void {
    this.invoicesLoading = true;
    this.invoicesService.getMyInvoices().subscribe({
      next: (invoices) => {
        this.invoices = invoices;
        this.invoicesLoading = false;
      },
      error: () => {
        this.invoicesLoading = false;
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

