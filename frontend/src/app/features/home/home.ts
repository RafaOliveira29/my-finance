import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-home',
  imports: [RouterLink],
  template: `
    <section class="page">
      <header class="page-head">
        <h1>Início</h1>
      </header>
      @if (auth.currentUser(); as user) {
        <p>
          Olá, <strong>{{ user.name }}</strong> ({{ user.email }})
        </p>
      }
      <p class="page-hint">
        Comece cadastrando suas <strong>categorias</strong> e, em seguida, suas
        <strong>receitas</strong> e <strong>despesas fixas</strong>. Nas próximas fases eles viram
        a competência mensal, o dashboard e o painel de dívidas.
      </p>

      <div class="card-grid">
        <a class="nav-card" routerLink="/categories">
          <h2>Categorias</h2>
          <p>Organize receitas e despesas por assunto (moradia, salário, lazer…).</p>
        </a>
        <a class="nav-card" routerLink="/income-sources">
          <h2>Receitas</h2>
          <p>Salário, aluguel recebido, freelas — o que entra todo mês.</p>
        </a>
        <a class="nav-card" routerLink="/expense-sources">
          <h2>Despesas fixas</h2>
          <p>Aluguel, internet, escola — o que sai todo mês.</p>
        </a>
      </div>
    </section>
  `,
})
export class Home {
  readonly auth = inject(AuthService);
}
