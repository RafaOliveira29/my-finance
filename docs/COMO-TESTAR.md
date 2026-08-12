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
Isso cria/atualiza as tabelas no banco (hoje: `users`).

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

### ⬜ Fases 2 a 7
*Cada fase concluída adiciona aqui seu Mapa de teste (ex.: Fase 2 — criar categoria/receita/despesa e ver na lista).* — **pendente.**

---

## 5. Resolução de problemas comuns
- **"Connection refused" / erro de banco:** o container não está no ar → `docker compose up -d db` e aguarde `healthy`.
- **Front não chama a API (CORS/erro de rede):** confira que o backend está em `:5080` e o front em `:4200` (a API libera CORS só para `:4200` em dev).
- **`Jwt:SigningKey não configurada`:** rode o backend em ambiente **Development** (o `dotnet run` já usa o perfil `http`, que carrega `appsettings.Development.json`).
- **Porta 5080/4200 ocupada:** feche instâncias anteriores (ou reinicie o terminal).
