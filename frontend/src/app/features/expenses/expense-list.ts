import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { CategoryService } from '../../core/cadastros/category.service';
import { ExpenseSourceService } from '../../core/cadastros/expense-source.service';
import { apiErrorMessage } from '../../core/http/api-error';
import {
  EXPENSE_KIND_LABEL,
  ExpenseSourceResponse,
  RECURRENCE_TYPE_LABEL,
} from '../../core/models/cadastros.models';

@Component({
  selector: 'app-expense-list',
  imports: [RouterLink, CurrencyPipe, DatePipe],
  template: `
    <section class="page">
      <header class="page-head">
        <h1>Despesas fixas</h1>
        <a class="btn-primary" routerLink="/expense-sources/new">Nova despesa fixa</a>
      </header>
      <p class="page-hint">
        Despesas fixas são o que sai todo mês (aluguel, internet, escola). Dívidas parceladas não
        entram aqui — elas ganham tela própria na fase de dívidas.
      </p>

      @if (error(); as message) {
        <p class="alert-error" role="alert">{{ message }}</p>
      }

      @if (loading()) {
        <p class="state">Carregando…</p>
      } @else if (items().length === 0) {
        <p class="state">Nenhuma despesa fixa cadastrada ainda.</p>
      } @else {
        <div class="table-wrap">
          <table class="data">
            <thead>
              <tr>
                <th>Descrição</th>
                <th>Categoria</th>
                <th>Tipo</th>
                <th>Valor padrão</th>
                <th>Vencimento</th>
                <th>Recorrência</th>
                <th>Início</th>
                <th>Situação</th>
                <th class="actions">Ações</th>
              </tr>
            </thead>
            <tbody>
              @for (item of items(); track item.id) {
                <tr>
                  <td data-label="Descrição">{{ item.description }}</td>
                  <td data-label="Categoria">{{ categoryName(item.categoryId) }}</td>
                  <td data-label="Tipo">{{ kindLabel[item.expenseKind] }}</td>
                  <td data-label="Valor padrão">{{ item.defaultAmount | currency: 'BRL' }}</td>
                  <td data-label="Vencimento">{{ dueLabel(item) }}</td>
                  <td data-label="Recorrência">{{ recurrenceLabel[item.recurrenceType] }}</td>
                  <td data-label="Início">{{ item.startDate | date: 'dd/MM/yyyy' }}</td>
                  <td data-label="Situação">
                    <span class="badge" [class.is-off]="!item.isActive">
                      {{ item.isActive ? 'Ativa' : 'Inativa' }}
                    </span>
                  </td>
                  <td data-label="Ações" class="actions">
                    <a class="btn-ghost" [routerLink]="['/expense-sources', item.id]">Editar</a>
                    <button type="button" class="btn-danger" (click)="remove(item)">Excluir</button>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      }
    </section>
  `,
})
export class ExpenseList {
  private readonly service = inject(ExpenseSourceService);
  private readonly categories = inject(CategoryService);

  readonly items = signal<ExpenseSourceResponse[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly kindLabel = EXPENSE_KIND_LABEL;
  readonly recurrenceLabel = RECURRENCE_TYPE_LABEL;

  private readonly categoryNames = signal(new Map<string, string>());

  constructor() {
    this.load();
  }

  categoryName(id: string): string {
    return this.categoryNames().get(id) ?? '—';
  }

  dueLabel(item: ExpenseSourceResponse): string {
    if (item.dueDay === null) return 'sem data fixa';
    return item.dueMonthOffset === 1 ? `dia ${item.dueDay} do mês seguinte` : `dia ${item.dueDay}`;
  }

  remove(item: ExpenseSourceResponse): void {
    if (!confirm(`Excluir a despesa "${item.description}"?`)) return;
    this.error.set(null);
    this.service.delete(item.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(apiErrorMessage(err, 'Não foi possível excluir a despesa.')),
    });
  }

  private load(): void {
    this.loading.set(true);
    forkJoin({ sources: this.service.list(), categories: this.categories.list() }).subscribe({
      next: ({ sources, categories }) => {
        this.categoryNames.set(new Map(categories.map((c) => [c.id, c.name])));
        this.items.set(sources);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Não foi possível carregar as despesas.'));
        this.loading.set(false);
      },
    });
  }
}
