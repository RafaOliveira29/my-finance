---
tipo: requisitos-epicos
projeto: MyFinance
artefato: 02-epicos
atualizado: 2026-07-17
---

# Artefato 2 — Épicos — MyFinance

Reconcilia os épicos do Projeto A (10) com o domínio de dívidas/credores e import do Projeto B. Cada épico marca a **origem** (A / B / novo).

---

### Épico E1 — Autenticação e Acesso  *(origem: A E9)*
- **Objetivo:** cada usuário acessa **apenas** seus próprios dados, com login seguro.
- **Por que existe:** o sistema lida com dados financeiros sensíveis; sem isso, multi-tenant é fantasma (erro dos dois projetos).
- **Cobre:** cadastro, login, sessão (JWT), logout, isolamento por `UserId`.
- **Resultado:** ambiente pessoal, privado e seguro desde o dia 1.

### Épico E2 — Receitas  *(origem: A E1)*
- **Objetivo:** registrar as **entradas** do mês (o lado que o Projeto B não tinha).
- **Cobre:** fontes de receita recorrentes (`IncomeSource`), valor padrão, dia de competência, recorrência mensal; CRUD e total do mês.
- **Resultado:** o usuário sabe **quanto entra** — base para "quanto posso gastar".

### Épico E3 — Despesas Fixas  *(origem: A E2)*
- **Objetivo:** cadastrar compromissos recorrentes (aluguel, internet, academia).
- **Cobre:** fontes de despesa fixa (`ExpenseSource`, `ExpenseKind=Fixed`), valor padrão, **dia de vencimento**, recorrência; total previsto do mês.
- **Resultado:** clareza de **quanto da renda já está comprometido**.

### Épico E4 — Despesas Variáveis / Lançamento Avulso  *(origem: A E3 + B Expenses)*
- **Objetivo:** registrar o gasto do dia a dia, **rápido**.
- **Cobre:** lançamento manual direto (`MonthlyEntry` `SourceType=Manual`) com valor, data, categoria, forma de pagamento, tags e observação — **sem** precisar criar uma "fonte".
- **Resultado:** despesa avulsa entra em ≤ 3 campos e impacta o resumo na hora.

### Épico E5 — Dívidas e Credores  *(origem: B — ênfase do dono)*
- **Objetivo:** saber **a quem devo, quanto, com que urgência e até quando**.
- **Cobre:** CRUD de credor; dívida com total, juros (informativo), vencimento, **prioridade**, status, categoria e observações; **painel por credor** com pago × falta; agregados por credor.
- **Resultado:** visão consolidada e priorizada das dívidas — o coração do lado "B".

### Épico E6 — Parcelamentos  *(origem: A E4, agora sob a Dívida)*
- **Objetivo:** acompanhar compromissos distribuídos em vários meses **em parcelas** (não só saldo total — ênfase do dono).
- **Cobre:** plano de parcelamento de uma dívida (total, nº de parcelas, valor da parcela com **ajuste de centavos na última**), **projeção de uma competência por parcela**, "parcela X/N", quantas faltam, quando termina.
- **Resultado:** cada mês mostra **a parcela daquele mês**; a dívida sabe quantas restam.

### Épico E7 — Competência Mensal & Geração  *(origem: A — núcleo; técnico)*
- **Objetivo:** "montar o mês antes dele acontecer".
- **Cobre:** geração **idempotente** dos `MonthlyEntry` previstos do mês a partir das fontes ativas, parcelas e dívidas; distinção competência × vencimento × pagamento (snapshot do previsto).
- **Resultado:** o mês existe como dado consultável (previsto), não como cálculo inferido a cada tela.

### Épico E8 — Pagamentos  *(origem: A E5 + B DebtPayments)*
- **Objetivo:** registrar o **realizado** contra o previsto.
- **Cobre:** pagamento parcial/total contra a competência, com valor, data e forma; recálculo de status; **guarda de superpagamento**; **estorno** que faz o status regredir; atomicidade.
- **Resultado:** previsto × realizado sempre coerente; saldo de dívida confiável.

### Épico E9 — Resumo Financeiro Mensal  *(origem: A E6 — o coração)*
- **Objetivo:** consolidar o mês e responder "quanto posso gastar".
- **Cobre:** totais de receita, comprometido (fixas + parcelas + dívidas), variáveis, pago, pendente, **saldo previsto × real**, "disponível com segurança"; quebras por categoria / tipo de despesa / forma de pagamento.
- **Resultado:** fechamento claro do mês. *Sem isto, cadastro é só armazenamento.*

### Épico E10 — Dashboard  *(origem: A E7 + B gráficos)*
- **Objetivo:** situação financeira num relance.
- **Cobre:** cards/KPIs (receitas, despesas, pendências, saldo disponível, total devido), **gráficos** (recharts→ngx-charts/equivalente) por categoria/tipo, **alertas** de saldo baixo/negativo e vencimentos próximos, atalho de painel de dívidas.
- **Resultado:** decisão sem abrir várias telas.

### Épico E11 — Classificação e Organização  *(origem: A E8)*
- **Objetivo:** organizar e localizar lançamentos.
- **Cobre:** categorias (cor/ícone), forma de pagamento, **filtros** por período/categoria/tipo, observações, diferenciação fixa/variável/parcela/dívida.
- **Resultado:** informação achável e bem classificada.

### Épico E12 — Importação CSV  *(origem: B — fast-follow)*
- **Objetivo:** trazer dados existentes sem digitar tudo.
- **Cobre:** upload CSV, mapeamento de colunas (PT/EN), pré-visualização, **import em bulk/transação**, relatório de erro **por linha**, parser robusto (aceita `;`, aspas, `\r\n`).
- **Resultado:** carga inicial rápida e segura.

### Épico E13 — Configurações  *(origem: B)*
- **Objetivo:** preferências por usuário **que realmente valem**.
- **Cobre:** moeda, formato de data, forma de pagamento padrão — aplicadas em toda a UI/cálculo (em B eram cosméticas).
- **Resultado:** experiência coerente com a preferência do usuário.

### Épico E14 — Base Técnica e Qualidade  *(origem: A E10)*
- **Objetivo:** estrutura que aguenta evoluir, sem dívida herdada.
- **Cobre:** Clean Architecture pragmática, contrato de API tipado, validação declarativa, tratamento de erro padronizado, constraints/índices no banco, **testes dos fluxos de dinheiro**.
- **Resultado:** código novo indistinguível de sênior; evolução sem reescrever o domínio.

---

## Priorização (MoSCoW + levas)

**Must (MVP) — em 3 levas:**
- **1ª leva (núcleo do fluxo mensal):** E14 (base), E1 (auth), E7 (competência/geração), E2 (receitas), E3 (fixas), E4 (avulsas), E8 (pagamentos, **incl. estorno**), E9 (resumo), E10 (dashboard), **E11 core (categorias, observações, forma de pagamento)**.
- **2ª leva (dívidas — ênfase):** E5 (dívidas/credores), E6 (parcelamentos), **E11 filtros/diferenciação por natureza**.
- **3ª leva (conveniência):** E13 (configurações), E12 (import CSV), alertas de vencimento.

**Should (pós-MVP próximo):** refinamento de import (mais formatos), comparativo simples entre meses.

**Won't (agora) — vai para o `08`:** Budget/metas, cartão de crédito, reembolso/despesa compartilhada, OFX/Open Finance, multiusuário/SaaS.

> **Nota sênior:** o **Resumo Mensal (E9)** e a **Competência/Geração (E7)** são o coração — decidem se o app é "controle" ou só "cadastro". As **Dívidas (E5/E6)** são a ênfase explícita do dono e entram cedo, não no fim.

## Ver também
- [[03-historias]] · [[01-produto]]
