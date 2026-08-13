import { HttpErrorResponse } from '@angular/common/http';
import { apiErrorMessage } from './api-error';

describe('apiErrorMessage', () => {
  const errorWith = (body: unknown) => new HttpErrorResponse({ error: body, status: 400 });

  it('usa os erros de validação quando o backend devolve 400 com a lista', () => {
    const error = errorWith({ title: 'Erro de validação', errors: { Name: ['Informe o nome.'] } });
    expect(apiErrorMessage(error, 'fallback')).toBe('Informe o nome.');
  });

  it('usa o detail nas regras de negócio (ex.: categoria em uso)', () => {
    const error = errorWith({ title: 'Conflito', detail: 'Esta categoria está em uso.' });
    expect(apiErrorMessage(error, 'fallback')).toBe('Esta categoria está em uso.');
  });

  it('cai para o title quando não há detail', () => {
    expect(apiErrorMessage(errorWith({ title: 'Não encontrado' }), 'fallback')).toBe(
      'Não encontrado',
    );
  });

  it('cai para a mensagem padrão quando o erro não é ProblemDetails', () => {
    expect(apiErrorMessage(errorWith(null), 'fallback')).toBe('fallback');
    expect(apiErrorMessage(null, 'fallback')).toBe('fallback');
  });
});
