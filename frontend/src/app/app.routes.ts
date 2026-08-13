import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register').then((m) => m.Register),
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./features/shell/shell').then((m) => m.Shell),
    children: [
      {
        path: '',
        loadComponent: () => import('./features/home/home').then((m) => m.Home),
      },
      {
        path: 'categories',
        loadComponent: () =>
          import('./features/categories/category-list').then((m) => m.CategoryList),
      },
      {
        path: 'categories/new',
        loadComponent: () =>
          import('./features/categories/category-form').then((m) => m.CategoryForm),
      },
      {
        path: 'categories/:id',
        loadComponent: () =>
          import('./features/categories/category-form').then((m) => m.CategoryForm),
      },
      {
        path: 'income-sources',
        loadComponent: () => import('./features/incomes/income-list').then((m) => m.IncomeList),
      },
      {
        path: 'income-sources/new',
        loadComponent: () => import('./features/incomes/income-form').then((m) => m.IncomeForm),
      },
      {
        path: 'income-sources/:id',
        loadComponent: () => import('./features/incomes/income-form').then((m) => m.IncomeForm),
      },
      {
        path: 'expense-sources',
        loadComponent: () => import('./features/expenses/expense-list').then((m) => m.ExpenseList),
      },
      {
        path: 'expense-sources/new',
        loadComponent: () => import('./features/expenses/expense-form').then((m) => m.ExpenseForm),
      },
      {
        path: 'expense-sources/:id',
        loadComponent: () => import('./features/expenses/expense-form').then((m) => m.ExpenseForm),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
