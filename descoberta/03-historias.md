---
tipo: requisitos-historias
projeto: MyFinance
artefato: 03-historias
atualizado: 2026-07-17
---

# Artefato 3 — Histórias de Usuário e Técnicas — MyFinance

Backlog **unificado** (Projeto A + Projeto B), renumerado de forma limpa e rastreável. `(A HUxx)` = herdada do Projeto A; `(B)` = domínio de dívidas/import; `(novo)` = lacuna corrigida. Cada HU respeita INVEST; o "pronto" mora no [[04-criterios-aceite]].

## Épico E1 — Autenticação e Acesso
- **HU01** (E1) — Como usuário, quero **criar uma conta**, para acessar meus dados com segurança. *(A HU42)*
- **HU02** (E1) — Como usuário, quero **fazer login**, para entrar no meu ambiente pessoal. *(A HU43)*
- **HU03** (E1) — Como usuário, quero **permanecer autenticado**, para não logar a todo momento. *(A HU44)*
- **HU04** (E1) — Como usuário, quero **sair com segurança**, para proteger meus dados. *(A HU45)*
- **HU05** (E1) — Como usuário, quero **acessar somente meus dados**, para garantir privacidade. *(A HU46)*

## Épico E2 — Receitas
- **HU06** (E2) — Como usuário, quero **cadastrar uma fonte de receita** (descrição, valor, dia de competência, recorrência), para registrar entradas. *(A HU01)*
- **HU07** (E2) — Como usuário, quero **listar minhas receitas**, para acompanhar o que entra. *(A HU02)*
- **HU08** (E2) — Como usuário, quero **editar uma receita**, para corrigir dados. *(A HU03)*
- **HU09** (E2) — Como usuário, quero **excluir/inativar uma receita**, para remover lançamentos indevidos. *(A HU04)*
- **HU10** (E2) — Como usuário, quero **ver o total de receitas do mês**, para saber quanto entrou. *(A HU05)*

## Épico E3 — Despesas Fixas
- **HU11** (E3) — Como usuário, quero **cadastrar uma despesa fixa** (descrição, valor, categoria, vencimento), para registrar compromissos recorrentes. *(A HU06)*
- **HU12** (E3) — Como usuário, quero **listar despesas fixas**, para ver minhas contas recorrentes. *(A HU07)*
- **HU13** (E3) — Como usuário, quero **editar uma despesa fixa**, para manter atualizada. *(A HU08)*
- **HU14** (E3) — Como usuário, quero **excluir/inativar uma despesa fixa**, para não impactar meses futuros. *(A HU09)*
- **HU15** (E3) — Como usuário, quero **ver o total previsto de fixas no mês**, para saber quanto já está comprometido. *(A HU10)*

## Épico E4 — Despesas Variáveis / Avulsas
- **HU16** (E4) — Como usuário, quero **registrar um gasto avulso** (descrição, valor, data, categoria) rápido, para controlar o dia a dia. *(A HU11)*
- **HU17** (E4) — Como usuário, quero **informar a forma de pagamento** do gasto, para diferenciar Pix/cartão/dinheiro. *(A HU12)*
- **HU18** (E4) — Como usuário, quero **ver gastos variáveis por período**, para saber para onde vai meu dinheiro. *(A HU13)*
- **HU19** (E4) — Como usuário, quero **editar um gasto avulso**, para corrigir lançamentos. *(A HU14)*
- **HU20** (E4) — Como usuário, quero **excluir um gasto avulso**, para remover registros errados. *(A HU15)*

## Épico E5 — Dívidas e Credores *(ênfase do dono)*
- **HU21** (E5) — Como usuário, quero **cadastrar um credor** (nome, contato), para saber **a quem** devo. *(B)*
- **HU22** (E5) — Como usuário, quero **listar credores com o total devido a cada um**, para ver minha exposição por credor. *(B)*
- **HU23** (E5) — Como usuário, quero **editar/inativar um credor**, para manter a lista limpa sem perder histórico. *(B)*
- **HU24** (E5) — Como usuário, quero **cadastrar uma dívida** (descrição, valor total, juros, vencimento, prioridade, credor, categoria), para registrar o que devo. *(B)*
- **HU25** (E5) — Como usuário, quero **listar minhas dívidas mostrando pago × falta e prioridade**, para saber o que ainda devo. *(B)*
- **HU26** (E5) — Como usuário, quero **abrir o painel de uma dívida** com o quê / para quem / quanto / prioridade / vencimento / **quanto já paguei / quanto falta**, para ter a situação completa. *(B)*
- **HU27** (E5) — Como usuário, quero **editar uma dívida**, para corrigir dados. *(B)*
- **HU28** (E5) — Como usuário, quero **encerrar/cancelar uma dívida**, para tirá-la do que está em aberto. *(B)*
- **HU29** (E5) — Como usuário, quero **ordenar/priorizar dívidas por urgência e vencimento**, para decidir o que pagar primeiro. *(B)*

## Épico E6 — Parcelamentos
- **HU30** (E6) — Como usuário, quero **parcelar uma dívida** (valor total, nº de parcelas, mês de início), para distribuir em vários meses. *(A HU16)*
- **HU31** (E6) — Como usuário, quero **ver os parcelamentos ativos**, para saber o que ainda impacta meu orçamento. *(A HU17)*
- **HU32** (E6) — Como usuário, quero **ver a parcela atual, o total e quantas faltam** (visão em parcelas, não só saldo total), para acompanhar o andamento. *(A HU18 — ênfase)*
- **HU33** (E6) — Como usuário, quero **editar um parcelamento**, para corrigir dados. *(A HU19)*
- **HU34** (E6) — Como usuário, quero **encerrar/inativar um parcelamento**, para impedir impacto após o fim. *(A HU20)*

## Épico E7 — Competência Mensal & Geração
- **HU35** (E7) — Como usuário, quero **montar o mês** (gerar os lançamentos previstos das fontes ativas, parcelas e dívidas), para ver o mês antes dele acontecer. *(novo — expõe o motor)*

## Épico E8 — Pagamentos
- **HU36** (E8) — Como usuário, quero **registrar um pagamento** (parcial ou total) de uma competência, para acompanhar o realizado. *(A HU21)*
- **HU37** (E8) — Como usuário, quero **informar data e forma do pagamento**, para manter o histórico fiel. *(A HU22)*
- **HU38** (E8) — Como usuário, quero **identificar o que está pendente**, para saber o que falta pagar. *(A HU23)*
- **HU39** (E8) — Como usuário, quero **identificar contas vencidas**, para priorizar atrasos. *(A HU24)*
- **HU40** (E8) — Como usuário, quero **ver o que já foi pago**, para acompanhar a execução do mês. *(A HU25)*
- **HU41** (E8) — Como usuário, quero **estornar um pagamento**, para corrigir um lançamento errado. *(novo — lacuna dos dois)*

## Épico E9 — Resumo Financeiro Mensal
- **HU42** (E9) — Como usuário, quero ver o **total comprometido** (fixas + parcelas + dívidas), para saber quanto da renda já está preso. *(A HU27)*
- **HU43** (E9) — Como usuário, quero ver o **total de gastos variáveis** do mês, para medir o impacto do consumo. *(A HU28)*
- **HU44** (E9) — Como usuário, quero ver **total pago e pendente**, para entender meu cenário. *(A HU29)*
- **HU45** (E9) — Como usuário, quero ver o **saldo restante do mês** (previsto × real), para saber quanto ainda tenho. *(A HU30)*
- **HU46** (E9) — Como usuário, quero ver **quanto posso gastar com segurança**, para não me enrolar. *(A HU31)*

## Épico E10 — Dashboard
- **HU47** (E10) — Como usuário, quero uma **tela inicial com o resumo do mês**, para entender minha situação rápido. *(A HU32)*
- **HU48** (E10) — Como usuário, quero **cards de receitas, despesas, pendências, saldo disponível e total devido**, para decidir sem abrir várias telas. *(A HU33 + B)*
- **HU49** (E10) — Como usuário, quero **gráficos de gastos por categoria/tipo**, para ver onde gasto mais. *(A HU34 + B recharts)*
- **HU50** (E10) — Como usuário, quero **alertas de saldo baixo/negativo e de vencimentos próximos**, para agir antes de piorar. *(A HU35)*
- **HU51** (E10) — Como usuário, quero **visualização clara no celular**, para usar no dia a dia. *(A HU36)*

## Épico E11 — Classificação e Organização
- **HU52** (E11) — Como usuário, quero **classificar lançamentos por categoria**, para organizar o controle. *(A HU37)*
- **HU53** (E11) — Como usuário, quero **definir a forma de pagamento** dos lançamentos, para entender como pago. *(A HU38)*
- **HU54** (E11) — Como usuário, quero **adicionar observações** em qualquer lançamento, para guardar contexto. *(A HU39)*
- **HU55** (E11) — Como usuário, quero **filtrar lançamentos por período, categoria e tipo**, para localizar informação. *(A HU40)*
- **HU56** (E11) — Como usuário, quero **diferenciar a natureza** dos lançamentos (fixa/variável/parcela/dívida/manual), para entender a natureza dos compromissos. *(A HU41)*
- **HU57** (E11) — Como usuário, quero **gerenciar categorias (CRUD) com cor e ícone**, para personalizar minha organização. *(novo + B)*

## Épico E12 — Importação CSV
- **HU58** (E12) — Como usuário, quero **importar um CSV** de despesas/dívidas mapeando as colunas e pré-visualizando, para não digitar tudo. *(B)*
- **HU59** (E12) — Como usuário, quero **ver um relatório de erro por linha** do import, para corrigir e reimportar. *(B)*

## Épico E13 — Configurações
- **HU60** (E13) — Como usuário, quero **configurar moeda, formato de data e forma de pagamento padrão**, para o app refletir minha preferência (de fato aplicada). *(B)*

## Épico E14 — Base Técnica e Qualidade (histórias técnicas)
- **HT01** (E14) — Como sistema, quero **estrutura em camadas (Clean pragmática)**, para manutenção e evolução. *(A HT01)*
- **HT02** (E14) — Como sistema, quero **persistência relacional com constraints (CHECK/UNIQUE/índices)**, para armazenamento confiável e consistente. *(A HT02, reforçada)*
- **HT03** (E14) — Como sistema, quero **endpoints padronizados e contrato de API tipado** (OpenAPI → modelos TS), para integração clara e sem `any`. *(A HT03, reforçada)*
- **HT04** (E14) — Como sistema, quero **validação de entrada declarativa** (FluentValidation/schema), para barrar dados inválidos com 400 claro. *(A HT04)*
- **HT05** (E14) — Como sistema, quero **tratamento de erro padronizado (middleware → ProblemDetails)**, para previsibilidade e nenhum erro silencioso. *(A HT05)*
- **HT06** (E14) — Como sistema, quero **regra de negócio separada da infraestrutura**, para domínio limpo e testável. *(A HT06)*
- **HT07** (E14) — Como sistema, quero **geração mensal idempotente**, para montar o mês N vezes sem duplicar. *(novo)*
- **HT08** (E14) — Como sistema, quero **pagamento atômico + concorrência otimista (RowVersion)**, para nunca dessincronizar saldo/status. *(novo)*
- **HT09** (E14) — Como sistema, quero **isolamento multi-tenant (global query filter + posse)**, para nenhum vazamento entre usuários. *(novo)*
- **HT10** (E14) — Como sistema, quero **testes automatizados dos fluxos de dinheiro**, para regressão visível. *(novo)*

---

## Priorização em levas (dentro do MVP)
- **1ª leva — núcleo do fluxo mensal:** HT01–HT06, HT08–HT10 · HU01–HU05 · HU06–HU20 · HU35 (+HT07) · HU36–HU41 (pagamentos **incl. estorno**) · HU42–HU51 · HU52–HU54, HU57.
- **2ª leva — dívidas & parcelas (ênfase):** HU21–HU34 · HU55–HU56 (filtros/diferenciação por natureza).
- **3ª leva — conveniência:** HU60 (settings) · HU58–HU59 (import) · HU50 refinado (alertas de vencimento).

> **Nota sênior:** não fazer tudo no primeiro ciclo. O núcleo (levas 1) prova o motor de competência e o dinheiro; a leva 2 entrega a ênfase de dívidas sobre a mesma fundação, sem retrabalho.

## Ver também
- [[04-criterios-aceite]] · [[02-epicos]]
