import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE } from '../config/api';

/**
 * Acesso HTTP ao CRUD padrão de um recurso da API. Cada serviço concreto informa apenas o
 * caminho do recurso (ex.: `categories`). O isolamento por usuário é do backend — vem do
 * token que o `authInterceptor` anexa a toda requisição.
 */
export abstract class CrudService<TResponse, TCreate, TUpdate = TCreate> {
  private readonly http = inject(HttpClient);
  private readonly url: string;

  protected constructor(resource: string) {
    this.url = `${API_BASE}/api/${resource}`;
  }

  list(): Observable<TResponse[]> {
    return this.http.get<TResponse[]>(this.url);
  }

  getById(id: string): Observable<TResponse> {
    return this.http.get<TResponse>(`${this.url}/${id}`);
  }

  create(body: TCreate): Observable<TResponse> {
    return this.http.post<TResponse>(this.url, body);
  }

  update(id: string, body: TUpdate): Observable<TResponse> {
    return this.http.put<TResponse>(`${this.url}/${id}`, body);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
