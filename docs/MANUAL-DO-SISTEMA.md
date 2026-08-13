# Manual do MyFinance — guia de uso

Tutorial de uso do sistema, em linguagem simples. **Documento vivo:** cresce a cada fase entregue. Seções marcadas com ⬜ ("Em breve") ainda não existem no app — ficam listadas para você saber o que vem e em qual fase.

> Este é o manual **completo e sempre atual**. (No cérebro/Obsidian há um resumo em `projetos/MyFinance/manual-do-sistema.md` que aponta para cá.)

---

## O que é o MyFinance
Um controle financeiro pessoal que responde, de forma clara:
- **Quanto ainda posso gastar com segurança este mês?** (renda − o que já está comprometido − o que já gastei)
- **A quem eu devo, quanto, com que urgência e até quando?** — com visão **em parcelas** (a parcela do mês), não só o saldo total.

Ele funde a ideia de "montar o mês antes de ele acontecer" com um painel de dívidas por credor.

## Antes de começar (ambiente de desenvolvimento)
Enquanto o produto não está publicado, você o usa localmente. Para deixar tudo no ar (banco + API + app), siga a seção **"O que deixar ligado"** do [COMO-TESTAR.md](COMO-TESTAR.md). Depois, abra **http://localhost:4200**.

---

## 1. Criar sua conta  *(disponível — Fase 1)*
1. Abra o app (http://localhost:4200). Você verá a tela de **Entrar**.
2. Clique em **"Criar conta"**.
3. Preencha:
   - **Nome** — como você quer ser chamado (ex.: `Rafael`).
   - **E-mail** — seu e-mail (ex.: `rafael@example.com`). É o seu login e é único.
   - **Senha** — mínimo **8 caracteres** (ex.: `senha1234`).
4. Clique **"Criar conta"**. Pronto: você entra direto e vê a tela inicial com **"Olá, Rafael"**.

> Sua senha é guardada de forma segura (hash Argon2id) — o sistema nunca armazena a senha em texto.

## 2. Entrar (login)  *(disponível — Fase 1)*
1. Na tela **Entrar**, informe **e-mail** e **senha**.
2. Clique **"Entrar"**. Você vai para a tela inicial.
3. Se errar a senha, aparece **"E-mail ou senha inválidos."** — tente de novo ou crie uma conta.

## 3. Sair (logout)  *(disponível — Fase 1)*
No topo da tela, clique **"Sair"**. Você volta para a tela de login e sua sessão é encerrada.

> **Seus dados são só seus:** cada usuário acessa apenas os próprios dados. Ao sair, as telas ficam protegidas até você entrar de novo.

## 4. Navegar pelo app  *(disponível — Fase 2)*
Depois de entrar, você vê o menu com quatro itens:
- **Início** — atalhos para os cadastros (e, mais adiante, o resumo do mês).
- **Categorias** — os "assuntos" do seu dinheiro.
- **Receitas** — o que entra todo mês.
- **Despesas fixas** — o que sai todo mês.

No **celular** o menu fica atrás do botão **☰** no topo; no computador ele fica fixo à esquerda.

## 5. Categorias  *(disponível — Fase 2)*
Categorias organizam seu dinheiro por assunto (Moradia, Salário, Lazer…). **Comece por elas** — receitas e despesas precisam de uma categoria.

**Criar:** *Categorias* → **"Nova categoria"** → preencha:
- **Nome** — ex.: `Moradia`.
- **Tipo** — **Despesa** (dinheiro que sai) ou **Receita** (dinheiro que entra). O tipo define onde a categoria vai aparecer depois.
- **Cor** — usada nos gráficos do painel (Fase 4).
- **Ícone** — opcional, um nome curto (ex.: `home`).

Clique **Salvar**: ela aparece na lista.

**Editar:** clique **Editar** na linha da categoria. Aqui também existe a chave **"Categoria ativa"** — desmarcar **inativa** a categoria (ela para de ser oferecida em novos cadastros, mas o histórico continua intacto).

**Excluir:** clique **Excluir**. Se a categoria **estiver em uso** por alguma receita ou despesa, o sistema **não deixa excluir** e explica o porquê — o correto nesse caso é **inativar**. Isso protege seu histórico de ficar órfão.

## 6. Receitas  *(disponível — Fase 2)*
São suas **fontes de renda** recorrentes: salário, aluguel recebido, freelas.

*Receitas* → **"Nova receita"**:
- **Descrição** — ex.: `Salário CLT`.
- **Categoria** — só aparecem categorias do tipo **Receita**.
- **Valor padrão** — quanto costuma entrar (ex.: `7500,55`). É só o padrão: nas próximas fases cada mês pode ter um valor diferente do previsto.
- **Dia da competência** — o dia do mês em que essa renda conta (ex.: `5`).
- **Recorrência** — **Mensal** (todo mês) ou **Única**.
- **Início** / **Fim (opcional)** — a partir de quando vale e, se for o caso, até quando.
- **Observações** — livre.

Na lista você vê o valor já formatado em real (**R$ 7.500,55**) e o selo **Ativa/Inativa**.

## 7. Despesas fixas  *(disponível — Fase 2)*
São as contas que se repetem: aluguel, internet, escola, academia.

*Despesas fixas* → **"Nova despesa fixa"**. Os campos são parecidos com os de receita, com dois específicos:
- **Dia do vencimento** — o dia em que a conta vence (pode deixar em branco se não houver data fixa).
- **Mês do vencimento** — **"No mês da competência"** (o normal) ou **"No mês seguinte"**. Use o segundo quando a conta *pertence* a um mês mas só vence no outro (ex.: a fatura de janeiro que você paga em fevereiro).

> **Dívidas parceladas não entram aqui.** Elas ganham tela própria na Fase 5, com credor, prioridade, quanto já foi pago e quanto falta.

---

## Em breve (o que cada próxima fase adiciona ao seu uso)
- ⬜ **Lançar gastos e registrar pagamentos** *(Fase 3)* — anotar um gasto do dia a dia em poucos toques; marcar contas como pagas (total ou parcial), com estorno.
- ⬜ **Resumo do mês e painel visual** *(Fase 4)* — ver receitas, comprometido, pago, pendente, **quanto ainda posso gastar**, e gráficos por categoria.
- ⬜ **Dívidas e parcelas** *(Fase 5)* — cadastrar a quem você deve, com prioridade e vencimento; ver **quanto já pagou e quanto falta**, e a **parcela do mês** de cada parcelamento.
- ⬜ **Importar dados e preferências** *(Fase 6)* — importar despesas/dívidas de um CSV; definir moeda, formato de data e forma de pagamento padrão; alertas de vencimento.
- ⬜ **Acabamento** *(Fase 7)* — visual refinado, tema claro/escuro, uso confortável no celular.

*(Este manual será atualizado com o passo a passo de cada item acima assim que a fase correspondente for concluída.)*
