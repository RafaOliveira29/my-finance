---
tipo: requisitos-futuro
projeto: MyFinance
artefato: 08-features-futuras
atualizado: 2026-07-17
---

# Artefato 8 — Features Futuras (roadmap + guarda de escopo) — MyFinance

Registra o que fica **para depois** e — mais importante — o que **preservar agora** para não travar o futuro. Herdado do backlog dos dois projetos (features futuras de A + capacidades não-MVP de B).

## Roadmap (fora do MVP)
1. **Orçamento / Metas por categoria (`Budget`)** — teto previsto × gasto por categoria/mês. *(entidade morta em B + feature futura de A — implementar de fato: rota + UI + alerta ao estourar.)*
2. **Notificação ativa e fechamento automático do mês** — *(distinto do alerta visual do MVP, CA060)* notificação **push/e-mail** de contas a vencer e **job** que fecha/gera o mês automaticamente.
3. **Comparativo entre meses e projeção futura** — tendência, média móvel, "como estará em 3 meses".
4. **Gestão de cartão de crédito** — `CreditCardAccount` (limite total do banco × **limite pessoal**), comprometimento e **competência da fatura**, "quanto ainda é recomendável gastar no cartão".
5. **Lançamento com reembolso esperado / despesa compartilhada** — separar valor lançado × custo real do usuário × a receber de terceiros; impacto no limite e no orçamento.
6. **Importação de extrato bancário (OFX/PDF)** e, depois, **Open Finance**.
7. **Recorrência não-mensal** (semanal/anual) como motor automático.
8. **Multiusuário / compartilhamento familiar** e, por fim, versão **SaaS**.
9. **Relatórios avançados / exportação** (PDF/planilha).

## Guarda de conceitos (crucial — não misturar agora para caber depois)
- **Cartão de crédito ≠ forma de pagamento ≠ categoria.** No MVP, "Cartão" é só `PaymentMethod`. O **instrumento** cartão (com limite e fatura) é entidade futura (`CreditCardAccount`); manter os conceitos separados desde já evita reescrever o pagamento depois.
- **Orçamento (`Budget`) ≠ despesa.** Meta é um teto planejado por categoria/mês, não um lançamento. Já modelado com UNIQUE `(UserId, CategoryId, Month, Year)` para encaixar sem migração dolorosa.
- **Reembolso não é "aumentar limite manualmente".** Modelar como *lançamento com reembolso esperado* (3 valores) — mais fiel e encaixável.
- **Juros informativo (hoje) → juros calculado (futuro).** `Debt.InterestRate` já existe e é exibido; quando virar cálculo, o campo já está lá — sem feature morta no meio.
- **Competência mensal já é a base para comparativo entre meses** — como o mês é snapshot (`MonthlyEntry`), a série temporal para o comparativo já nasce pronta.
- **`SourceType=Manual` e `RecurrenceType`** já preveem gasto avulso e recorrência — a recorrência não-mensal entra como novo valor do enum sem mexer no núcleo.

## Ideias avaliadas
| Ideia | Faz sentido? | Como modelar no futuro |
|---|---|---|
| Metas por categoria | Sim | `Budget` (já reservado) + alerta ao aproximar/estourar |
| Cartão com limite/fatura | Sim | `CreditCardAccount` + `CardStatementEntry`; forma de pagamento aponta ao cartão |
| Reembolso/dividir gasto | Sim | lançamento com `ExpectedReimbursement` (valor lançado × custo real × a receber) |
| Tabela única de transações | **Não** | anti-pattern — manter `Source/Debt → MonthlyEntry → Payment` |
| Recalcular tudo em consulta | **Não** | manter snapshot mensal materializado |

## Ver também
- [[../DOC]] (índice/documentação viva do pacote) · [[documentacao-viva]] · [[05-modelagem-dominio]]
