import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { CategoryService } from './category.service';

/** Prova o CRUD base parametrizado através de um serviço concreto: verbo e URL de cada operação. */
describe('CategoryService (CRUD base)', () => {
  const url = 'http://localhost:5080/api/categories';
  let service: CategoryService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(CategoryService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('lista com GET no recurso', () => {
    service.list().subscribe();
    expect(http.expectOne(url).request.method).toBe('GET');
  });

  it('busca por id com GET no recurso/id', () => {
    service.getById('abc').subscribe();
    expect(http.expectOne(`${url}/abc`).request.method).toBe('GET');
  });

  it('cria com POST enviando o corpo', () => {
    const body = { name: 'Moradia', type: 'Expense' as const, color: null, icon: null };
    service.create(body).subscribe();
    const request = http.expectOne(url);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(body);
  });

  it('atualiza com PUT no recurso/id', () => {
    const body = { name: 'Lazer', type: 'Expense' as const, color: null, icon: null, isActive: true };
    service.update('abc', body).subscribe();
    const request = http.expectOne(`${url}/abc`);
    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual(body);
  });

  it('exclui com DELETE no recurso/id', () => {
    service.delete('abc').subscribe();
    expect(http.expectOne(`${url}/abc`).request.method).toBe('DELETE');
  });
});
