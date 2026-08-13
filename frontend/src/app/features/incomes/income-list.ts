import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { CategoryService } from '../../core/cadastros/category.service';
import { IncomeSourceService } from '../../core/cadastros/income-source.service';
import { apiErrorMessage } from '../../core/http/api-error';
import { IncomeSourceResponse, RECURRENCE_TYPE_LABEL } from '../../core/models/cadastros.models';

@Component({
  selector: 'app-income-list',
  imports: [RouterLink, CurrencyPipe, DatePipe],
  template: `
    <section class="page">
      <header class="page-head">
        <h1>Receitas</h1>
        <a class="btn-primary" routerLink="/income-sources/new">Nova receita</a>
      </header>
      <p class="page-hint">
        Fontes de receita são o que entra todo mês (salário, aluguel recebido, freelas). O valor
        aqui é o padrão — nas próximas fases cada mês pode ajustar o valor realizado.
      </p>

      @if (error(); as message) {
        <p class="alert-error" role="alert">{{ message }}</p>
      }

      @if (loading()) {
        <p class="state">Carregando…</p>
      } @else if (items().length === 0) {
        <p class="state">Nenhuma fonte de receita cadastrada ainda.</p>
      } @else {
        <div class="table-wrap">
          <table class="data">
            <thead>
              <tr>
                <th>Descrição</th>
                <th>Categoria</th>
                <th>Valor padrão</th>
                <th>Dia</th>
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
                  <td data-label="Valor padrão">{{ item.defaultAmount | currency: 'BRL' }}</td>
                  <td data-label="Dia">{{ item.competenceDay }}</td>
                  <td data-label="Recorrência">{{ recurrenceLabel[item.recurrenceType] }}</td>
                  <td data-label="Início">{{ item.startDate | date: 'dd/MM/yyyy' }}</td>
                  <td data-label="Situação">
                    <span class="badge" [class.is-off]="!item.isActive">
                      {{ item.isActive ? 'Ativa' : 'Inativa' }}
                    </span>
                  </td>
                  <td data-label="Ações" class="actions">
                    <a class="btn-ghost" [routerLink]="['/income-sources', item.id]">Editar</a>
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
export class IncomeList {
  private readonly service = inject(IncomeSourceService);
  private readonly categories = inject(CategoryService);

  readonly items = signal<IncomeSourceResponse[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly recurrenceLabel = RECURRENCE_TYPE_LABEL;

  private readonly categoryNames = signal(new Map<string, string>());

  constructor() {
    this.load();
  }

  categoryName(id: string): string {
    return this.categoryNames().get(id) ?? '—';
  }

  remove(item: IncomeSourceResponse): void {
    if (!confirm(`Excluir a receita "${item.description}"?`)) return;
    this.error.set(null);
    this.service.delete(item.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(apiErrorMessage(err, 'Não foi possível excluir a receita.')),
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
        this.error.set(apiErrorMessage(err, 'Não foi possível carregar as receitas.'));
        this.loading.set(false);
      },
    });
  }
}
