import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService, UserRole } from '../../core/auth.service';

type UiRole = UserRole;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <section class="min-h-[calc(100vh-4rem)] flex items-center justify-center px-4 py-10">
      <div class="w-full max-w-md rounded-2xl border border-slate-200 bg-white/90 p-6 shadow-2xl shadow-slate-200/40 dark:border-slate-800 dark:bg-slate-900/70 dark:shadow-slate-950/50">
        <h1 class="text-xl font-semibold text-slate-900 dark:text-slate-50 mb-1">
          Sign in
        </h1>
        <p class="text-xs text-slate-500 dark:text-slate-400 mb-5">
          Choose your role and enter your registered email and password. Access is controlled via JWT tokens issued by the backend.
        </p>

        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="space-y-4">
          <div>
            <label class="block text-xs font-medium text-slate-700 dark:text-slate-200 mb-1">
              Role
            </label>
            <div class="grid grid-cols-2 gap-2 text-[11px]">
              <button
                type="button"
                *ngFor="let role of roles"
                class="rounded-xl border px-2.5 py-2 text-left transition-colors"
                [ngClass]="
                  selectedRole === role.key
                    ? 'border-emerald-400/80 bg-emerald-500/10 text-emerald-700 dark:text-emerald-100'
                    : 'border-slate-300 bg-white text-slate-700 hover:border-slate-400 dark:border-slate-700 dark:bg-slate-900/80 dark:text-slate-200 dark:hover:border-slate-500'
                "
                (click)="selectRole(role.key)"
              >
                <div class="font-semibold">{{ role.label }}</div>
                <div class="mt-0.5 text-[10px] text-slate-500 dark:text-slate-300">
                  {{ role.short }}
                </div>
              </button>
            </div>
            <p
              *ngIf="formSubmitted && !selectedRole"
              class="mt-1 text-[11px] text-rose-600 dark:text-rose-300"
            >
              Please select a role.
            </p>
          </div>

          <div class="space-y-1">
            <label
              for="email"
              class="block text-xs font-medium text-slate-700 dark:text-slate-200"
            >
              Email
            </label>
            <input
              id="email"
              type="email"
              formControlName="email"
              class="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-700 placeholder:text-slate-400 focus:border-emerald-400 focus:outline-none focus:ring-1 focus:ring-emerald-400 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-50 dark:placeholder:text-slate-500"
              placeholder="you@example.com"
            />
            <p
              *ngIf="formSubmitted && form.controls['email'].invalid"
              class="text-[11px] text-rose-600 dark:text-rose-300"
            >
              Enter a valid email address.
            </p>
          </div>

          <div class="space-y-1">
            <label
              for="password"
              class="block text-xs font-medium text-slate-700 dark:text-slate-200"
            >
              Password
            </label>
            <input
              id="password"
              type="password"
              formControlName="password"
              class="w-full rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm text-slate-700 placeholder:text-slate-400 focus:border-emerald-400 focus:outline-none focus:ring-1 focus:ring-emerald-400 dark:border-slate-700 dark:bg-slate-900 dark:text-slate-50 dark:placeholder:text-slate-500"
              placeholder="••••••••"
            />
            <p
              *ngIf="formSubmitted && form.controls['password'].invalid"
              class="text-[11px] text-rose-600 dark:text-rose-300"
            >
              Password is required.
            </p>
          </div>

          <div *ngIf="errorMessage" class="rounded-lg border border-rose-500/60 bg-rose-500/10 px-3 py-2 text-[11px] text-rose-600 dark:text-rose-100">
            {{ errorMessage }}
          </div>

          <button
            type="submit"
            class="inline-flex w-full items-center justify-center rounded-lg bg-emerald-500 px-4 py-2.5 text-sm font-semibold text-slate-950 shadow-lg shadow-emerald-500/40 hover:bg-emerald-400 disabled:cursor-not-allowed disabled:opacity-60"
            [disabled]="loading"
          >
            <span *ngIf="!loading">Sign in</span>
            <span *ngIf="loading">Signing in…</span>
          </button>
        </form>

        <p class="mt-4 text-[11px] text-slate-500 dark:text-slate-400 leading-relaxed">
          This login authenticates your role-based access to the Property Insurance
          Management System. Contact your administrator if you need a new account.
        </p>

        <div class="mt-3 text-[11px] text-slate-500 dark:text-slate-400 flex justify-between">
          <a routerLink="/" class="hover:text-slate-700 dark:hover:text-slate-200">← Back to landing</a>
          <span>Secure token-based session</span>
        </div>
      </div>
    </section>
  `,
})
export class LoginComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  form: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  roles: { key: UiRole; label: string; short: string }[] = [
    {
      key: 'Admin',
      label: 'Admin',
      short: 'User and policy administration',
    },
    {
      key: 'Agent',
      label: 'Agent',
      short: 'Customer onboarding and risk calculation',
    },
    {
      key: 'Customer',
      label: 'Customer',
      short: 'Buy policies and view invoices',
    },
    {
      key: 'ClaimsOfficer',
      label: 'Claims Officer',
      short: 'Verify and approve/reject claims',
    },
  ];

  selectedRole: UiRole | '' = '';
  loading = false;
  formSubmitted = false;
  errorMessage = '';

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params) => {
      const role = params.get('role') as UiRole | null;
      if (role && this.roles.some((r) => r.key === role)) {
        this.selectedRole = role;
      }
    });
  }

  selectRole(role: UiRole): void {
    this.selectedRole = role;
    this.errorMessage = '';
  }

  onSubmit(): void {
    this.formSubmitted = true;
    this.errorMessage = '';

    if (!this.selectedRole || this.form.invalid) {
      return;
    }

    this.loading = true;
    const { email, password } = this.form.value;

    this.auth
      .login({
        email,
        password,
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          const backendRole = (res.role ?? '').toString();

          if (backendRole && backendRole !== this.selectedRole) {
            this.errorMessage =
              'Selected role does not match the role assigned to this account.';
            return;
          }

          this.redirectByRole(backendRole as UiRole);
        },
        error: (err) => {
          this.loading = false;
          if (err?.error && typeof err.error === 'string') {
            this.errorMessage = err.error;
          } else if (err?.error?.title) {
            this.errorMessage = err.error.title;
          } else {
            this.errorMessage = 'Login failed. Please check your credentials.';
          }
        },
      });
  }

  private redirectByRole(role: UiRole): void {
    switch (role) {
      case 'Admin':
        this.router.navigate(['/admin']);
        break;
      case 'Agent':
        this.router.navigate(['/agent']);
        break;
      case 'Customer':
        this.router.navigate(['/customer']);
        break;
      case 'ClaimsOfficer':
        this.router.navigate(['/claims-officer']);
        break;
      default:
        this.router.navigate(['/']);
    }
  }
}

