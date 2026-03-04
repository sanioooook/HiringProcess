import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  // Redirect root to the main app view
  { path: '', redirectTo: 'app', pathMatch: 'full' },

  // Auth pages (no guard)
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then(m => m.LoginComponent),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register.component').then(m => m.RegisterComponent),
  },

  // Protected: main application
  {
    path: 'app',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/hiring-processes/list/hiring-process-list.component')
        .then(m => m.HiringProcessListComponent),
  },

  // Protected: settings
  {
    path: 'settings',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/settings/settings.component').then(m => m.SettingsComponent),
  },

  // Catch-all fallback
  { path: '**', redirectTo: 'app' },
];
