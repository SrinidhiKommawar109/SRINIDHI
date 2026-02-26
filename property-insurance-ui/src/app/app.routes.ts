import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { AdminDashboard } from './features/dashboard/admin-dashboard/admin-dashboard';
import { CategoryList } from './features/category/category-list/category-list';
import { AgentDashboard } from './features/dashboard/agent-dashboard/agent-dashboard';
import { CustomerDashboard } from './features/dashboard/customer-dashboard/customer-dashboard';
import { ClaimsOfficerDashboard } from './features/dashboard/claims-officer-dashboard/claims-officer-dashboard';
import { role } from './core/guards/role';

export const routes: Routes = [
    { path: '', component: Login },

    {
        path: 'admin',
        component: AdminDashboard,
        children: [
            { path: 'categories', component: CategoryList }
        ]
    },
    {
    path: 'agent',
    component: AgentDashboard,
    canActivate: [role(['Agent'])]
  },

  {
    path: 'customer',
    component: CustomerDashboard,
    canActivate: [role(['Customer'])]
  },

  {
    path: 'claims',
    component: ClaimsOfficerDashboard,
    canActivate: [role(['ClaimsOfficer'])]
  }
];