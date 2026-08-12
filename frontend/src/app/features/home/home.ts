import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-home',
  template: `
    <section class="home">
      <h1>MyFinance</h1>
      @if (auth.currentUser(); as user) {
        <p>Olá, <strong>{{ user.name }}</strong> ({{ user.email }})</p>
      }
      <p class="home-hint">
        Fase 1 concluída — autenticação e isolamento por usuário. As telas financeiras
        (receitas, despesas, dívidas, dashboard) chegam nas próximas fases.
      </p>
      <button (click)="logout()">Sair</button>
    </section>
  `,
})
export class Home {
  readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  logout(): void {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }
}
