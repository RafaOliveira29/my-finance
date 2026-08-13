import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { CategoryResponse } from '../../core/models/cadastros.models';
import { CategoryList } from './category-list';

describe('CategoryList', () => {
  const url = 'http://localhost:5080/api/categories';
  let http: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoryList],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('mostra as categorias devolvidas pela API', async () => {
    const fixture = TestBed.createComponent(CategoryList);
    const categories: CategoryResponse[] = [
      { id: '1', name: 'Moradia', type: 'Expense', color: '#3366ff', icon: null, isActive: true },
      { id: '2', name: 'Salário', type: 'Income', color: null, icon: null, isActive: false },
    ];
    http.expectOne(url).flush(categories);
    await fixture.whenStable();

    const texto = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(texto).toContain('Moradia');
    expect(texto).toContain('Despesa');
    expect(texto).toContain('Salário');
    expect(texto).toContain('Receita');
    expect(texto).toContain('Inativa');
  });

  it('mostra o estado vazio quando não há categorias', async () => {
    const fixture = TestBed.createComponent(CategoryList);
    http.expectOne(url).flush([]);
    await fixture.whenStable();

    const texto = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(texto).toContain('Nenhuma categoria cadastrada ainda.');
  });

  it('mostra a mensagem do backend quando a exclusão é barrada (409)', async () => {
    const fixture = TestBed.createComponent(CategoryList);
    http.expectOne(url).flush([
      { id: '1', name: 'Moradia', type: 'Expense', color: null, icon: null, isActive: true },
    ]);
    await fixture.whenStable();

    vi.spyOn(window, 'confirm').mockReturnValue(true);
    fixture.componentInstance.remove(fixture.componentInstance.items()[0]);
    http
      .expectOne(`${url}/1`)
      .flush({ detail: 'Esta categoria está em uso.' }, { status: 409, statusText: 'Conflict' });
    await fixture.whenStable();

    expect(fixture.componentInstance.error()).toBe('Esta categoria está em uso.');
  });
});
