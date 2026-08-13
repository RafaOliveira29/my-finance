# Como testar o MyFinance

Guia vivo de testes. **Regra do projeto:** ao concluir cada fase (ou sub-fase), adiciona-se aqui um **Mapa de teste** — como um humano confirma, na prática, que aquela fase funciona. Assim qualquer pessoa (ou um chat futuro) valida o sistema sem adivinhar.

> Ambiente destas instruções: **Windows + PowerShell**. Caminho do repo: `...\projetos com claude\my-finance`.

---

## 1. Pré-requisitos — o que instalar

| Ferramenta | Para quê | Versão mínima | Como obter | Precisa? |
|---|---|---|---|---|
| **.NET SDK 10** | rodar o backend, os testes e as migrations | 10.x | https://dotnet.microsoft.com/download | **Sim** |
| **Node.js + npm** | rodar o frontend | Node ≥ 20 (rec. 22 LTS) | https://nodejs.org | **Sim** |
| **Docker Desktop** | subir o PostgreSQL local | 24+ | https://www.docker.com/products/docker-desktop | **Sim** |
| **dotnet-ef** | criar/aplicar migrations | 10.x | `dotnet tool install -g dotnet-ef` | Só p/ migrations |
| **REST Client** | testar a API direto (opcional) | — | extensão *REST Client* no VS Code, ou Insomnia/Postman | Opcional |

**Não precisa:** Angular CLI global (o `npm start` usa a CLI local do projeto) nem PostgreSQL instalado na máquina (o Docker sobe um container).

> Na sua máquina já estão OK: .NET 10, Node, Docker e `dotnet-ef`. Para um PC novo, instale os três primeiros da tabela.

## 2. O que deixar ligado durante o teste

Três coisas rodando ao mesmo tempo (cada uma num terminal):

1. **Docker Desktop** aberto + o banco:
   ```powershell
   cd "...\projetos com claude\my-finance"
   docker compose up -d db          # sobe o container myfinance-db (porta 5433)
   ```
2. **Backend** (API em http://localhost:5080):
   ```powershell
   cd "...\my-finance\backend"
   dotnet run --project src/MyFinance.API
   ```
3. **Frontend** (app em http://localhost:4200):
   ```powershell
   cd "...\my-finance\frontend"
   npm start
   ```

**Para parar:** `Ctrl+C` nos terminais do back/front; `docker compose stop db` para o banco (os dados ficam salvos no volume).

## 3. Setup inicial (só na primeira vez, ou após novas migrations)
```powershell
cd "...\my-finance"
docker compose up -d db
cd backend
dotnet ef database update --project src/MyFinance.Infrastructure --startup-project src/MyFinance.API
```
Isso cria/atualiza as tabelas no banco (hoje: `users`, `categories`, `income_sources`, `expense_sources`).

---

## 4. Mapa de teste por fase

### ✅ Fase 0 — Fundação
*Objetivo: a base compila, sobe e conversa com o banco.*
1. `cd backend` → `dotnet build MyFinance.slnx` → **esperado:** `0 Erro(s)`, `0 Aviso(s)`.
2. `dotnet test MyFinance.slnx` → **esperado:** todos os testes verdes.
3. `docker compose up -d db` (na raiz) e `docker inspect -f "{{.State.Health.Status}}" myfinance-db` → **esperado:** `healthy`.
4. `dotnet run --project src/MyFinance.API` e abrir no navegador:
   - http://localhost:5080/health → **esperado:** `{"status":"ok"}`
   - http://localhost:5080/openapi/v1.json → **esperado:** documento OpenAPI (JSON).

### ✅ Fase 1 — Auth & tenant (cadastro, login, área protegida)
*Objetivo: criar conta, entrar, acessar área logada e ver os erros corretos.*

**A) Testes automatizados (rápido; NÃO precisa de banco nem servidores):**
```powershell
cd backend
dotnet test MyFinance.slnx
```
**Esperado:** 14 testes verdes (regras da entidade `User` + `AuthService`: cadastro, e-mail duplicado, login, senha errada, validação).

**B) Teste manual pela interface (o fluxo real do usuário):**
Pré: banco + backend + frontend ligados (seção 2).
1. Abra **http://localhost:4200** → você é levado para **/login** (a raiz é protegida). ✔️ prova o *guard*.
2. Clique **"Criar conta"**, preencha nome, e-mail e senha (mín. 8 caracteres) → **"Criar conta"** → cai na tela inicial mostrando **"Olá, &lt;seu nome&gt;"**. ✔️ cadastro + sessão.
3. Clique **"Sair"** → volta para **/login**. ✔️ logout.
4. Faça **login** com o mesmo e-mail/senha → volta para a tela inicial. ✔️ login.
5. **Erros esperados:** senha com menos de 8 no cadastro → o formulário barra; login com senha errada → mensagem *"E-mail ou senha inválidos."*.
6. **Sessão/isolamento:** recarregue a página logado → continua logado; depois de **Sair**, tente abrir `http://localhost:4200/` → é redirecionado ao login. ✔️ rota protegida.

**C) Teste no nível da API (opcional):**
Abra `backend/src/MyFinance.API/MyFinance.API.http` no VS Code (extensão *REST Client*) e clique **"Send Request"** em cada bloco, de cima para baixo (o `/me` reaproveita o token do login).
- **Esperado:** register e login → **200** com `accessToken`; `/me` com token → **200** com seus dados; `/me` sem token → **401**; e-mail duplicado → **409**; dados inválidos → **400** (com a lista de erros).

Alternativa em PowerShell (sem REST Client):
```powershell
$b = "http://localhost:5080/api/auth"
$r = Invoke-RestMethod "$b/register" -Method Post -ContentType 'application/json' -Body '{"name":"Rafael","email":"rafael@example.com","password":"senha1234"}'
$me = Invoke-RestMethod "$b/me" -Headers @{ Authorization = "Bearer $($r.accessToken)" }
$me   # deve mostrar id, name, email
```

### ✅ Fase 2 — Cadastros base (categorias, receitas, despesas fixas)
*Objetivo: cadastrar de onde vem e para onde vai o dinheiro — e provar que **um usuário nunca vê o dado do outro**.*

**A) Testes automatizados (rápido; NÃO precisa de banco nem servidores):**
```powershell
cd backend
dotnet test MyFinance.slnx      # esperado: 30 testes verdes (17 domínio + 13 aplicação)
cd ../frontend
npm test                        # esperado: 14 testes verdes
```

**B) Teste manual pela interface (o fluxo real do usuário):**
Pré: banco + backend + frontend ligados (seção 2), e uma conta criada (Fase 2 exige estar logado).
1. Entre no app → a tela inicial agora tem **menu** (Início, Categorias, Receitas, Despesas fixas).
2. **Categorias → "Nova categoria"**: nome `Moradia`, tipo **Despesa**, escolha uma cor → **Salvar**. Ela aparece na lista. ✔️ criar.
3. Crie também `Salário` do tipo **Receita** (você vai precisar dela no passo 4).
4. **Receitas → "Nova receita"**: descrição `Salário CLT`, valor `7500,55`, dia da competência `5` → **Salvar**. Na lista o valor aparece como **R$ 7.500,55**. ✔️ dinheiro correto.
5. **Despesas fixas → "Nova despesa fixa"**: descrição `Aluguel`, valor `2200`, dia do vencimento `10`, mês do vencimento **"No mês seguinte"** → **Salvar**. Na lista aparece *"dia 10 do mês seguinte"*.
6. **Editar**: em Categorias, clique **Editar** numa categoria, mude o nome e salve → a lista reflete a mudança. ✔️ editar.
7. **Regra da categoria em uso:** tente **Excluir** a categoria `Salário` (usada pela receita) → aparece o aviso *"Esta categoria está em uso e não pode ser excluída. Inative-a…"* e ela **continua na lista**. ✔️ RN11/CA065.
8. **Inativar em vez de excluir:** edite essa categoria, desmarque **"Categoria ativa"**, salve → a lista mostra o selo **Inativa**.
9. **Excluir de verdade:** exclua uma categoria que não está em uso → ela some da lista. ✔️ excluir.
10. **Isolamento (o mais importante):** clique **Sair**, crie uma **segunda conta** e vá em Categorias/Receitas/Despesas → **todas as listas estão vazias**. Você não vê nada da primeira conta. ✔️ multi-tenant (RN13/CA076).

**C) Responsividade (obrigatório — nada pode quebrar):**
Abra o app, pressione **F12** → ícone de dispositivo, e percorra **320px → 1440px**:
- Abaixo de 900px: some a barra lateral, aparece o botão **☰**, e cada linha das listas vira um **cartão** (sem rolagem lateral).
- A partir de 900px: barra lateral fixa à esquerda e listas em tabela.
- Em nenhuma largura a página deve rolar para os lados.

**D) Teste no nível da API (opcional):**
Abra `backend/src/MyFinance.API/MyFinance.API.http` e envie os blocos da seção **Fase 2**, de cima para baixo.
- **Esperado:** criar → **201**; listar/obter → **200**; atualizar → **200**; excluir → **204**; categoria em uso → **409**; dados inválidos ou categoria inexistente → **400**; sem token → **401**; acessar pelo id um registro de **outro usuário** → **404**.

> **Dicas que evitam falso negativo neste projeto** (aprendidas na marra):
> - Antes de subir a API, **mate instâncias antigas** e libere a porta — senão você testa um build velho:
>   ```powershell
>   Get-Process -Name 'MyFinance.API' -ErrorAction SilentlyContinue | Stop-Process -Force
>   ```
> - Para testar isolamento **do zero**, limpe as tabelas (apaga TODOS os dados locais):
>   ```powershell
>   docker exec myfinance-db psql -U myfinance -d myfinance -c "TRUNCATE income_sources, expense_sources, categories, users CASCADE;"
>   ```
> - Ao contar itens de uma lista no PowerShell, use `@(...)` e compare **ids**, não só quantidades — foi confundindo "quantos vieram" com "de quem são" que um vazamento inexistente pareceu real.

### ⬜ Fases 3 a 7
*Cada fase concluída adiciona aqui seu Mapa de teste.* — **pendente.**

---

## 5. Resolução de problemas comuns
- **"Connection refused" / erro de banco:** o container não está no ar → `docker compose up -d db` e aguarde `healthy`.
- **Front não chama a API (CORS/erro de rede):** confira que o backend está em `:5080` e o front em `:4200` (a API libera CORS só para `:4200` em dev).
- **`Jwt:SigningKey não configurada`:** rode o backend em ambiente **Development** (o `dotnet run` já usa o perfil `http`, que carrega `appsettings.Development.json`).
- **Porta 5080/4200 ocupada:** feche instâncias anteriores (ou reinicie o terminal).
