---
tipo: requisitos-exemplo
projeto: MyFinance
artefato: 07-exemplo-uso
atualizado: 2026-07-17
---

# Artefato 7 — Exemplo de Uso (validação do modelo) — MyFinance

Cenário real percorrido ponta a ponta **antes de codar**, para provar que o modelo do [[05-modelagem-dominio]] responde às **6 perguntas** do MVP — incluindo a **ênfase do dono** (dívida por credor + visão em parcelas). Se não responder, voltar ao `05`.

## Cenário — Rafael, abril/2026
**Receitas:** salário R$ 5.000 (competência dia 5) · freela R$ 800 (competência dia 20).
**Despesas fixas:** aluguel R$ 1.500 (vence 10) · internet R$ 120 (vence 15) · academia R$ 100 (vence 8).
**Gasto avulso:** mercado (ao longo do mês).
**Dívidas:**
- **Notebook** na *Loja X*, R$ 3.600 em **12×** (parcela 4/12 cai em abril) — dívida **parcelada**.
- **Empréstimo** com *Tio João*, R$ 2.000, vencimento 30/04, **prioridade Alta**, sem parcelas (pagamento ad-hoc).

## Como fica no sistema (entidades)
1. **Fontes:** `IncomeSource(Salário, 5000, CompetenceDay=5)`, `IncomeSource(Freela, 800, dia 20)`, `ExpenseSource(Aluguel, Fixed, 1500, DueDay=10)`, `ExpenseSource(Internet, Fixed, 120, DueDay=15)`, `ExpenseSource(Academia, Fixed, 100, DueDay=8)`.
2. **Credores:** `Creditor(Loja X)`, `Creditor(Tio João)`.
3. **Dívida parcelada:** `Debt(Notebook, Creditor=Loja X, Total=3600, Priority=Medium)` com `InstallmentPlan(Total=3600, N=12, InstallmentAmount=300, Start=jan/2026)`.
   - `300 × 12 = 3600` exato; se fosse R$ 3.601/12, 11 parcelas de 300,08 e a **última** de 300,12 (ajuste de centavos, RN09).
4. **Dívida à vista:** `Debt(Empréstimo, Creditor=Tio João, Total=2000, DueDate=2026-04-30, Priority=High)` — sem `InstallmentPlan`.
5. **Gerar abril/2026 (HU35):** o motor cria as `MonthlyEntry` de competência `2026-04-01`:
   - Income: Salário 5.000 (Due 05), Freela 800 (Due 20).
   - Expense/fixa: Aluguel 1.500 (Due 10), Internet 120 (Due 15), Academia 100 (Due 08).
   - Expense/Debt (parcela): Notebook **4/12** = 300 (SourceType=Debt, InstallmentNumber=4/12).
   - Expense/Debt (à vista): Empréstimo 2.000 (Due 30, SourceType=Debt).
   - Gerar de novo **não duplica** (RN04).

## O que o sistema mostra (previsto)
- **Receita prevista:** 5.800.
- **Comprometido** (fixas + parcela + dívida do mês): 1.500 + 120 + 100 + 300 + 2.000 = **4.020**.
- **Variável previsto:** 0 (mercado é avulso, entra ao gastar).
- **Saldo previsto:** 5.800 − 4.020 = **1.780** → base do "quanto posso gastar".

## Quando os eventos acontecem (realizado × previsto)
- 05/04 salário cai → `Payment(5000, Pix)` na competência do salário → **Paid**.
- 20/04 freela recebido → `Payment(800, Transfer)` na competência do freela → **Paid** (receita realizada do mês = 5.800).
- 08/04 academia paga (Pix), 10/04 aluguel pago (Boleto), 15/04 internet paga (Débito) → **Paid**.
- 12/04 parcela do notebook paga (Cartão) → competência **Paid**; a **dívida Notebook** passa a `pago=1.200 / falta=2.400 (8/12 restantes)`.
- Mercado: 2 pagamentos (R$ 420 + R$ 380 = 800) num lançamento **Manual** → variável realizado 800.
- Empréstimo do Tio João: pago **parcial** R$ 1.200/2.000 (Pix) → competência **PartiallyPaid**; dívida `pago=1.200 / falta=800`, status **Active**; como `DueDate=30/04`, se virar o mês sem quitar → **Overdue** (RN01, derivado).
- **Estorno de teste:** um pagamento do mercado lançado errado é estornado (HU41) → variável recalcula, sem resíduo.

## Fechamento — o que o sistema responde (as 6 perguntas)
1. **Quanto recebi?** 5.800 previsto / 5.800 recebido.
2. **Quanto comprometido?** 4.020.
3. **Quanto gastei/pago × pendente?** Pago: fixas 1.720 + parcela 300 + empréstimo 1.200 + mercado 800 = **4.020**; Pendente: empréstimo **800**.
4. **Quanto posso gastar?** Disponível = 5.800 − 4.020 − 800 (variável já gasto) = **980** (e cai conforme novos avulsos).
5. **A quem devo / falta / urgência / vencimento?** *Loja X*: falta **2.400** (8/12). *Tio João*: falta **800**, **prioridade Alta**, vence **30/04** → aparece no topo do painel e como alerta.
6. **Parcela do mês?** Notebook **4/12 (R$ 300)** — mostra parcela, não o saldo total; restam **8 parcelas**.

**Quebras do fechamento:** por **categoria** (Moradia, Utilidades, Saúde, Alimentação, Dívidas), por **tipo** (fixa/variável/parcela/dívida), por **forma de pagamento** (Pix/Boleto/Débito/Cartão).

> ✔️ O modelo `Source/Debt → MonthlyEntry → Payment` responde tudo, **sem** tabela única de transações, **sem** recomputar em consulta, com dívida parcelada e à vista no mesmo painel e **visão em parcelas**. Modelo validado.

## Ver também
- [[08-features-futuras]] · [[05-modelagem-dominio]]
