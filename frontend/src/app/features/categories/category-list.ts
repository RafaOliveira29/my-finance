import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CategoryService } from '../../core/cadastros/category.service';
import { apiErrorMessage } from '../../core/http/api-error';
import { CATEGORY_TYPE_LABEL, CategoryResponse } from '../../core/models/cadastros.models';

@Component({
  selector: 'app-category-list',
  imports: [RouterLink],
  template: `
    <section class="page">
      <header class="page-head">
        <h1>Categorias</h1>
        <a class="btn-primary" routerLink="/categories/new">Nova categoria</a>
      </header>
      <p class="page-hint">
        Categorias organizam receitas e despesas. Uma categoria em uso não pode ser excluída —
        nesse caso, inative-a pela edição.
      </p>

      @if (error(); as message) {
        <p class="alert-error" role="alert">{{ message }}</p>
      }

      @if (loading()) {
        <p class="state">Carregando…</p>
      } @else if (items().length === 0) {
        <p class="state">Nenhuma categoria cadastrada ainda.</p>
      } @else {
        <div class="table-wrap">
          <table class="data">
            <thead>
              <tr>
                <th>Nome</th>
                <th>Tipo</th>
                <th>Cor</th>
                <th>Situação</th>
                <th class="actions">Ações</th>
              </tr>
            </thead>
            <tbody>
              @for (item of items(); track item.id) {
                <tr>
                  <td data-label="Nome">{{ item.name }}</td>
                  <td data-label="Tipo">{{ typeLabel[item.type] }}</td>
                  <td data-label="Cor">
                    @if (item.color) {
                      <span class="color-dot" [style.background]="item.color"></span>
                      {{ item.color }}
                    } @else {
                      —
                    }
                  </td>
                  <td data-label="Situação">
                    <span class="badge" [class.is-off]="!item.isActive">
                      {{ item.isActive ? 'Ativa' : 'Inativa' }}
                    </span>
                  </td>
                  <td data-label="Ações" class="actions">
                    <a class="btn-ghost" [routerLink]="['/categories', item.id]">Editar</a>
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
export class CategoryList {
  private readonly service = inject(CategoryService);

  readonly items = signal<CategoryResponse[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly typeLabel = CATEGORY_TYPE_LABEL;

  constructor() {
    this.load();
  }

  remove(item: CategoryResponse): void {
    if (!confirm(`Excluir a categoria "${item.name}"?`)) return;
    this.error.set(null);
    this.service.delete(item.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(apiErrorMessage(err, 'Não foi possível excluir a categoria.')),
    });
  }

  private load(): void {
    this.loading.set(true);
    this.service.list().subscribe({
      next: (categories) => {
        this.items.set(categories);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Não foi possível carregar as categorias.'));
        this.loading.set(false);
      },
    });
  }
}
