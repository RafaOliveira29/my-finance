# MyFinance

Controle financeiro pessoal por **competência mensal** + **gestão de dívidas por credor** (com visão em parcelas). Produto full-stack: **Angular 21** + **ASP.NET Core (.NET 10) / Clean Architecture pragmática** + **PostgreSQL**.

> Documentação viva completa em [`DOC.md`](DOC.md); pacote de descoberta (produto, épicos, histórias, critérios, modelagem, arquitetura) em [`descoberta/`](descoberta/).

## Estrutura (monorepo simétrico)
```
MyFinance/
├─ backend/                        # ASP.NET Core (.NET 10) — Clean Architecture
│  ├─ MyFinance.slnx
│  ├─ global.json
│  ├─ src/
│  │  ├─ MyFinance.Domain          # entidades ricas, enums, VOs (sem framework)
│  │  ├─ MyFinance.Application     # casos de uso, DTOs, ports, validators
│  │  ├─ MyFinance.Infrastructure  # EF Core (escrita) + Dapper (leitura) + Npgsql
│  │  └─ MyFinance.API             # controllers finos, middleware, DI, OpenAPI
│  └─ tests/
│     ├─ MyFinance.Domain.Tests
│     └─ MyFinance.Application.Tests
├─ frontend/                       # Angular 21 (zoneless, signals)
├─ descoberta/                     # pacote de descoberta (docs versionadas)
├─ .github/workflows/              # CI (builda backend + frontend)
├─ docker-compose.yml              # PostgreSQL 16 local
└─ DOC.md · README.md
```

## Como rodar (dev)
```bash
# 1. Banco (na raiz do repo)
docker compose up -d db

# 2. Backend (http://localhost:5080, OpenAPI em /openapi/v1.json)
dotnet run --project backend/src/MyFinance.API

# 3. Frontend
cd frontend && npm start
```

## Testes
```bash
dotnet test backend/MyFinance.slnx
```
