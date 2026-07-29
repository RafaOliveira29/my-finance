---
tipo: requisitos-produto
projeto: MyFinance
artefato: 01-produto
atualizado: 2026-07-17
---

# Artefato 1 — Documento de Produto (PRD) — MyFinance

> Produto novo, **brownfield**: funde o **Projeto A** (competência mensal: renda, despesa, parcelamento, previsto×realizado) com o **Projeto B** (dívidas/credores, pagamentos de dívida, dashboard com gráficos, import CSV). Nome de trabalho **MyFinance** (provisório — alternativas: *Fôlego*, *Quitado*, *Saldo*).

## 1. Visão
Organizador de vida financeira pessoal orientado à **competência mensal**, que responde de forma clara e previsível **"quanto ainda posso gastar com segurança este mês"** — e, no mesmo lugar, mostra **o que devo, para quem, quanto, com que urgência e até quando**, com visão **em parcelas**, não só em saldo total. Menos "contábil", mais "visão prática do mês".

## 2. Problema (dores atuais, com evidência dos dois projetos)
- A pessoa não sabe, num relance, **quanto do salário já está comprometido** antes do mês acontecer (o Projeto A nasceu exatamente para isso).
- **Dívidas ficam invisíveis ou espalhadas**: para quem devo, quanto falta, o que é mais urgente, o que vence primeiro (dor central do Projeto B).
- Ver só o **saldo total de uma dívida** esconde o que importa no mês: **a parcela deste mês** (força do Projeto A que o B não tinha).
- Lançar gasto do dia a dia é chato → abandono. Precisa ser **rápido e mobile**.
- Nas versões anteriores, **previsto e realizado se confundiam**, relatórios mensais saíam errados e não havia isolamento por usuário nem integridade real de dados (ver **Anti-patterns** em [[05-modelagem-dominio]] e o catálogo de dívidas do estudo — Etapa 1).

## 3. Objetivo do produto
O usuário passa a conseguir, num único app:
1. **Montar o mês antes dele acontecer** (renda + despesas fixas + parcelas + dívidas projetadas) e acompanhar **previsto × realizado**.
2. **Gerir dívidas por credor** com prioridade, vencimento, quanto já pagou e **quanto falta**, inclusive **parceladas** (parcela do mês).
3. **Fechar o mês** com um resumo consolidado e visual (por categoria, tipo de despesa e forma de pagamento).

## 4. Objetivo do MVP — as 6 perguntas que o MVP responde
1. Quanto **recebi / vou receber** este mês?
2. Quanto já está **comprometido** (fixas + parcelas + dívidas do mês)?
3. Quanto **já gastei/paguei** e quanto está **pendente**?
4. **Quanto ainda posso gastar** com segurança?
5. **A quem devo**, quanto falta em cada dívida, o que é **mais urgente** e o que **vence primeiro**?
6. Em cada parcelamento/dívida, **qual a parcela deste mês** e quantas ainda faltam?

## 5. Público-alvo
- **Inicial:** o próprio dono e pessoas físicas que querem controle mensal prático e visão de dívidas — sem planilha, sem app "de banco".
- **Futuro:** casais/famílias (compartilhamento), depois SaaS multiusuário.

## 6. Premissas
- Uso **majoritariamente mobile**, lançamentos rápidos e frequentes.
- **Cadastro manual** é a fonte primária no MVP (sem integração bancária/Open Finance ainda).
- Volume típico por usuário: dezenas a **poucas centenas de lançamentos por mês**; alvo de projeto até **1.000/mês** sem degradar.
- Um único banco **PostgreSQL**; front **Angular 21+**; back **C# / ASP.NET Core**.
- Valor monetário sempre em **uma moeda por usuário** (BRL por padrão), configurável.

## 7. Escopo do MVP

### DENTRO
- Autenticação real (cadastro, login, sessão, logout) e **isolamento total por usuário**.
- **Categorias** (receita/despesa) com cor/ícone — CRUD.
- **Fontes de receita** (recorrentes) e **fontes de despesa fixa** (recorrentes).
- **Lançamento avulso** de despesa variável do dia a dia (rápido).
- **Competência mensal**: geração dos lançamentos previstos do mês (idempotente) e acompanhamento previsto × realizado.
- **Dívidas e credores**: CRUD de credor; dívida com total, juros (informativo), vencimento, **prioridade**, status, observações — vinculada a credor e categoria.
- **Parcelamento** de dívida: total, nº de parcelas, valor da parcela (com ajuste de centavos na última), **projeção de uma competência por parcela**, visão "parcela X/N do mês".
- **Pagamentos** (parciais e totais) contra a competência, com data e forma de pagamento, recálculo de status, **guarda de superpagamento** e **estorno**.
- **Resumo financeiro mensal** (receitas, comprometido, variáveis, pago, pendente, **saldo previsto × real**, "quanto posso gastar") com quebras por categoria / tipo / forma de pagamento.
- **Dashboard** com KPIs, **gráficos** (por categoria/tipo) e **alertas** de saldo baixo/negativo.
- **Painel de dívidas** (ênfase do dono): por credor — o quê / para quem / quanto / prioridade / vencimento / pago / **falta** / parcela do mês.
- **Filtros** por período, categoria e tipo; **observações** em qualquer lançamento.
- **Configurações** por usuário (moeda, formato de data, forma de pagamento padrão) **efetivamente aplicadas**.
- **Importação CSV** de despesas/dívidas (mapeamento de colunas PT/EN, relatório de erro por linha) — em **bulk/transação** *(fast-follow: última leva do MVP)*.

### FORA (explícito — vai para o `08`)
- Integração bancária / Open Finance / leitura de fatura / importação **OFX/PDF**.
- **Orçamento/metas por categoria** (Budget) — conceito preservado no modelo, **não** implementado agora.
- **Cartão de crédito** como instrumento (limite total × limite pessoal, fatura, competência da fatura).
- **Lançamento com reembolso esperado / despesa compartilhada**.
- **Comparativo entre meses** e projeção futura; relatórios avançados.
- **Multiusuário / compartilhamento familiar** e versão **SaaS**.
- Recorrência não-mensal (semanal/anual) como motor automático — só mensal no MVP.

## 8. Requisitos não funcionais (mensuráveis)
| ID | RNF | Meta mensurável |
|---|---|---|
| RNF01 | Desempenho de leitura | Dashboard e resumo do mês carregam em **< 1,5 s** com até **1.000 lançamentos/mês**; agregação feita no **Postgres (GROUP BY)**, nunca em memória. |
| RNF02 | Desempenho de API | **p95 < 300 ms** em leituras e **< 500 ms** em escritas, sob 50 req/s. |
| RNF03 | Responsividade | Layout **sem quebra de 320 px a 1440 px** (testar 320/375/768/1024/1440); mobile-first. |
| RNF04 | Segurança | Senha com **Argon2id**; **JWT**; **isolamento por `UserId`** (global query filter + verificação de posse); HTTPS; segredos fora do repositório. |
| RNF05 | Precisão monetária | **`numeric(18,2)`** fim a fim; **zero** aritmética de dinheiro em float; regra de arredondamento definida (última parcela absorve os centavos). |
| RNF06 | Registro rápido | Registrar um gasto avulso em **≤ 3 campos essenciais / ≤ 3 toques**. |
| RNF07 | Integridade | Pagamento **atômico** (transação); constraints no banco (**CHECK/UNIQUE**); **optimistic concurrency** (`xmin`/RowVersion) nas entidades de dinheiro. |
| RNF08 | Confiabilidade | Fluxos de dinheiro (geração mensal, pagamento, parcela, resumo) cobertos por **testes automatizados** desde o início; alvo ≥ **80%** no Domain+Application. |
| RNF09 | Observabilidade | Erros tratados por **middleware global → ProblemDetails**; logs estruturados; nenhum erro silencioso no front (feedback em todo submit/delete). |
| RNF10 | UX/Acessibilidade | **Tema claro/escuro**; **i18n pt-BR** (preparado para en); foco e feedback visíveis; contraste AA. |
| RNF11 | Idempotência | Gerar o mês **N vezes não duplica** lançamentos. |
| RNF12 | Importação | CSV de até **1.000 linhas** em **transação única**, com relatório de erro por linha, em **< 5 s**. |

## 9. Riscos → mitigação
- **Cadastro manual chato → abandono** → fluxo de lançamento rápido, mobile-first, defaults inteligentes (categoria/forma de pagamento padrão).
- **Escopo grande (fusão de 2 apps) → MVP não termina** → levas priorizadas (núcleo → dívidas/parcelas → conveniência); dívidas entram cedo por serem ênfase.
- **Reconciliar dois modelos → inconsistência** → glossário e modelo únicos no `05`, validados pelo walkthrough do `07` antes de codar.
- **Dinheiro/datas com bug sutil (fuso, centavo)** → `numeric` + `DateOnly`, testes de parcela/pagamento, RNF05.
- **Regressão invisível** → testes dos fluxos de dinheiro desde a 1ª leva (RNF08).

## 10. Critérios de sucesso do MVP
- As **6 perguntas** do item 4 respondidas corretamente no walkthrough do `07` com dados reais.
- Um mês montado, pago parcialmente e fechado **sem inconsistência** entre previsto, realizado e saldo de dívida.
- Painel de dívidas responde **para quem / quanto / prioridade / vencimento / pago / falta / parcela do mês** em ≤ 2 telas.
- RNF01–RNF07 atendidos em ambiente de teste.

## 11. Diretriz técnica
- **Stack:** Angular 21+ (standalone, signals, zoneless, OnPush) · ASP.NET Core (.NET) Web API (C#) · PostgreSQL (EF Core + Npgsql).
- **Arquitetura:** **Clean Architecture pragmática, orientada a performance** (4 camadas) com **leitura via projeção/CQRS-lite** para os caminhos quentes (dashboard/resumo/listas). Detalhe e justificativa de performance no `06`.

## Ver também
- [[02-epicos]] · [[05-modelagem-dominio]] · [[06-arquitetura]]
