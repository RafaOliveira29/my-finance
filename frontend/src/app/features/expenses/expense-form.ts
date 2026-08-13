import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CategoryService } from '../../core/cadastros/category.service';
import { ExpenseSourceService } from '../../core/cadastros/expense-source.service';
import { todayIso } from '../../core/date';
import { apiErrorMessage } from '../../core/http/api-error';
import { CategoryResponse, RecurrenceType } from '../../core/models/cadastros.models';

@Component({
  selector: 'app-expense-form',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <section class="page">
      <header class="page-head">
        <h1>{{ isEdit ? 'Editar despesa fixa' : 'Nova despesa fixa' }}</h1>
      </header>

      @if (error(); as message) {
        <p class="alert-error" role="alert">{{ message }}</p>
      }

      @if (loading()) {
        <p class="state">Carregando…</p>
      } @else if (categories().length === 0) {
        <p class="state">
          Cadastre antes uma categoria do tipo <strong>Despesa</strong> —
          <a routerLink="/categories/new">criar categoria</a>.
        </p>
      } @else {
        <form class="form-card" [formGroup]="form" (ngSubmit)="submit()">
          <label class="field">
            Descrição
            <input type="text" formControlName="description" maxlength="150" />
            @if (invalid('description')) {
              <small class="field-error">Informe a descrição.</small>
            }
          </label>

          <label class="field">
            Categoria
            <select formControlName="categoryId">
              @for (category of categories(); track category.id) {
                <option [value]="category.id">
                  {{ category.name }}{{ category.isActive ? '' : ' (inativa)' }}
                </option>
              }
            </select>
          </label>

          <div class="field-row">
            <label class="field">
              Valor padrão (R$)
              <input type="number" formControlName="defaultAmount" min="0" step="0.01" />
              @if (invalid('defaultAmount')) {
                <small class="field-error">Informe um valor igual ou maior que zero.</small>
              }
            </label>

            <label class="field">
              Dia do vencimento
              <input type="number" formControlName="dueDay" min="1" max="31" placeholder="opcional" />
              @if (invalid('dueDay')) {
                <small class="field-error">Informe um dia entre 1 e 31, ou deixe em branco.</small>
              }
            </label>
          </div>

          <label class="field">
            Mês do vencimento
            <select formControlName="dueMonthOffset">
              <option [ngValue]="0">No mês da competência</option>
              <option [ngValue]="1">No mês seguinte</option>
            </select>
            <small>Ex.: a fatura de janeiro que vence em fevereiro usa "no mês seguinte".</small>
          </label>

          <label class="field">
            Recorrência
            <select formControlName="recurrenceType">
              <option value="Monthly">Mensal</option>
              <option value="OneTime">Única</option>
            </select>
          </label>

          <div class="field-row">
            <label class="field">
              Início
              <input type="date" formControlName="startDate" />
              @if (invalid('startDate')) {
                <small class="field-error">Informe a data de início.</small>
              }
            </label>

            <label class="field">
              Fim (opcional)
              <input type="date" formControlName="endDate" />
            </label>
          </div>

          <label class="field">
            Observações
            <textarea formControlName="notes" rows="2" maxlength="500"></textarea>
          </label>

          @if (isEdit) {
            <label class="field field-check">
              <input type="checkbox" formControlName="isActive" />
              Despesa ativa
            </label>
          }

          <div class="form-actions">
            <button type="submit" class="btn-primary" [disabled]="saving()">
              {{ saving() ? 'Salvando…' : 'Salvar' }}
            </button>
            <a class="btn-ghost" routerLink="/expense-sources">Cancelar</a>
          </div>
        </form>
      }
    </section>
  `,
})
export class ExpenseForm {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(ExpenseSourceService);
  private readonly categoryService = inject(CategoryService);
  private readonly router = inject(Router);
  private readonly id = inject(ActivatedRoute).snapshot.paramMap.get('id');

  readonly isEdit = this.id !== null;
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  private readonly allCategories = signal<CategoryResponse[]>([]);
  /** Despesa só aceita categoria de despesa — evita erro de validação no backend. */
  readonly categories = computed(() => this.allCategories().filter((c) => c.type === 'Expense'));

  readonly form = this.fb.nonNullable.group({
    description: ['', [Validators.required, Validators.maxLength(150)]],
    categoryId: ['', [Validators.required]],
    defaultAmount: [0, [Validators.required, Validators.min(0)]],
    dueDay: this.fb.control<number | null>(null, [Validators.min(1), Validators.max(31)]),
    dueMonthOffset: [0],
    recurrenceType: ['Monthly' as RecurrenceType, [Validators.required]],
    startDate: [todayIso(), [Validators.required]],
    endDate: [''],
    notes: [''],
    isActive: [true],
  });

  constructor() {
    this.load();
  }

  invalid(control: 'description' | 'defaultAmount' | 'dueDay' | 'startDate'): boolean {
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
      categoryId: value.categoryId,
      description: value.description,
      // A Fase 2 cadastra apenas despesas fixas; as avulsas entram como lançamento do mês na Fase 3.
      expenseKind: 'Fixed' as const,
      defaultAmount: value.defaultAmount,
      dueDay: value.dueDay,
      dueMonthOffset: value.dueMonthOffset,
      recurrenceType: value.recurrenceType,
      startDate: value.startDate,
      endDate: value.endDate || null,
      notes: value.notes.trim() || null,
    };
    const request = this.id
      ? this.service.update(this.id, { ...payload, isActive: value.isActive })
      : this.service.create(payload);

    request.subscribe({
      next: () => this.router.navigateByUrl('/expense-sources'),
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Não foi possível salvar a despesa.'));
        this.saving.set(false);
      },
    });
  }

  private load(): void {
    this.categoryService.list().subscribe({
      next: (categories) => {
        this.allCategories.set(categories);
        const first = this.categories()[0];
        if (first) this.form.controls.categoryId.setValue(first.id);
        if (this.id) this.loadSource(this.id);
        else this.loading.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Não foi possível carregar as categorias.'));
        this.loading.set(false);
      },
    });
  }

  private loadSource(id: string): void {
    this.service.getById(id).subscribe({
      next: (source) => {
        this.form.setValue({
          description: source.description,
          categoryId: source.categoryId,
          defaultAmount: source.defaultAmount,
          dueDay: source.dueDay,
          dueMonthOffset: source.dueMonthOffset,
          recurrenceType: source.recurrenceType,
          startDate: source.startDate,
          endDate: source.endDate ?? '',
          notes: source.notes ?? '',
          isActive: source.isActive,
        });
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(apiErrorMessage(err, 'Despesa não encontrada.'));
        this.loading.set(false);
      },
    });
  }
}
