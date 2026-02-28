import { Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

interface RoleCard {
  key: string;
  label: string;
  description: string;
  accent: string;
}

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [RouterLink, CommonModule],
  styles: [`
    .landing-grid { display: grid; grid-template-columns: 1fr; }
    @media (min-width: 768px) {
      .landing-grid { grid-template-columns: minmax(0, 1.4fr) minmax(0, 1fr); }
    }
  `],
  template: `
    <section class="max-w-6xl mx-auto px-4 py-10 md:py-16">
      <div class="landing-grid gap-10 items-start">
        <div class="space-y-6">
          <p class="inline-flex items-center rounded-full border border-emerald-400/40 bg-emerald-500/10 px-3 py-1 text-xs font-medium text-emerald-700 dark:text-emerald-200 mb-2">
            End-to-end Property Insurance Management
          </p>
          <h1 class="text-3xl md:text-4xl lg:text-5xl font-semibold tracking-tight text-slate-900 dark:text-slate-50">
            Manage property policies, premiums, and claims
            <span class="block text-emerald-600 dark:text-emerald-300">with role-based control.</span>
          </h1>
          <p class="text-sm md:text-base text-slate-600 dark:text-slate-300 max-w-xl">
            A secure, workflow-driven Property Insurance Management System for
            Admins, Agents, Customers, and Claims Officers. Track policy requests,
            calculate premiums, and process claims with full auditability.
          </p>
          <div class="flex flex-wrap items-center gap-3 pt-2">
            <a
              routerLink="/login"
              class="inline-flex items-center justify-center rounded-full bg-emerald-500 px-5 py-2.5 text-sm font-medium text-slate-950 shadow-lg shadow-emerald-500/30 hover:bg-emerald-400 transition-colors"
            >
              Login to system
            </a>
            <button
              type="button"
              class="inline-flex items-center gap-2 rounded-full border border-slate-300 px-4 py-2 text-xs md:text-sm font-medium text-slate-700 hover:bg-slate-100 transition-colors dark:border-slate-700 dark:text-slate-200 dark:hover:bg-slate-900"
              (click)="scrollToRoles()"
            >
              View role capabilities
              <span class="h-5 w-5 rounded-full border border-slate-400 flex items-center justify-center text-xs dark:border-slate-500">
                ?
              </span>
            </button>
          </div>
          <div class="mt-6 grid gap-4 text-xs text-slate-600 sm:grid-cols-3 dark:text-slate-300">
            <div class="flex flex-col gap-1">
              <span class="font-semibold text-slate-900 dark:text-slate-100">Secure JWT Login</span>
              <span>Backed by ASP.NET Core authentication with role-based access.</span>
            </div>
            <div class="flex flex-col gap-1">
              <span class="font-semibold text-slate-900 dark:text-slate-100">Policy Workflow</span>
              <span>From customer request to admin approval and premium invoicing.</span>
            </div>
            <div class="flex flex-col gap-1">
              <span class="font-semibold text-slate-900 dark:text-slate-100">Claims Handling</span>
              <span>File claims, verify as Claims Officer, and auto-generate invoices.</span>
            </div>
          </div>
        </div>

        <div class="grid gap-4">
          <div class="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm dark:border-slate-800 dark:bg-slate-900/60">
            <img
              src="/images/placeholder-1.svg"
              alt="Property insurance overview"
              class="w-full rounded-xl border border-slate-200 dark:border-slate-800"
            />
            <p class="mt-3 text-[11px] text-slate-600 dark:text-slate-300">
              Centralized policy operations with a secure audit trail.
            </p>
          </div>
          <div class="grid grid-cols-2 gap-3">
            <img
              src="/images/placeholder-2.svg"
              alt="Claims processing preview"
              class="rounded-xl border border-slate-200 dark:border-slate-800"
            />
            <img
              src="/images/placeholder-3.svg"
              alt="Agent workflow preview"
              class="rounded-xl border border-slate-200 dark:border-slate-800"
            />
          </div>
        </div>
      </div>
    </section>

    <section id="about" class="max-w-6xl mx-auto px-4 py-12">
      <div class="grid gap-6 md:grid-cols-[1.2fr_1fr] items-start">
        <div>
          <h2 class="text-xl md:text-2xl font-semibold text-slate-900 dark:text-slate-50">
            About the platform
          </h2>
          <p class="mt-3 text-sm text-slate-600 dark:text-slate-300">
            The Property Insurance Management System streamlines end-to-end policy
            management for a multi-role environment. It enables policy creation,
            risk assessment, claims verification, and invoice tracking in one
            cohesive workflow.
          </p>
        </div>
        <div class="rounded-2xl border border-slate-200 bg-white p-4 text-[11px] text-slate-600 dark:border-slate-800 dark:bg-slate-900/60 dark:text-slate-300">
          <p class="font-semibold text-slate-900 dark:text-slate-100">Quote of the day</p>
          <p class="mt-2">
            “Insurance is the promise that tomorrow can still be better.”
          </p>
          <p class="mt-2 text-[10px] text-slate-500 dark:text-slate-400">
            — Property Insurance Team
          </p>
        </div>
      </div>
    </section>

    <section id="features" class="max-w-6xl mx-auto px-4 py-12">
      <h2 class="text-xl md:text-2xl font-semibold text-slate-900 dark:text-slate-50">
        Features
      </h2>
      <div class="mt-4 grid gap-3 md:grid-cols-3">
        <div class="rounded-xl border border-slate-200 bg-white p-4 text-[11px] text-slate-600 dark:border-slate-800 dark:bg-slate-900/60 dark:text-slate-300">
          <p class="font-semibold text-slate-900 dark:text-slate-100">Role-based access</p>
          <p class="mt-1">Granular access controls tailored for each stakeholder.</p>
        </div>
        <div class="rounded-xl border border-slate-200 bg-white p-4 text-[11px] text-slate-600 dark:border-slate-800 dark:bg-slate-900/60 dark:text-slate-300">
          <p class="font-semibold text-slate-900 dark:text-slate-100">Risk analytics</p>
          <p class="mt-1">Automated risk scoring with premium and commission insights.</p>
        </div>
        <div class="rounded-xl border border-slate-200 bg-white p-4 text-[11px] text-slate-600 dark:border-slate-800 dark:bg-slate-900/60 dark:text-slate-300">
          <p class="font-semibold text-slate-900 dark:text-slate-100">Claims workflow</p>
          <p class="mt-1">Streamlined approval and invoice generation after verification.</p>
        </div>
      </div>
    </section>

    <section id="location" class="max-w-6xl mx-auto px-4 py-12">
      <h2 class="text-xl md:text-2xl font-semibold text-slate-900 dark:text-slate-50">
        Location & outreach
      </h2>
      <p class="mt-3 text-sm text-slate-600 dark:text-slate-300">
        Location details and branch coverage will be added here. This section can
        include offices, contact channels, and service regions as needed.
      </p>
    </section>

    <section id="roles" class="max-w-6xl mx-auto px-4 py-12">
      <div
        id="role-section"
        class="rounded-2xl border border-slate-200 bg-white p-4 md:p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900/60"
      >
        <div class="flex items-center justify-between mb-3">
          <h2 class="text-sm font-semibold text-slate-900 dark:text-slate-100">
            Choose your role
          </h2>
          <span class="rounded-full bg-slate-100 px-2 py-0.5 text-xs text-slate-600 dark:bg-slate-800 dark:text-slate-300">
            4 roles available
          </span>
        </div>

        <div class="grid gap-3 md:grid-cols-2">
          <button
            type="button"
            *ngFor="let role of roles()"
            class="flex items-start gap-3 rounded-xl border border-slate-200 bg-white px-3 py-3 text-left hover:border-emerald-400/70 hover:bg-emerald-50 transition-colors dark:border-slate-800 dark:bg-slate-900/80 dark:hover:bg-slate-900"
            (click)="goToLogin(role.key)"
          >
            <div
              class="mt-1 h-8 w-8 rounded-xl flex items-center justify-center text-xs font-semibold text-slate-950"
              [ngClass]="role.accent"
            >
              {{ getInitials(role.label) }}
            </div>
            <div>
              <div class="flex items-center gap-2">
                <span class="text-xs font-semibold text-slate-900 dark:text-slate-50">
                  {{ role.label }}
                </span>
              </div>
              <p class="mt-1 text-xs text-slate-600 dark:text-slate-300">
                {{ role.description }}
              </p>
            </div>
          </button>
        </div>
      </div>
    </section>
  `,
})
export class LandingComponent {
  private readonly router = inject(Router);

  readonly roles = signal<RoleCard[]>([
    {
      key: 'Admin',
      label: 'Admin',
      description: 'Configure categories and plans, approve policy requests, and manage users.',
      accent: 'bg-gradient-to-tr from-sky-500 to-cyan-400',
    },
    {
      key: 'Agent',
      label: 'Agent',
      description: 'Assist customers, send property forms, calculate risk and premium installments.',
      accent: 'bg-gradient-to-tr from-indigo-500 to-violet-400',
    },
    {
      key: 'Customer',
      label: 'Customer',
      description: 'Browse plans, request policies, submit property details, and view invoices.',
      accent: 'bg-gradient-to-tr from-emerald-500 to-lime-400',
    },
    {
      key: 'ClaimsOfficer',
      label: 'Claims Officer',
      description: 'Review pending claims, verify incidents, and approve or reject payouts.',
      accent: 'bg-gradient-to-tr from-rose-500 to-orange-400',
    },
  ]);

  readonly roleCount = computed(() => this.roles().length);

  getInitials(label: string): string {
    return label
      .split(' ')
      .map((p) => p[0])
      .join('')
      .toUpperCase();
  }

  goToLogin(roleKey: string): void {
    this.router.navigate(['/login'], {
      queryParams: { role: roleKey },
    });
  }

  scrollToRoles(): void {
    const el = document.getElementById('role-section');
    if (el) {
      el.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }
}

