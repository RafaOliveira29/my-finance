/** Cadastros base (Fase 2): categorias, fontes de receita e despesas fixas. */

/** Enums trafegam como texto no JSON da API (JsonStringEnumConverter). */
export type CategoryType = 'Income' | 'Expense';
export type ExpenseKind = 'Fixed' | 'Variable';
export type RecurrenceType = 'OneTime' | 'Monthly';

export const CATEGORY_TYPE_LABEL: Record<CategoryType, string> = {
  Income: 'Receita',
  Expense: 'Despesa',
};

export const EXPENSE_KIND_LABEL: Record<ExpenseKind, string> = {
  Fixed: 'Fixa',
  Variable: 'Variável',
};

export const RECURRENCE_TYPE_LABEL: Record<RecurrenceType, string> = {
  OneTime: 'Única',
  Monthly: 'Mensal',
};

export interface CreateCategoryRequest {
  name: string;
  type: CategoryType;
  color: string | null;
  icon: string | null;
}

export interface UpdateCategoryRequest extends CreateCategoryRequest {
  isActive: boolean;
}

export interface CategoryResponse extends UpdateCategoryRequest {
  id: string;
}

export interface CreateIncomeSourceRequest {
  categoryId: string;
  description: string;
  defaultAmount: number;
  /** Dia do mês (1..31) em que a receita entra na competência. */
  competenceDay: number;
  recurrenceType: RecurrenceType;
  /** Data no formato 'AAAA-MM-DD' (DateOnly no backend). */
  startDate: string;
  endDate: string | null;
  notes: string | null;
}

export interface UpdateIncomeSourceRequest extends CreateIncomeSourceRequest {
  isActive: boolean;
}

export interface IncomeSourceResponse extends UpdateIncomeSourceRequest {
  id: string;
}

export interface CreateExpenseSourceRequest {
  categoryId: string;
  description: string;
  expenseKind: ExpenseKind;
  defaultAmount: number;
  /** Dia do vencimento no mês (1..31); nulo quando não há data fixa. */
  dueDay: number | null;
  /** 0 = vence no mês da competência; 1 = vence no mês seguinte (RN01). */
  dueMonthOffset: number;
  recurrenceType: RecurrenceType;
  startDate: string;
  endDate: string | null;
  notes: string | null;
}

export interface UpdateExpenseSourceRequest extends CreateExpenseSourceRequest {
  isActive: boolean;
}

export interface ExpenseSourceResponse extends UpdateExpenseSourceRequest {
  id: string;
}
