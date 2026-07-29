---
tipo: requisitos-arquitetura
projeto: MyFinance
artefato: 06-arquitetura
atualizado: 2026-07-17
---

# Artefato 6 — Arquitetura — MyFinance

> **Trava de stack e arquitetura (Etapa 3).** Clean Architecture **pragmática, orientada a performance**, sem over-engineering e sem dívida herdada.

## Stack (travada)
| Camada | Escolha | Versão-alvo |
|---|---|---|
| Front | **Angular** standalone, **signals**, **zoneless**, OnPush, lazy routes | 21+ (o Projeto A já estava em 21.2) |
| Estilo/UI | Tailwind v4 + design system próprio enxuto (herdando o de A, com padrões visuais/UX de B: empty/loading states, gráficos) | — |
| Back | **ASP.NET Core Web API (C#)** | .NET 10 (LTS atual) |
| ORM/Dados | **EF Core** (escrita) + **Dapper**/SQL de projeção (leitura quente) + **Npgsql** | — |
| Banco | **PostgreSQL** | 16+ |
| Contrato | **OpenAPI** → geração de modelos TS (NSwag/Kiota) | — |

## Estilo — qual e **por quê (performance)**
**Clean Architecture em 4 projetos** (`Domain` / `Application` / `Infrastructure` / `API`) — regra de dependência para dentro; `Domain` sem framework — **com duas adaptações deliberadas para performance**, porque o caminho quente deste produto é **leitura agregada** (dashboard, resumo do mês, painel de dívidas):

1. **CQRS-lite (comando ≠ consulta), sem Event Sourcing.**
   - **Escrita** (cadastros, geração do mês, pagamento): passa pelo domínio rico + EF Core (rastreamento, transação, `RowVersion`). Correção > velocidade.
   - **Leitura** de telas pesadas (resumo, dashboard, listas com agregação): **projeção direta para DTO** via SQL/Dapper com `GROUP BY` **no Postgres** — sem materializar agregados em memória, sem `AsTracking`.
   - **Por quê:** elimina de saída os dois maiores gargalos herdados — o resumo **O(n·m)** em memória do Projeto A e a **mesma query repetida/sem índice** do Projeto B. Agregar no banco é o que sustenta o **RNF01 (< 1,5 s / 1.000 lançamentos)** e **RNF02 (p95 < 300 ms)**.

2. **Snapshot mensal materializado (`MonthlyEntry`) em vez de recomputar a cada consulta.**
   - O previsto do mês é gravado uma vez (geração idempotente). Leituras do mês são um `SELECT` indexado, não um recálculo de regras.
   - **Por quê:** transforma o dashboard num read barato; performance previsível independente do histórico.

**Decisões de performance de apoio (com pé no chão):**
- **Índices** desenhados para as queries reais: composto `MonthlyEntry(UserId, ReferenceMonth)`, `(UserId, Status)`, `Debt(UserId, Status/Priority)` — ver `05`.
- **EF Core:** `AsNoTracking` em toda leitura; **compiled queries** nos caminhos quentes; **sem lazy loading** (includes explícitos / split queries); `DbContext` scoped; **Npgsql pooling**.
- **`numeric(18,2)`** no banco e `decimal` em C# fim a fim (RN14) — precisão sem custo de correção depois.
- **Multi-tenant por `HasQueryFilter(UserId)`** — segurança **e** poda de índice (todas as queries já nascem escopadas).
- **Paginação** obrigatória em listas; **output cache/ETag** no resumo do mês (invalidado em escrita).
- **Transações** curtas e explícitas em pagamento/estorno/geração (RN07/RN08).
- **Front:** **zoneless + signals + OnPush** (menos change detection), **lazy loading** por feature, `trackBy`, camada `ApiService` tipada gerada do OpenAPI (zero `any`), **um CRUD base parametrizado** para matar o copy-paste de A.

**Por que não outra coisa:** Layered puro deixaria o domínio de dinheiro anêmico (erro de A); Microservices/Event-Driven/CQRS-full seriam cerimônia sem ganho para um app pessoal (over-engineering que o dono vetou). Clean-pragmático + CQRS-lite é o ponto de melhor performance por esforço.

## Camadas / estrutura de pastas

**Repo — monorepo simétrico** ([[organizacao-repositorio]]): `backend/` e `frontend/` como pastas **irmãs**; `descoberta/`, `.github/workflows/`, `docker-compose.yml`, `DOC.md`/`README.md` na **raiz**. CI único builda os dois; contrato OpenAPI → TS cruza as pastas.

### Back — `backend/` (`MyFinance.slnx` · `src/` · `tests/` · `global.json`)
```
MyFinance.Domain/           # entidades ricas (setters privados, factories, guards), enums, VOs, interfaces de domínio. SEM framework.
  Entities/  Enums/  ValueObjects/  Abstractions/
MyFinance.Application/      # casos de uso, DTOs, validators, interfaces (ports)
  Common/ (Result, erros, paginação)
  Categories/ Incomes/ Expenses/ Creditors/ Debts/ Installments/
  MonthlyEntries/ Payments/ Summary/ Import/ Settings/
  Abstractions/ (IUnitOfWork, IReadStore, IMonthlyEntryGenerator, IPaymentService, IMonthlySummaryService, ICurrentUser)
MyFinance.Infrastructure/   # EF Core (escrita) + Dapper (leitura) + Npgsql
  Persistence/ (MyFinanceDbContext, Configurations, Migrations)
  ReadModels/ (queries Dapper de resumo/dashboard/painel de dívida)
  Auth/ (Argon2, JWT)  Import/ (parser CSV)  Services/
MyFinance.API/             # controllers finos, middleware (exceção→ProblemDetails), DI, auth, OpenAPI
  Controllers/ Middleware/ Extensions/ Program.cs
tests/
  MyFinance.Domain.Tests/  MyFinance.Application.Tests/  MyFinance.Api.IntegrationTests/
```
**Regra de dependência:** `Domain` ⟵ `Application` ⟵ `Infrastructure`/`API`. `Domain` não conhece EF/API.

### Front — `frontend/` (Angular, feature-based / modular)
```
src/app/
  core/        # auth, http (interceptors: token, erro), api gerada (OpenAPI), guards, current-user, config
  shared/      # design system (button, card, table, modal, empty/loading state, money/date pipes), CRUD base
  features/
    auth/ dashboard/ incomes/ expenses/ creditors/ debts/ installments/
    monthly/ payments/ summary/ categories/ import/ settings/
  layout/      # sidebar, header, main-layout
```
- **Interceptors** (o que faltava em A): token JWT + tratamento global de erro (ProblemDetails → toast). **Guard** de rota autenticada.
- **Estado:** signals + serviços por feature (sem Redux — pé no chão). `httpResource`/RxJS onde fizer sentido.

## Fluxo de uma requisição
**Escrita:** `Controller (fino)` → `Application UseCase/Service` (validação + regra) → `Domain` (invariantes) → `EF Core / UnitOfWork` (transação) → **PostgreSQL**.
**Leitura quente:** `Controller` → `Application Query` → `IReadStore (Dapper, GROUP BY)` → **PostgreSQL** → `DTO` (sem passar por entidades rastreadas).

## Prompts de handoff (para o chat que vai codar)
> **Backend — bootstrap:** "Crie a solution `MyFinance` em ASP.NET Core (.NET 10) + PostgreSQL, Clean Architecture pragmática (Domain/Application/Infrastructure/API). `Domain` sem framework, entidades **ricas** (setters privados + factories). Entidades: User, Category, IncomeSource, ExpenseSource, Creditor, Debt, InstallmentPlan, MonthlyEntry, Payment, Settings (ver `05`). EF Core (escrita) com Fluent API por entidade, `numeric(18,2)` para dinheiro, `DateOnly`/`DATE` para competência/vencimento, enums como string, PK Guid, **global query filter por UserId**, **RowVersion (xmin)** em MonthlyEntry/Debt. Crie CHECK de exatamente-uma-fonte em MonthlyEntry e os índices do `05`. Middleware global de exceção → ProblemDetails."
> **Backend — motor:** "Implemente `IMonthlyEntryGenerator` (geração idempotente do mês, RN04/RN10), `IPaymentService` (pagamento atômico + estorno + guarda de superpagamento, RN06–RN08, status derivado RN05) e `IMonthlySummaryService` (agregação via Dapper/GROUP BY, RNF01). Cubra com testes (RNF08)."
> **Front — bootstrap:** "Crie o app Angular 21 standalone, zoneless + signals + OnPush, Tailwind v4. Gere os modelos/serviços TS do OpenAPI da API (zero `any`). Interceptors de token e de erro (ProblemDetails→toast), guard de auth, CRUD base parametrizado. Telas por feature (ver estrutura). Responsivo 320–1440 (RNF03), tema claro/escuro."

## Ver também
- [[07-exemplo-uso]] · [[05-modelagem-dominio]] · [[estilos-arquiteturais]]
