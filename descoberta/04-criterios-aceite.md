---
tipo: requisitos-criterios
projeto: MyFinance
artefato: 04-criterios-aceite
atualizado: 2026-07-17
---

# Artefato 4 — Critérios de Aceite — MyFinance

Gherkin, **IDs globais únicos** (`CA###`) — corrige o bug do pacote original do Projeto A, onde cada épico reiniciava em CA01–CA04 (colisão, perda de rastreabilidade). Cada CA aponta a **HU** que valida. Matriz de rastreabilidade e catálogo RF/RN ao final.

## Catálogo de Requisitos Funcionais (RF)
`RF-01` Autenticação · `RF-02` Isolamento por usuário · `RF-03` CRUD categorias · `RF-04` Fontes de receita · `RF-05` Fontes de despesa fixa · `RF-06` Lançamento avulso · `RF-07` CRUD credores · `RF-08` CRUD dívidas · `RF-09` Painel/priorização de dívidas · `RF-10` Parcelamento · `RF-11` Geração mensal (competência) · `RF-12` Pagamentos e estorno · `RF-13` Status derivado · `RF-14` Resumo mensal · `RF-15` Dashboard/gráficos/alertas · `RF-16` Filtros e observações · `RF-17` Import CSV · `RF-18` Configurações · `RF-19` Validação e erro padronizado · `RF-20` Contrato tipado/endpoints.
*(RN01–RN15: ver [[05-modelagem-dominio]].)*

---

## E1 — Autenticação
- **CA001 — Cadastro válido** (HU01): Dado o formulário de cadastro, Quando informo nome, e-mail único e senha válida, Então a conta é criada e a senha é persistida com **hash Argon2id** (nunca em texto).
- **CA002 — E-mail duplicado** (HU01): Dado um e-mail já cadastrado, Quando tento criar conta, Então o sistema impede e retorna **409** com mensagem clara.
- **CA003 — Login válido** (HU02): Dado credenciais corretas, Quando faço login, Então recebo um **JWT** válido e sou direcionado ao meu ambiente.
- **CA004 — Login inválido** (HU02): Dado senha incorreta, Quando faço login, Então recebo **401** e nenhuma sessão é criada.
- **CA005 — Sessão persistente** (HU03): Dado que estou logado, Quando recarrego o app dentro da validade do token, Então continuo autenticado sem novo login.
- **CA006 — Logout** (HU04): Dado que faço logout, Então o token é descartado e rotas protegidas exigem novo login.
- **CA007 — Isolamento de dados** (HU05): Dado que estou autenticado como usuário A, Quando requisito qualquer recurso de outro usuário (por Id), Então recebo **404/403** e nunca dados alheios (sem IDOR).

## E2 — Receitas
- **CA008 — Cadastro de receita** (HU06): Dado a tela de fonte de receita, Quando informo descrição, valor > 0, dia de competência (1–31) e recorrência, Então a fonte é salva.
- **CA009 — Campos obrigatórios** (HU06): Dado o cadastro, Quando salvo sem descrição/valor/data, Então o sistema impede e exibe **400** com os campos faltantes.
- **CA010 — Valor inválido** (HU06): Dado o cadastro, Quando o valor ≤ 0, Então o sistema impede e exibe erro.
- **CA011 — Listar receitas** (HU07): Dado receitas cadastradas, Quando abro a lista, Então vejo todas as minhas fontes de receita ativas.
- **CA012 — Editar receita** (HU08): Dado uma receita, Quando altero e salvo, Então os dados são atualizados e o previsto de meses **ainda não gerados** reflete a mudança.
- **CA013 — Inativar receita** (HU09): Dado uma receita ativa, Quando inativo, Então ela para de gerar competências futuras **sem** apagar o histórico já gerado.
- **CA014 — Total de receitas do mês** (HU10): Dado receitas do mês, Então o sistema exibe a soma correta das receitas previstas do mês.

## E3 — Despesas Fixas
- **CA015 — Cadastro de despesa fixa** (HU11): Dado a tela, Quando informo descrição, valor, categoria e dia de vencimento, Então a despesa fixa é registrada.
- **CA016 — Campos obrigatórios** (HU11): Dado o cadastro, Quando falto campo obrigatório, Então o sistema impede (400).
- **CA017 — Listagem** (HU12): Dado fixas cadastradas, Quando acesso a tela, Então vejo todas as contas recorrentes.
- **CA018 — Inativar recalcula meses futuros** (HU14): Dado uma fixa inativada, Então os cálculos de meses **ainda não gerados** deixam de considerá-la.
- **CA019 — Total previsto de fixas** (HU15): Dado fixas do mês, Então o sistema exibe o total previsto de despesas fixas.

## E4 — Despesas Variáveis / Avulsas
- **CA020 — Lançamento avulso rápido** (HU16): Dado a tela de gasto avulso, Quando informo valor, data e categoria, Então o gasto é salvo como `MonthlyEntry` `SourceType=Manual` em **≤ 3 campos essenciais**.
- **CA021 — Forma de pagamento** (HU17): Dado um gasto avulso, Quando informo a forma de pagamento, Então o método (Pix/Cartão/Débito/Dinheiro/…) é associado ao pagamento.
- **CA022 — Listagem por período** (HU18): Dado gastos registrados, Quando visualizo um mês, Então vejo os gastos daquele período.
- **CA023 — Recalcular totais** (HU19/HU20): Dado que edito/excluo um gasto, Então os totais do mês são recalculados na hora.

## E5 — Dívidas e Credores *(ênfase)*
- **CA024 — Cadastro de credor** (HU21): Dado a tela de credor, Quando informo nome (e contato opcional), Então o credor é salvo.
- **CA025 — Total devido por credor** (HU22): Dado credores com dívidas, Quando abro a lista de credores, Então vejo, por credor, **quanto ainda devo** (soma de falta das dívidas ativas).
- **CA026 — Inativar credor com dívida** (HU23): Dado um credor com dívidas, Quando tento excluir, Então o sistema **impede a exclusão** (RESTRICT) e permite **inativar** (soft-delete), preservando histórico.
- **CA027 — Cadastro de dívida** (HU24): Dado a tela de dívida, Quando informo descrição, valor total > 0, vencimento, **prioridade**, credor e categoria, Então a dívida é registrada com status **Active** e **falta = `TotalAmount`** (derivado, sem pagamentos ainda — não há coluna `paid` gravável).
- **CA028 — Painel da dívida** (HU26): Dado uma dívida, Quando abro o painel, Então vejo **descrição (o quê), credor (para quem), total (quanto), prioridade, vencimento, quanto já paguei e quanto falta** — com falta = total − soma dos pagamentos (derivado, RN02).
- **CA029 — Lista com pago × falta** (HU25): Dado dívidas cadastradas, Quando abro a lista, Então cada dívida mostra pago, falta e prioridade, e o status reflete a regra de `Debt.Status` (RN05).
- **CA030 — Priorização** (HU29): Dado dívidas com prioridades e vencimentos, Quando ordeno por urgência, Então a ordenação é **determinística**: `ORDER BY Priority DESC, DueDate ASC` (vencimento nulo por último) — ex.: uma *Urgent* que vence longe vem antes de uma *High*, e entre a mesma prioridade a de **vencimento mais próximo** aparece primeiro.
- **CA031 — Cancelar dívida** (HU28): Dado uma dívida ativa, Quando cancelo, Então o status vira **Cancelled**, as competências futuras dela deixam de compor o comprometido, e novos pagamentos ficam bloqueados.
- **CA032 — Juros é informativo** (HU24): Dado uma dívida com juros informado, Então o juros é **exibido** mas **não** recalcula o total automaticamente (RN15 — sem feature morta).

## E6 — Parcelamentos
- **CA033 — Cálculo da parcela + ajuste de centavos** (HU30): Dado um parcelamento de total `T` em `N` parcelas, Quando salvo, Então cada parcela = `round(T/N)` e a **última** absorve a diferença, somando **exatamente T** (RN09).
- **CA034 — Projeção em competências** (HU30): Dado um parcelamento a partir do mês `M`, Então o sistema cria **uma** `MonthlyEntry` por parcela, uma por mês, com `InstallmentNumber`/`InstallmentTotal` (RN10).
- **CA035 — Visão em parcelas** (HU32): Dado um parcelamento ativo, Quando abro os detalhes, Então vejo **parcela atual (X/N)**, total de parcelas e **quantas faltam** — e no mês vejo **a parcela daquele mês**, não o saldo total.
- **CA036 — Parcelamentos ativos** (HU31): Dado parcelamentos em andamento, Quando abro a lista, Então vejo os que ainda têm parcelas futuras.
- **CA037 — Encerramento não impacta o futuro** (HU34): Dado um parcelamento encerrado, Então ele não gera novas competências nem impacta meses futuros.

## E7 — Competência & Geração
- **CA038 — Montar o mês** (HU35): Dado fontes ativas, parcelas e dívidas, Quando gero o mês `M`, Então são criadas as `MonthlyEntry` previstas de `M` com `AmountExpected` snapshot e `Nature` materializada — receitas e fixas do mês, **parcela** de `M` de cada plano ativo (RN10) e **dívida à vista** como **uma** competência na competência do seu `DueDate` (RN16).
- **CA039 — Geração idempotente** (HU35 / HT07): Dado que já gerei o mês `M`, Quando gero `M` de novo, Então **nenhum lançamento é duplicado** (RN04) — inclusive a **dívida à vista** não gera uma segunda competência em outro mês (RN16). Garantido pelas UNIQUE parciais por `SourceType`.
- **CA040 — Competência ≠ vencimento** (HT / RN01): Dado uma `ExpenseSource` com `DueMonthOffset=+1`, Quando gero a competência `M`, Então o lançamento tem `ReferenceMonth=M` e `DueDate` no mês **seguinte** (`M+1`) — competência e vencimento em meses distintos, sem confundir o relatório do mês.

## E8 — Pagamentos
- **CA041 — Pagamento total** (HU36): Dado uma competência pendente de `AmountExpected`, Quando pago o valor cheio, Então cria-se um `Payment` e o status vira **Paid** na mesma transação (RN07).
- **CA042 — Pagamento parcial** (HU36): Dado uma competência de 100, Quando pago 40, Então o status vira **PartiallyPaid** e falta = 60.
- **CA043 — Data e forma** (HU37): Dado um pagamento, Quando informo data e forma, Então ambos são gravados no `Payment`.
- **CA044 — Guarda de superpagamento** (HU36 / RN06): Dado uma competência de 100 já com 80 pagos, Quando tento pagar 30, Então o sistema **impede** (retorna **409/ProblemDetails**) — falta nunca fica negativa.
- **CA045 — Pendentes** (HU38): Dado competências não pagas, Então elas aparecem como **Pending** na lista de pendências.
- **CA046 — Vencidas** (HU39 / RN01): Dado uma competência com `Status ∈ {Pending, PartiallyPaid}` e `DueDate < hoje`, Então ela é **exibida como vencida** (derivado em consulta, sem coluna `Overdue`) — uma parcial-e-vencida permanece `PartiallyPaid` no banco e aparece com a flag de vencida.
- **CA047 — Já pagas** (HU40): Dado competências quitadas, Então aparecem como **Paid** com data de pagamento.
- **CA048 — Estorno** (HU41 / RN08): Dado uma competência **Paid**, Quando estorno o pagamento, Então o `Payment` é removido, o status **regride** (Pending/PartiallyPaid/Overdue conforme regra) e os derivados/saldo da dívida recalculam na mesma transação.
- **CA049 — Concorrência otimista** (HT08): Dado dois pagamentos concorrentes na mesma competência, Quando o segundo usa uma versão desatualizada (`RowVersion`), Então ele falha com conflito controlado (não sobrescreve).

## E9 — Resumo Financeiro Mensal
- **CA050 — Total de receitas** (HU10/HU42): Dado receitas do mês, Então o resumo mostra o total de receitas previstas.
- **CA051 — Total comprometido** (HU42): Dado fixas + parcelas + dívidas do mês, Então o resumo mostra o **comprometido** (RN01, competência).
- **CA052 — Total variável** (HU43): Dado gastos avulsos do mês, Então o resumo mostra o total de variáveis.
- **CA053 — Pago × pendente** (HU44): Dado a competência do mês, Então o resumo mostra total pago e total pendente coerentes com os `Payments`.
- **CA054 — Saldo previsto × real** (HU45): Dado receitas e despesas, Então o resumo calcula saldo **previsto** (receita − previsto) e **real** (recebido − pago).
- **CA055 — Disponível com segurança** (HU46): Dado o mês, Então "quanto posso gastar" = **ReceitaPrevista − Comprometido − GastoVariávelRealizado**, onde Comprometido = previsto de `Nature ∈ {Fixed, Installment, Debt}` e GastoVariávelRealizado = Payments de `SourceType=Manual`; **fixas/parcelas/dívidas não são contadas duas vezes** (não usar soma total de Payments).
- **CA056 — Quebras** (E9): Dado o mês, Então o resumo detalha por **categoria**, **`Nature`** (Fixed/Variable/Installment/Debt/Manual) e **forma de pagamento** — todas materializadas no lançamento (sem join à origem, RN17).

## E10 — Dashboard
- **CA057 — Resumo na home** (HU47): Dado que abro o app, Então a tela inicial mostra o resumo do mês atual.
- **CA058 — Cards principais** (HU48): Dado a home, Então vejo cards de receitas, despesas, pendências, **saldo disponível** e **total devido**.
- **CA059 — Gráfico por categoria** (HU49): Dado gastos do mês, Então vejo um gráfico de gastos por **categoria** e por **`Nature`** com valores corretos (agregação no banco).
- **CA060 — Alertas visuais** (HU50): Dado `AvailableToSpend < 0` **ou** `AvailableToSpend < LowBalanceThresholdPct%` da receita prevista (Settings, default 10%) **ou** competência a vencer em ≤ `DueSoonDays` dias (default 7), Então o dashboard exibe **alerta visual**. *(No MVP só o alerta visual; notificação ativa/push e fechamento automático do mês são futuros — [[08-features-futuras]].)*
- **CA061 — Responsividade** (HU51): Dado que acesso pelo celular, Então o layout se adapta **sem quebra de 320 px a 1440 px** (RNF03).

## E11 — Classificação
- **CA062 — Associar categoria** (HU52): Dado um lançamento, Então posso associar uma categoria compatível com o tipo (Income/Expense).
- **CA063 — Observações** (HU54): Dado um lançamento, Então posso adicionar observação opcional.
- **CA064 — Filtros** (HU55): Dado lançamentos cadastrados, Então consigo filtrar por período, categoria e tipo, combinados.
- **CA065 — CRUD de categoria com cor/ícone** (HU57): Dado a tela de categorias, Quando crio/edito com cor e ícone, Então a categoria é salva; categoria **em uso não pode ser apagada** (RN11), só inativada.

## E12 — Importação CSV
- **CA066 — Mapeamento e prévia** (HU58): Dado um CSV, Quando mapeio as colunas (aceitando cabeçalhos PT/EN) e confirmo, Então vejo uma **pré-visualização** antes de importar.
- **CA067 — Import atômico** (HU58 / RNF12): Dado um CSV válido de até 1.000 linhas, Quando importo, Então tudo é gravado em **uma transação** (ou nada) — sem N+1.
- **CA068 — Erro por linha** (HU59): Dado um CSV com linhas inválidas, Quando importo, Então recebo um **relatório por linha** indicando o erro, e as linhas válidas podem ser confirmadas.
- **CA069 — Parser robusto** (HU58): Dado um CSV com delimitador `;`, aspas e quebras `\r\n`, Quando importo, Então o parser lê corretamente (não só `,`/`\n`).

## E13 — Configurações
- **CA070 — Preferências aplicadas** (HU60): Dado que defino moeda, formato de data e forma de pagamento padrão, Então **toda** a UI/cálculo passa a usá-las (não apenas salva).

## E14 — Base Técnica
- **CA071 — Erro padronizado** (HT05 / RF19): Dado dados inválidos enviados à API, Então recebo **ProblemDetails** (400/404/409) — nunca 500 cru; erros internos são logados e retornam resposta controlada.
- **CA072 — Endpoints respondem** (HT03): Dado a API no ar, Então os endpoints de receitas, despesas, dívidas, parcelas, pagamentos, resumo e dashboard respondem conforme contrato OpenAPI.
- **CA073 — Contrato tipado** (HT03): Dado o contrato OpenAPI, Então os modelos TS do front são **gerados** dele (fonte única, zero `any` na borda de dados).
- **CA074 — Money em numeric** (HT02 / RN14): Dado qualquer cálculo monetário, Então ele ocorre em `decimal`/SQL `numeric(18,2)`, nunca em float.
- **CA075 — Cobertura de dinheiro** (HT10 / RNF08): Dado os fluxos de geração/pagamento/parcela/resumo, Então há testes automatizados cobrindo-os (alvo ≥ 80% Domain+Application).

## Cobertura de edição e classificação (HUs antes sem CA)
- **CA076 — Editar despesa fixa** (HU13): Dado uma despesa fixa, Quando edito e salvo, Então os dados atualizam e **só os meses ainda não gerados** refletem a mudança (meses já materializados preservam o snapshot, RN02).
- **CA077 — Editar dívida** (HU27): Dado uma dívida **sem pagamentos**, Quando altero total/vencimento/prioridade, Então atualiza e as competências projetadas ainda pendentes são recalculadas; **com pagamentos existentes**, alterar o total exige confirmação e **nunca apaga `Payments`**.
- **CA078 — Editar parcelamento** (HU33): Dado um parcelamento **sem parcelas pagas**, Quando altero nº de parcelas/valor, Então o cronograma é recalculado (ajuste de centavos na última, RN09); **parcelas já pagas** bloqueiam reduzir o total abaixo do já quitado.
- **CA079 — Forma de pagamento aplicada** (HU53): Dado um lançamento, Quando registro o pagamento com uma forma, Então a forma fica associada ao `Payment` e aparece na quebra por forma de pagamento (CA056).
- **CA080 — Diferenciar natureza** (HU56): Dado lançamentos de naturezas diferentes, Quando abro a lista, Então cada um exibe sua `Nature` (fixa/variável/parcela/dívida/manual) de forma distinguível.

---

## Matriz de rastreabilidade (Épico → HU → CA → RF/RN)
| Épico | HU | CA | RF / RN |
|---|---|---|---|
| E1 | HU01–HU05 | CA001–CA007 | RF-01, RF-02 / RN13 |
| E2 | HU06–HU10 | CA008–CA014 | RF-04 / RN02, RN14 |
| E3 | HU11–HU15 | CA015–CA019, **CA076** (HU13) | RF-05 / RN01, RN02 |
| E4 | HU16–HU20 | CA020–CA023 | RF-06 / RN03, RN14 |
| E5 | HU21–HU29 | CA024–CA032, **CA077** (HU27) | RF-07, RF-08, RF-09 / RN05, RN12, RN15 |
| E6 | HU30–HU34 | CA033–CA037, **CA078** (HU33) | RF-10 / RN09, RN10, RN16 |
| E7 | HU35 | CA038–CA040 | RF-11 / RN01, RN04, RN16, RN17 |
| E8 | HU36–HU41 | CA041–CA049 | RF-12, RF-13 / RN05, RN06, RN07, RN08 |
| E9 | HU42–HU46 | CA050–CA056 | RF-14 / RN01, RN02, RN17 |
| E10 | HU47–HU51 | CA057–CA061 | RF-15 / RN17 |
| E11 | HU52–HU57 | CA062–CA065, **CA079** (HU53), **CA080** (HU56) | RF-16, RF-03 / RN11 |
| E12 | HU58–HU59 | CA066–CA069 | RF-17 / — |
| E13 | HU60 | CA070 | RF-18 / — |
| E14 | HT01–HT10 | CA071–CA075 | RF-19, RF-20 / RN07, RN14 |

## Ver também
- [[05-modelagem-dominio]] · [[03-historias]]
