import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

interface NavItem {
  path: string;
  label: string;
  exact: boolean;
}

/** Moldura da área logada: cabeçalho, menu de navegação e conteúdo da rota ativa. */
@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="shell">
      <header class="shell-top">
        <a class="shell-brand" routerLink="/">MyFinance</a>
        <button
          type="button"
          class="shell-menu-toggle"
          aria-controls="nav-principal"
          [attr.aria-expanded]="menuOpen()"
          (click)="menuOpen.set(!menuOpen())"
        >
          <span aria-hidden="true">☰</span>
          <span class="sr-only">Abrir menu de navegação</span>
        </button>
        <div class="shell-user">
          @if (auth.currentUser(); as user) {
            <span class="shell-user-name">{{ user.name }}</span>
          }
          <button type="button" class="btn-ghost" (click)="logout()">Sair</button>
        </div>
      </header>

      <nav id="nav-principal" class="shell-nav" [class.is-open]="menuOpen()">
        @for (item of navItems; track item.path) {
          <a
            [routerLink]="item.path"
            routerLinkActive="is-active"
            [routerLinkActiveOptions]="{ exact: item.exact }"
            (click)="menuOpen.set(false)"
          >
            {{ item.label }}
          </a>
        }
      </nav>

      <main class="shell-main">
        <router-outlet />
      </main>
    </div>
  `,
})
export class Shell {
  readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly menuOpen = signal(false);

  readonly navItems: readonly NavItem[] = [
    { path: '/', label: 'Início', exact: true },
    { path: '/categories', label: 'Categorias', exact: false },
    { path: '/income-sources', label: 'Receitas', exact: false },
    { path: '/expense-sources', label: 'Despesas fixas', exact: false },
  ];

  logout(): void {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }
}
