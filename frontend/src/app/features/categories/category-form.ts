import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CategoryService } from '../../core/cadastros/category.service';
import { apiErrorMessage } from '../../core/http/api-error';
import { CategoryType } from '../../core/models/cadastros.models';

@Component({
  selector: 'app-category-form',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <section class="page">
      <header class="page-head">
        <h1>{{ isEdit ? 'Editar categoria' : 'Nova categoria' }}</h1>
      </header>

      @if (error(); as message) {
        <p class="alert-error" role="alert">{{ message }}</p>
      }

      @if (loading()) {
        <p class="state">Carregando…</p>
      } @else {
        <form class="form-card" [formGroup]="form" (ngSubmit)="submit()">
          <label class="field">
            Nome
            <input type="text" formControlName="name" maxlength="100" />
            @if (showRequired('name')) {
              <small class="field-error">Informe o nome da categoria.</small>
            }
          </label>

          <label class="field">
            Tipo
            <select formControlName="type">
              <option value="Expense">Despesa</option>
              <option value="Income">Receita</option>
            </select>
          </label>

          <label class="field">
            Cor
            <input type="color" formControlName="color" />
            <small>Usada nos gráficos do dashboard.</small>
          </label>

          <label class="field">
            Ícone
            <input type="text" formControlName="icon" maxlength="50" placeholder="ex.: home" />
          </label>

          @if (isEdit) {
            <label class="field field-check">
              <input type="checkbox" formControlName="isActive" />
              Categoria ativa
            </label>
          }

          <div class="form-actions">
            <button type="submit" class="btn-primary" [disabled]="saving()">
              {{ saving() ? 'Salvando…' : 'Salvar' }}
            </button>
            <a class="btn-ghost" routerLink="/categories">Cancelar</a>
          </div>
        </form>
      }
    </section>
  `,
})
export class CategoryForm {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(CategoryService);
  private readonly router = inject(Router);
  private readonly id = inject(ActivatedRoute).snapshot.paramMap.get('id');

  readonly isEdit = this.id !== null;
  readonly loading = signal(this.isEdit);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    type: ['Expense' as CategoryType, [Validators.required]],
    color: ['#4f46e5'],
    icon: [''],
    isActive: [true],
  });

  constructor() {
    if (this.id) this.load(this.id);
  }

  showRequired(control: 'name'): boolean {
    const field = this.form.controls[control];
    return field.touched && field.invalid;
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.error.set(null);

    const value = this.form.getRawValue();
    const payload = {
      name: value.name,
      type: value.type,
      color: value.color || null,
      icon: value.icon.trim() || null,
    };
    const request = this.id
      ? this.service.update(this.id, { ...payload, isActive: value.isActive })
      : this.service.create(payload);

    request.subscribe({
      next: () => this.router.navigateByUrl('/categories'),
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Não foi possível salvar a categoria.'));
        this.saving.set(false);
      },
    });
  }

  private load(id: string): void {
    this.service.getById(id).subscribe({
      next: (category) => {
        this.form.setValue({
          name: category.name,
          type: category.type,
          color: category.color ?? '#4f46e5',
          icon: category.icon ?? '',
          isActive: category.isActive,
        });
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Categoria não encontrada.'));
        this.loading.set(false);
      },
    });
  }
}
