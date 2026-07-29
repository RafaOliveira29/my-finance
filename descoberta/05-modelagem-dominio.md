---
tipo: requisitos-modelagem
projeto: MyFinance
artefato: 05-modelagem-dominio
atualizado: 2026-07-17
---

# Artefato 5 — Modelagem de Domínio — MyFinance

Reconcilia o núcleo **Source → MonthlyEntry → Payment** (Projeto A) com **Creditor → Debt → (InstallmentPlan) → Payment** (Projeto B), num modelo único, rico e com integridade no banco. Validado pelo walkthrough do [[07-exemplo-uso]].

---

## Glossário (linguagem ubíqua) — o que NÃO confundir

| Termo | Entidade/Campo | Definição (e o que **não** é) |
|---|---|---|
| **Origem** | `IncomeSource` / `ExpenseSource` | Compromisso **recorrente estrutural** (o "molde": salário, aluguel, internet). **Não** é o evento do mês nem o pagamento. |
| **Competência** | `MonthlyEntry.ReferenceMonth` (DateOnly) | Mês a que o valor **pertence** financeiramente. **≠ vencimento ≠ data de pagamento.** |
| **Vencimento** | `DueDate` (DateOnly) | Data-limite para pagar. Deriva o status **Vencido**. |
| **Materialização / Lançamento mensal** | `MonthlyEntry` | A **instância do mês** (snapshot do previsto daquele mês). Coração do sistema. |
| **Previsto** | `MonthlyEntry.AmountExpected` | O quanto se espera do mês. Snapshot — não recalcula do zero a cada consulta. |
| **Realizado** | `soma(Payments)` | O quanto de fato aconteceu. **Fonte única da verdade** do pago. |
| **Pagamento** | `Payment` | Evento real de quitação (parcial ou total) contra uma competência. |
| **Dívida** | `Debt` | Obrigação a um **credor** (total, juros, prioridade, vencimento). Pode ser à vista (paga ad-hoc) ou **parcelada**. |
| **Credor** | `Creditor` | A **quem** se deve. |
| **Parcelamento** | `InstallmentPlan` | Plano que fatia o total de uma **dívida** em N parcelas → N competências. |
| **Categoria** | `Category` | Classificação (`Income`/`Expense`) com cor/ícone. **Não** é tipo de despesa nem forma de pagamento. |
| **Tipo de despesa** | `ExpenseKind` | `Fixed` / `Variable` — atributo **só da `ExpenseSource`** (natureza da despesa recorrente). **Não** é categoria nem a dimensão `Natureza` do lançamento. |
| **Natureza (do lançamento)** | `MonthlyEntry.Nature` | Dimensão **materializada no lançamento**: `Fixed` / `Variable` / `Installment` / `Debt` / `Manual` (derivada de `SourceType` + `ExpenseKind` na geração). É a dimensão usada nas **quebras** do resumo. **Não** confundir com `ExpenseKind` (que só existe na origem). |
| **Forma de pagamento** | `PaymentMethod` | Pix / Cartão / Dinheiro / Boleto… **Como** se pagou. **Não** é instrumento nem categoria. |
| **Instrumento financeiro** | *(futuro: `CreditCardAccount`)* | Cartão como fonte de limite/fatura. **Fora do MVP** — não confundir com forma de pagamento. |
| **Orçamento / Meta** | *(futuro: `Budget`)* | Teto planejado por categoria/mês. **Fora do MVP** — conceito preservado. |
| **Disponível** | `MonthlySummary.AvailableToSpend` | "Quanto ainda posso gastar com segurança" = **ReceitaPrevista − Comprometido − GastoVariávelRealizado**, onde *Comprometido* = previsto de `Nature ∈ {Fixed, Installment, Debt}` e *GastoVariávelRealizado* = `soma(Payments)` de lançamentos `SourceType=Manual`. **Nunca** a soma total de Payments (senão fixas/parcelas já pagas entram duas vezes). |

> **Regra de ouro:** categoria ≠ tipo de despesa ≠ forma de pagamento ≠ instrumento ≠ obrigação futura. Misturar isso é o que quebrou os dois projetos.

---

## Decisões de reconciliação (A × B) — as que importam

1. **Parcelamento migra para a Dívida.** No Projeto A o `InstallmentPlan` pendurava numa `ExpenseSource`; no B a `Debt` não tinha parcelas. **Unificado:** `InstallmentPlan` pertence a `Debt` (1:1). Toda compra parcelada é uma **dívida** (com ou sem credor nomeado) que projeta uma competência por parcela. Isso entrega a **ênfase do dono** (painel de dívida + visão em parcelas) num só agregado. `ExpenseSource` fica só com **recorrência** (`Fixed`/`Variable`) — sem `Installment`.
2. **Pagamento é único e ancora sempre em `MonthlyEntry`.** Fundimos `Payment`(A) + `DebtPayment`(B) numa entidade só. **Pago/falta são 100% derivados** (read-model Dapper/`GROUP BY` sobre `Payments`) — **não existe coluna `paid_amount` gravável** em `Debt` nem em `MonthlyEntry` (fecha de vez a dupla-verdade do B). *Se* um dia um total materializado for necessário por performance, ele terá **um único escritor** (`PaymentService`, mesma transação, cliente nunca escreve), com `RowVersion` na entidade e um **teste de invariante** `cache == soma(Payments)` após pagamento/estorno — mas o **default do MVP é derivado, sem cache**.
3. **Despesa avulsa não vira "fonte".** O `Expense` avulso do B entra como `MonthlyEntry` com `SourceType=Manual` — sem tabela `Expense` separada e sem forçar criar uma origem (peso do A). Melhor dos dois.
4. **Renda preservada.** `IncomeSource` + geração continuam (o B não tinha renda; sem ela não há "quanto posso gastar").
5. **Discriminador explícito + CHECK.** `MonthlyEntry` ganha `EntryType` (Income/Expense) e `SourceType` (IncomeSource/ExpenseSource/Debt/Manual) e um **CHECK de exatamente-uma-fonte** (corrige as 3 FKs nullable sem discriminador do A).
6. **Categoria unificada** em `Income|Expense` (sem o `both` ambíguo do B) + cor/ícone (do B) + CRUD completo (B era read-only). Dívida usa categoria de **despesa**.
7. **Enums unificados:** `EntryStatus` **persistido** = Pending/PartiallyPaid/Paid/Cancelled (`Overdue` é estado **derivado** de exibição, não coluna — ver Regras de estado); `PaymentMethod` = Pix/CreditCard/DebitCard/Cash/BankSlip/Transfer/Other; `DebtPriority` = Low/Medium/High/Urgent.
8. **Dinheiro `numeric(18,2)` fim a fim; datas de calendário como `DateOnly`/`DATE`.** Nada de float, nada de timestamp em data de competência.
9. **Multi-tenant real:** `UserId` em toda entidade de topo, **global query filter** + verificação de posse. `Payment` carrega `UserId` denormalizado (filtro/índice de tenant sem join).
10. **Um único dono da geração de competências.** O **gerador mensal** é o único que cria `MonthlyEntry`; o `InstallmentPlan` apenas **define o cronograma** (`StartReferenceMonth..EndReferenceMonth`, valor da parcela) e a dívida **à vista** projeta **exatamente uma** competência (ver RN16). Idempotência garantida pelos índices UNIQUE parciais (ver Índices) — nunca por dois mecanismos concorrentes criando a mesma linha.

---

## Entidades (responsabilidade + campos)

> Tipos: `Guid` PK gerado no banco · dinheiro `numeric(18,2)` · datas de calendário `DateOnly` (`DATE`) · timestamps de auditoria `timestamptz`. Entidades **ricas** (setters privados, factories/guards) — não anêmicas.

### User
Dono dos dados (raiz multi-tenant). — `Id, Name, Email (único), PasswordHash, CreatedAt, UpdatedAt`.

### Category
Classificação de lançamentos. — `Id, UserId, Name, Type {Income|Expense}, Color, Icon, IsActive, CreatedAt, UpdatedAt`.

### IncomeSource
Origem estrutural de receita. — `Id, UserId, CategoryId, Description, DefaultAmount, CompetenceDay (1..31), RecurrenceType {OneTime|Monthly}, StartDate, EndDate?, IsActive, Notes?, CreatedAt, UpdatedAt`.

### ExpenseSource
Origem estrutural de despesa recorrente. — `Id, UserId, CategoryId, Description, ExpenseKind {Fixed|Variable}, DefaultAmount, DueDay? (1..31), DueMonthOffset (0|+1, default 0), RecurrenceType {Monthly} (MVP; `OneTime` é futuro), StartDate, EndDate?, IsActive, Notes?, CreatedAt, UpdatedAt`. *(sem `Installment` — ver decisão 1)* `Variable` **gera competência mensal** com `DefaultAmount` como **estimativa** que o usuário ajusta (comportamento real, não flag decorativa); `DueMonthOffset=+1` cobre a conta que **pertence** ao mês da competência mas **vence** no mês seguinte (competência ≠ vencimento — RN01).

### Creditor
A quem se deve. — `Id, UserId, Name, Email?, Phone?, Notes?, IsActive, CreatedAt, UpdatedAt`.

### Debt
Obrigação a um credor (agregado dono da parcela). — `Id, UserId, CreditorId? (nullable), CategoryId?, Description, TotalAmount, InterestRate (numeric(5,2), informativo no MVP), DueDate?, StartDate, Status {Active|Paid|Cancelled} (`Overdue` **derivado**, não persistido), Priority {Low|Medium|High|Urgent}, Notes?, CreatedAt, UpdatedAt`. Pago/Falta **derivados** dos Payments das competências (sem coluna gravável, sem `RowVersion` — vêm do read-model). Tem 0..1 `InstallmentPlan`. **À vista** (sem plano) projeta exatamente uma competência (ver RN16 / decisão 10).

### InstallmentPlan
Lógica de parcelamento de uma dívida. — `Id, DebtId (1:1), TotalAmount, TotalInstallments, InstallmentAmount (com ajuste de centavos na última), StartReferenceMonth, EndReferenceMonth, IsCompleted, CreatedAt, UpdatedAt`. `CurrentInstallment` **derivado** (não materializado). UNIQUE `(DebtId)`.

### MonthlyEntry  *(coração)*
Materialização mensal (snapshot do previsto). — `Id, UserId, EntryType {Income|Expense}, SourceType {IncomeSource|ExpenseSource|Debt|Manual}, Nature {Fixed|Variable|Installment|Debt|Manual} (materializada na geração — dimensão das quebras, RN17), IncomeSourceId?, ExpenseSourceId?, DebtId?, InstallmentPlanId?, CategoryId, ReferenceMonth (DateOnly), Description, AmountExpected, Status {Pending|PartiallyPaid|Paid|Cancelled} (`Overdue` **derivado** em consulta, não persistido), DueDate?, OccurredAt?, InstallmentNumber?, InstallmentTotal?, Notes?, RowVersion (xmin), CreatedAt, UpdatedAt`. `AmountPaid` = **derivado** de `soma(Payments)` (sem coluna gravável — read-model; `RowVersion` protege as transições de status da competência).

### Payment
Pagamento real (fonte única do realizado). — `Id, UserId, MonthlyEntryId, Amount, PaymentDate (DateOnly), PaymentMethod {PaymentMethod}, Notes?, CreatedAt`.

### Settings
Preferências por usuário (aplicadas de fato). — `UserId (PK), Currency, DateFormat, DefaultPaymentMethod, Theme, LowBalanceThresholdPct (default 10), DueSoonDays (default 7), UpdatedAt`. Os dois limiares alimentam os alertas do dashboard (CA060).

### MonthlySummary  *(read model — não é tabela; é projeção/consulta agregada)*
`UserId, ReferenceMonth, TotalIncomeExpected/Received, TotalExpenseExpected/Paid, TotalCommitted (fixas+parcelas+dívidas), TotalVariable, TotalPaid, TotalPending, ExpectedBalance, ActualBalance, AvailableToSpend, contagens por status, quebras por categoria / `Nature` / forma de pagamento`. Calculado no Postgres (GROUP BY), não em memória. *(As quebras usam a dimensão `Nature` materializada em `MonthlyEntry`, não `ExpenseKind` — RN17.)*

---

## Relacionamentos
- **User** 1:N Category, IncomeSource, ExpenseSource, Creditor, Debt, MonthlyEntry, Payment · 1:1 Settings.
- **Category** 1:N IncomeSource, ExpenseSource, Debt, MonthlyEntry — *delete* `RESTRICT` (não apaga categoria em uso).
- **Creditor** 1:N Debt — *delete* `RESTRICT` + **soft-delete** (`IsActive`); nunca cascata cega (erro do B).
- **Debt** 1:1 InstallmentPlan · 1:N MonthlyEntry.
- **IncomeSource / ExpenseSource** 1:N MonthlyEntry (FK `RESTRICT` + **soft-delete** `IsActive` na origem — preserva histórico **sem violar** o CHECK de exatamente-uma-fonte; hard-delete só reatribuindo `SourceType=Manual` e zerando a FK na mesma transação). *(Inativar ≠ apagar: inativar mexe só em `IsActive` e não dispara delete.)*
- **MonthlyEntry** 1:N Payment (delete `CASCADE`).

### Índices (performance — ver `06`)
- `MonthlyEntry (UserId, ReferenceMonth)` composto · `(UserId, Status)` · `(UserId, DueDate) WHERE Status IN (Pending, PartiallyPaid)` (vencidas + alerta de vencimento próximo) · `(DebtId)` · `(IncomeSourceId)` · `(ExpenseSourceId)`.
- `Payment (UserId, MonthlyEntryId)` · `(MonthlyEntryId)`.
- `Debt (UserId, Status)` · `(UserId, Priority)` · `(CreditorId)`.
- `Category (UserId)` · `Creditor (UserId)`.
- UNIQUE: `User(Email)`, `InstallmentPlan(DebtId)`. **Idempotência da geração** por índices UNIQUE **parciais** por `SourceType` (uma UNIQUE comum com FKs nullable não fecha — NULLs são distintos entre si):
  - `(UserId, IncomeSourceId, ReferenceMonth) WHERE SourceType='IncomeSource'`
  - `(UserId, ExpenseSourceId, ReferenceMonth) WHERE SourceType='ExpenseSource'`
  - `(UserId, DebtId, InstallmentNumber, ReferenceMonth) WHERE SourceType='Debt' AND InstallmentNumber IS NOT NULL` *(parcelada)*
  - `(UserId, DebtId, ReferenceMonth) WHERE SourceType='Debt' AND InstallmentNumber IS NULL` *(à vista — RN16)*
  - **`Manual` não tem unicidade** (dois avulsos no mesmo mês são legítimos).

---

## Regras de estado (testáveis)

### `EntryStatus` (MonthlyEntry)
**Status persistido** ∈ {Pending, PartiallyPaid, Paid, Cancelled} — derivado dos Payments, nunca escrito à mão (exceto `Cancelled`):
- `soma(Payments) ≥ AmountExpected` → **Paid**
- `0 < soma(Payments) < AmountExpected` → **PartiallyPaid**
- `soma(Payments) = 0` → **Pending**
- cancelado manualmente → **Cancelled** (bloqueia novos pagamentos)

**`Overdue` é derivado em consulta, NÃO persistido:** um lançamento é *vencido* quando `Status ∈ {Pending, PartiallyPaid}` **e** `DueDate < hoje`. Assim a precedência fica resolvida (parcial-e-vencido = `PartiallyPaid` no banco + flag *vencido* na leitura) e não há status stale nem dependência de job para gravar `Overdue`.

### `Debt.Status` — derivado do plano/competências (não só da amostra já materializada)
- **Parcelada:** `Paid` **só** quando `InstallmentPlan.IsCompleted` (todas as `TotalInstallments` quitadas) — **nunca** marcar quitada só porque as competências **já geradas** foram pagas (uma 12× com 4 parcelas pagas ainda deve **2.400**).
- **À vista:** `Paid` quando a única competência projetável está quitada.
- cancelada manualmente → **Cancelled**.
- caso contrário → **Active**.
- **`Overdue`** (derivado em consulta, não persistido): existe competência da dívida com `Status ∈ {Pending, PartiallyPaid}` e `DueDate < hoje`.

### Transições
- **Overdue** é sempre **derivado em consulta** de `DueDate` (não é coluna) — nunca por edição manual nem por job que grava status (erro do B).
- Pagamento que quita → recalcula status na **mesma transação**. Estorno (delete de Payment) → status **regride** coerentemente.

---

## Regras de negócio (RN) — catálogo rastreável
- **RN01 — Competência ≠ vencimento ≠ pagamento.** Relatórios usam `ReferenceMonth`; vencido usa `DueDate`; realizado usa `Payment.PaymentDate`.
- **RN02 — Previsto é snapshot.** `AmountExpected` fixa o previsto do mês; realizado vem dos `Payments` (não sobrescreve o previsto).
- **RN03 — Exatamente-uma-fonte.** `MonthlyEntry` tem exatamente uma de {IncomeSourceId, ExpenseSourceId, DebtId} preenchida conforme `SourceType`, ou nenhuma se `Manual` (CHECK no banco).
- **RN04 — Geração idempotente.** Gerar o mês N vezes não duplica (UNIQUE por origem+competência+parcela).
- **RN05 — Status derivado.** `EntryStatus` e `Debt.Status` calculados pela regra acima; nunca gravados manualmente (exceto `Cancelled`).
- **RN06 — Guarda de superpagamento.** No MVP, pagamento cuja soma exceda `AmountExpected` é **bloqueado** (409/ProblemDetails); *falta* nunca fica negativa. Excedente/crédito fica para o [[08-features-futuras]].
- **RN07 — Pagamento atômico.** Inserir Payment + recalcular status/derivados ocorre em uma transação.
- **RN08 — Estorno.** Remover um Payment recalcula status e derivados (permite corrigir erro).
- **RN09 — Ajuste de centavos.** `InstallmentAmount = round(Total/N)`; a **última** parcela absorve a diferença para fechar exatamente `TotalAmount` (sem drift).
- **RN10 — Parcela ↔ competência.** Cada parcela vira **uma** `MonthlyEntry` (`InstallmentNumber`/`InstallmentTotal`), na competência do mês correspondente.
- **RN11 — Categoria em uso não se apaga.** `RESTRICT`; alternativa é inativar.
- **RN12 — Credor com dívidas não se apaga.** `RESTRICT` + soft-delete; histórico preservado.
- **RN13 — Isolamento por usuário.** Toda query filtra por `UserId` autenticado; toda mutação verifica posse (sem IDOR).
- **RN14 — Dinheiro em `numeric`.** Nenhuma aritmética monetária fora de `decimal`/SQL.
- **RN15 — Juros informativo (MVP).** `Debt.InterestRate` é exibido e não recalcula o total automaticamente (sem feature morta — o campo só existe porque é mostrado).
- **RN16 — Dívida à vista projeta UMA competência.** Dívida sem `InstallmentPlan` gera **exatamente uma** `MonthlyEntry` (`SourceType=Debt`, `InstallmentNumber` nulo, `AmountExpected=TotalAmount`) na competência do `DueDate` (fallback `StartDate`); **não** é regenerada em outros meses enquanto `Active` (idempotência pela UNIQUE parcial de dívida-sem-parcela) — senão inflaria o comprometido e o disponível.
- **RN17 — `Nature` materializada.** Todo `MonthlyEntry` grava `Nature` na geração (derivada de `SourceType`+`ExpenseKind`): é a **dimensão única das quebras** do resumo/dashboard (não usar `ExpenseKind`, que só existe na origem e exigiria join, quebrando o snapshot).

---

## Anti-patterns (o que NÃO fazer) — herdado das dívidas dos dois
- ❌ **Tabela única de "transações"** misturando receita/despesa/parcela/dívida/status. *(vira caos — erro clássico já apontado na doc do A)*
- ❌ **Calcular tudo em tempo de consulta** (status, parcela, mês). → snapshot em `MonthlyEntry`.
- ❌ **Parcelamento sem entidade própria.** → `InstallmentPlan`.
- ❌ **Confundir vencimento com competência.** → RN01.
- ❌ **`paid_amount` gravável pelo cliente + soma de pagamentos** (dupla verdade do B). → derivado/um dono transacional.
- ❌ **3 FKs nullable sem discriminador nem CHECK** (A). → `SourceType` + CHECK.
- ❌ **Cascade destrutivo cego** (apagar User/Creditor destrói histórico). → RESTRICT + soft-delete.
- ❌ **Dinheiro em float / data de competência com hora+fuso.** → `numeric(18,2)` + `DateOnly`.
- ❌ **Campo/flag decorativo** (juros/prioridade/recorrência que não fazem nada — B). → só existe o que tem comportamento (RN15).

## Ver também
- [[06-arquitetura]] · [[07-exemplo-uso]] · [[04-criterios-aceite]]
