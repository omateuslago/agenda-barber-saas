import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login').then(m => m.Login)
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register').then(m => m.Register)
  },
  {
    path: '',
    loadComponent: () =>
      import('./layout/main-layout/main-layout').then(m => m.MainLayout),
    children: [
      {
        path: 'barber-shop',
        loadComponent: () =>
          import('./features/barber-shop/barber-shop').then(m => m.BarberShop)
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/dashboard/dashboard').then(m => m.Dashboard)
      },
      {
        path: 'services',
        loadComponent: () =>
          import('./features/services/services').then(m => m.Services)
      },
      {
        path: 'barbers',
        loadComponent: () =>
          import('./features/barbers/barbers').then(m => m.Barbers)
      },
      {
        path: 'appointments',
        loadComponent: () =>
          import('./features/appointments/appointments').then(m => m.Appointments)
      }
    ]
  }
];