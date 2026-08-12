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
Na tela inicial, clique **"Sair"**. Você volta para a tela de login e sua sessão é encerrada.

> **Seus dados são só seus:** cada usuário acessa apenas os próprios dados. Ao sair, as telas ficam protegidas até você entrar de novo.

---

## Em breve (o que cada próxima fase adiciona ao seu uso)
- ⬜ **Categorias, receitas e despesas fixas** *(Fase 2)* — organizar de onde vem e para onde vai o dinheiro (com cor/ícone), cadastrar salário/renda e contas fixas.
- ⬜ **Lançar gastos e registrar pagamentos** *(Fase 3)* — anotar um gasto do dia a dia em poucos toques; marcar contas como pagas (total ou parcial), com estorno.
- ⬜ **Resumo do mês e painel visual** *(Fase 4)* — ver receitas, comprometido, pago, pendente, **quanto ainda posso gastar**, e gráficos por categoria.
- ⬜ **Dívidas e parcelas** *(Fase 5)* — cadastrar a quem você deve, com prioridade e vencimento; ver **quanto já pagou e quanto falta**, e a **parcela do mês** de cada parcelamento.
- ⬜ **Importar dados e preferências** *(Fase 6)* — importar despesas/dívidas de um CSV; definir moeda, formato de data e forma de pagamento padrão; alertas de vencimento.
- ⬜ **Acabamento** *(Fase 7)* — visual refinado, tema claro/escuro, uso confortável no celular.

*(Este manual será atualizado com o passo a passo de cada item acima assim que a fase correspondente for concluída.)*
