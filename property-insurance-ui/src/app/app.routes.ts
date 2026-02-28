import { Routes } from '@angular/router';
import { LandingComponent } from './features/landing/landing.component';
import { LoginComponent } from './features/auth/login.component';
import { AdminDashboardComponent } from './features/dashboard/admin-dashboard.component';
import { AgentDashboardComponent } from './features/dashboard/agent-dashboard.component';
import { CustomerDashboardComponent } from './features/dashboard/customer-dashboard.component';
import { ClaimsOfficerDashboardComponent } from './features/dashboard/claims-officer-dashboard.component';
import { authGuard } from './core/auth.guard';

export const routes: Routes = [
  {
    path: '',
    component: LandingComponent,
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'admin',
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
    component: AdminDashboardComponent,
  },
  {
    path: 'agent',
    canActivate: [authGuard],
    data: { roles: ['Agent'] },
    component: AgentDashboardComponent,
  },
  {
    path: 'customer',
    canActivate: [authGuard],
    data: { roles: ['Customer'] },
    component: CustomerDashboardComponent,
  },
  {
    path: 'claims-officer',
    canActivate: [authGuard],
    data: { roles: ['ClaimsOfficer'] },
    component: ClaimsOfficerDashboardComponent,
  },
  {
    path: '**',
    redirectTo: '',
  },
];
