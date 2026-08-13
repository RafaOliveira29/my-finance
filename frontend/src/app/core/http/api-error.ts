import { HttpErrorResponse } from '@angular/common/http';

interface ProblemDetails {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
}

/**
 * Mensagem legível de um erro da API. O backend responde sempre em ProblemDetails: erros de
 * validação (400) vêm em `errors`, regras de negócio (409) e demais falhas em `detail`.
 */
export function apiErrorMessage(error: unknown, fallback: string): string {
  const problem = (error as HttpErrorResponse | null)?.error as ProblemDetails | null;
  const validationMessages = problem?.errors ? Object.values(problem.errors).flat() : [];
  if (validationMessages.length > 0) return validationMessages.join(' ');
  return problem?.detail ?? problem?.title ?? fallback;
}
