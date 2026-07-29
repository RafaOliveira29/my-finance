---
tipo: doc-projeto
projeto: MyFinance
stack: angular + dotnet + postgresql
arquitetura: Clean Architecture pragmática (CQRS-lite para leitura)
atualizado: 2026-07-17
---

# 📘 MyFinance — Documentação viva

> Controle financeiro pessoal orientado à **competência mensal** + **gestão de dívidas por credor**. Produto novo que **funde** dois projetos existentes (referência de domínio, não base de código). Nome provisório: **MyFinance** *(alternativas: Fôlego / Quitado / Saldo)*.

## 1. Visão
- **Ideia inicial:** um único app que responde "**quanto ainda posso gastar com segurança este mês**" e, no mesmo lugar, mostra "**o que devo, para quem, quanto, com que urgência, até quando, quanto paguei e quanto falta**" — com **visão em parcelas**, não só saldo total.
- **Estado final previsto:** MVP em 3 levas (núcleo mensal → dívidas/parcelas → conveniência), com auth real, dinheiro correto (`numeric`), integridade no banco e dashboard visual. Evoluções (metas, cartão, OFX, SaaS) no [[descoberta/08-features-futuras]].

## 2. Como começou
- **Brownfield / fusão.** Base de domínio vinda de:
  - **Projeto A** (`../Projeto A`) — .NET 8 Clean Arch + Angular 21 + **pacote de descoberta em 8 PDFs**. Domínio: competência mensal, renda/despesa, parcelamento, previsto×realizado, resumo. Código anêmico e sem auth, mas **doc excelente** e lógica de serviço boa.
  - **Projeto B** (`../Projeto B/finance-core`) — Next.js/React/shadcn gerado pelo v0.dev. Domínio: **dívidas/credores**, pagamentos de dívida, despesas avulsas, dashboard com gráficos, import CSV. Código de scaffold não revisado, mas **schema Postgres bom** e **UX/gráficos/import** úteis.
- **Nenhuma linha copiada.** Extraiu-se o **QUÊ** (domínio/requisitos/features) e reconstrói-se o **COMO** com qualidade. O estudo completo (entendimento, comparativo, dívidas a não repetir) está resumido no histórico e destilado no pacote de descoberta.

## 3. Estado atual
- ✅ **Etapa 1** — Estudo profundo dos dois projetos (leitura integral + 8 PDFs).
- ✅ **Etapa 2** — Pacote de descoberta reconciliado (8 artefatos, abaixo).
- ✅ **Etapa 3** — Stack/arquitetura travadas ([[descoberta/06-arquitetura]]).
- ✅ **Etapa 4** — Artefatos salvos em `MyFinance/`.
- ✅ **Revisão adversarial concluída** (3 críticos: régua do cérebro, domínio/modelo, rastreabilidade) → veredito *aprovado com ressalvas*; **12 must-fix + 7 nice-to-fix aplicados** ao pacote (ver §5).
- ✅ **Etapa 5** — Plano em fases aprovado pelo dono (2026-07-17).
- ✅ **Fase 0 (fundação) concluída e verificada** — build 0 avisos/0 erros, 3 testes verdes, API sobe (`/health`, `/openapi/v1.json` 3.1.1), Postgres via docker + pipeline EF ok, Angular 21 zoneless buildando, CI configurado.
- ▶️ **Próxima: Fase 1 (Auth & tenant)**.

> **➡️ PONTO DE RETOMADA:** Fase 0 concluída e verificada. **Próximo passo: Fase 1 — Auth & tenant** (cadastro/login/JWT/logout, Argon2id, global query filter por `UserId`, guard/interceptors no front; prova CA001–CA007). **Card vivo espelhado no cérebro:** `Cerebro/projetos/MyFinance/DOC.md` (regra [[documentacao-viva]]); este `DOC.md` do repo é a doc viva completa. **Projeto agora vive no repo git `my-finance`** (GitHub: `RafaOliveira29/my-finance`); ainda **não commitado/pushado** por mim (aguardando seu pedido).

### Pacote de descoberta
1. [[descoberta/01-produto]] — PRD (visão, MVP, escopo in/out, RNFs mensuráveis, riscos).
2. [[descoberta/02-epicos]] — 14 épicos priorizados (MoSCoW + levas).
3. [[descoberta/03-historias]] — 60 HU + 10 HT unificadas (proveniência A/B/novo), em levas.
4. [[descoberta/04-criterios-aceite]] — 80 CAs (Gherkin, **IDs globais únicos**) + matriz de rastreabilidade + RF.
5. [[descoberta/05-modelagem-dominio]] — glossário, 10 entidades ricas, relacionamentos, regras de status, RN01–RN15, anti-patterns.
6. [[descoberta/06-arquitetura]] — stack travada + Clean pragmática/CQRS-lite (justificativa de performance).
7. [[descoberta/07-exemplo-uso]] — walkthrough (abril/2026) que valida o modelo, com dívida parcelada e à vista.
8. [[descoberta/08-features-futuras]] — roadmap + guarda de conceitos.

## 4. Arquitetura
**Clean Architecture pragmática** (4 camadas: Domain/Application/Infrastructure/API) + **CQRS-lite**: escrita pelo domínio rico + EF Core (transação, `RowVersion`); **leitura quente** (dashboard/resumo/painel de dívida) por **projeção SQL/Dapper com `GROUP BY` no Postgres**. Snapshot mensal materializado (`MonthlyEntry`). Detalhe e justificativa em [[descoberta/06-arquitetura]]. Front Angular 21 zoneless + signals + OnPush, contrato TS gerado do OpenAPI.

**Repo — monorepo simétrico** ([[organizacao-repositorio]]): `backend/` (`MyFinance.slnx`, `src/`, `tests/`, `global.json`) e `frontend/` como pastas irmãs; `descoberta/`, `.github/`, `docker-compose.yml`, `DOC.md`, `README.md` na raiz. CI único builda os dois.

## 5. Decisões
- **[2026-07-17] Nome** — opções: MyFinance / Fôlego / Quitado / Saldo · **escolhida (provisória):** MyFinance · **porquê:** aprovado pelo dono; renomear a pasta é trivial.
- **[2026-07-17] Stack** — **escolhida:** Angular 21+ · .NET (Clean) · PostgreSQL · **porquê:** decisão do dono; A já estava em Angular 21 e .NET Clean — herda a stack-alvo, descarta o código.
- **[2026-07-17] Estilo arquitetural** — opções: Layered / Clean / Clean+CQRS-full · **escolhida:** Clean pragmática + **CQRS-lite** · **porquê:** o caminho quente é leitura agregada; agregar no banco elimina o O(n·m) de A e as queries repetidas de B (RNF01/02) sem a cerimônia de CQRS-full.
- **[2026-07-17] Reconciliação de domínio** — **parcelamento migra para a Dívida** (`Debt` 1:1 `InstallmentPlan`); `Payment` único ancorado em `MonthlyEntry` (fonte única do realizado); despesa avulsa vira `MonthlyEntry Manual` (sem tabela `Expense`); renda preservada; discriminador `EntryType`+`SourceType`+CHECK. **porquê:** entrega a ênfase do dono (painel de dívida + parcelas) num só agregado e mata as dívidas dos dois modelos.
- **[2026-07-17] Não repetir** — auth real desde o dia 1, `numeric(18,2)` fim a fim, pagamento atômico, constraints no banco, zero feature decorativa, testes de dinheiro. Ver dívidas catalogadas no estudo (Etapa 1).
- **[2026-07-17] Revisão adversarial aplicada** — 3 críticos (régua/domínio/rastreabilidade) → *aprovado com ressalvas*; 12 must-fix + 7 nice-to-fix aplicados. Principais ajustes de modelagem (`05`): (a) **dívida à vista** projeta exatamente 1 competência no mês do `DueDate` (RN16, idempotente); (b) **`Debt.Status`** deriva do `InstallmentPlan.IsCompleted`, não das parcelas já materializadas; (c) origens com FK `RESTRICT`+soft-delete (não `SET NULL`, que colidia com o CHECK); (d) **pago/falta 100% derivados** (sem cache/coluna gravável — fecha a dupla-verdade); (e) **UNIQUE parciais por `SourceType`** para a idempotência; (f) **`AvailableToSpend`** define "já gasto" só como Manual (sem contagem dupla); (g) dimensão **`Nature`** materializada para as quebras; (h) **`Overdue` derivado** (não persistido), com índice por `DueDate`; (i) `ExpenseSource.DueMonthOffset` para competência ≠ vencimento; (j) política de superpagamento e ordenação de dívidas fixadas; + 5 CAs novos (CA076–CA080) e alinhamento de levas/links.
- **[2026-07-17] Monorepo simétrico** — reorganizado para `backend/` + `frontend/` irmãos (convenção [[organizacao-repositorio]]); `src/`, `tests/`, `MyFinance.slnx`, `global.json` movidos para `backend/`. Paths internos do `.slnx`/`.csproj` inalterados (relativos, moveram juntos); ajustados apenas o job backend do CI (`working-directory: backend`) e o README. `docker-compose.yml` (sem build context) e `.gitignore` (padrões não-ancorados) não precisaram mudar. Build/test/`npm start` revalidados verdes. **Sem git** (a pedido do dono).
- **[2026-07-17] Movido para o repositório git `my-finance`** — todo o projeto passou para o clone `projetos com claude/my-finance/` (GitHub `RafaOliveira29/my-finance`), preservando o `.git` do clone. A pasta é kebab (`my-finance`), mas os nomes internos seguem `MyFinance.*` (assemblies/namespaces) e `myfinance` (Angular) — independem do nome da pasta, então **nada quebrou**. O move inicial foi parcial (2 processos travando: a `MyFinance.API.exe` do smoke test e o `esbuild` do `ng serve`); recuperado matando os locks, movendo o fonte, reinstalando `frontend/node_modules` e removendo a `MyFinance/` antiga (com o `.git` acidental da Fase 0). Revalidado: **build 0/0, 3 testes ✔, `npm start` 200 + watch**. Sem commit/push (não solicitado).
- **[2026-07-17] Fase 0 — escolhas de fundação** — solution no formato **`.slnx`** (default do .NET 10); **Shouldly** em vez de FluentAssertions (v8+ exige licença paga); **`Microsoft.OpenApi` fixado em 2.11.0** para sanar a vuln **NU1903/GHSA-v5pm-xwqc-g5wc** da transitiva 2.0.0 (o v3 quebra o source generator do framework, então mantida a linha v2); **Angular zoneless explícito** (`provideZonelessChangeDetection`); Postgres exposto na porta **5433** (evita conflito com instância local); connection string dev só em `appsettings.Development.json` (prod via env `ConnectionStrings__Default`).

## 6. Onde paramos / próximos passos — **PLANO EM FASES (aguardando OK)**

> Sequência pensada para provar o motor de dinheiro cedo e entregar a ênfase de dívidas sem retrabalho. Cada fase termina **compilando, testada e verificável**.

- ✅ **Fase 0 — Fundação** (E14/HT) — **CONCLUÍDA (2026-07-17)**: solution `.slnx` .NET 10 (Domain/Application/Infrastructure/API + 2 de teste, refs Clean); EF Core 10 + Npgsql + Dapper; `MyFinanceDbContext` + factory de design-time; `GlobalExceptionHandler`→ProblemDetails; OpenAPI (`/openapi/v1.json`); healthcheck; base `Entity`/`AggregateRoot`; seams `AddApplication`/`AddInfrastructure`; docker-compose Postgres 16; Angular 21 **zoneless** em `frontend/`; CI GitHub Actions. **Verificado:** build 0/0, `dotnet test` 3✔, smoke API (`/health`+OpenAPI), EF `dbcontext info` + `db_ok=1`, front build ✔. **Prova:** CA071–CA073. *Obs.: geração de tipos TS do OpenAPI é ligada na Fase 1 (quando surgem os 1ºs endpoints), a partir do `/openapi/v1.json` já emitido.*
- **Fase 1 — Auth & tenant** (E1/HT09): cadastro/login/JWT/logout, Argon2id, global query filter por `UserId`, guard/interceptors no front. **Prova:** isolamento (CA001–CA007).
- **Fase 2 — Cadastros base** (E2/E3/E11 core): Categorias (cor/ícone), IncomeSource, ExpenseSource; CRUD base parametrizado no front. **Prova:** CA008–CA019, CA062–CA063, CA065, CA076.
- **Fase 3 — Competência & pagamentos** (E7/E8/E4): motor de geração idempotente, lançamento avulso, PaymentService (parcial/total, superpagamento, estorno, status derivado, atômico + RowVersion). **Prova:** CA020–CA023, CA038–CA049, CA079. *(coração do dinheiro — com testes, RNF08.)*
- **Fase 4 — Resumo & dashboard** (E9/E10): MonthlySummary via Dapper/GROUP BY; dashboard com KPIs, gráficos e alertas; "quanto posso gastar". **Prova:** CA050–CA061 (RNF01/02).
- **Fase 5 — Dívidas & parcelas (ênfase)** (E5/E6): Creditor, Debt, InstallmentPlan (ajuste de centavos, projeção em competências), painel de dívida (o quê/para quem/quanto/prioridade/vencimento/pago/falta), visão em parcelas, priorização. **Prova:** CA024–CA037, CA064 (filtros), CA077–CA078, CA080.
- **Fase 6 — Conveniência** (E13/E12): Settings aplicadas de fato; Import CSV (bulk/transação, parser robusto, erro por linha); alertas de vencimento. **Prova:** CA066–CA070.
- **Fase 7 — Polimento**: responsividade 320–1440 auditada, tema claro/escuro, i18n, acessibilidade AA, testes E2E dos fluxos-chave, verificação de performance (RNF01/02). **Prova:** CA074–CA075 (numeric fim a fim, cobertura de dinheiro).

> **Cobertura:** as 7 fases cobrem os **80 CAs** (CA001–CA080). **Gate de qualidade:** a Fase 3 (coração do dinheiro) só inicia com o `05` já refletindo os ajustes da revisão (geração de dívida à vista/parcelada, status derivado, UNIQUE parciais, sem dupla-verdade) — já aplicados.

- [x] OK do dono (2026-07-17) — Fase 0 **concluída e verificada**.
- [ ] **Fase 1 — Auth & tenant** (próxima).

## 7. Documentação viva (onde mora)
- **Fonte da verdade (repo):** este `DOC.md` + `descoberta/01..08` — versionados com o código.
- **Card vivo (cérebro):** `Cerebro/projetos/MyFinance/DOC.md` — curto, aponta para o repo; atualizado ao fim de cada bloco de trabalho (regra [[documentacao-viva]]).

## 8. Pilares aplicados
Segue os pilares compartilhados: [[persona-dev]], [[principios-codigo]], [[responsividade-e-ux]], [[documentacao-viva]].
