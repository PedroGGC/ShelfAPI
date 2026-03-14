# ShelfAPI 📦

API REST de **gerenciamento de produtos** com CRUD completo, construída com ASP.NET Core 8, Entity Framework Core e SQLite.

---

## ⚡ Início Rápido

**Pré-requisito:** [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```bash
git clone https://github.com/seu-usuario/ShelfAPI.git
cd ShelfAPI
dotnet run
```

Pronto! A API sobe em **http://localhost:5003** e o **Swagger UI** abre automaticamente na raiz — acesse [`http://localhost:5003`](http://localhost:5003) no browser para explorar e testar os endpoints visualmente.

> O banco de dados SQLite (`shelf.db`) é criado automaticamente. Não é necessário rodar nenhum comando de migração.

---

## 🛣️ Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| `GET` | `/api/products` | Lista todos os produtos |
| `GET` | `/api/products/{id}` | Busca produto por ID |
| `POST` | `/api/products` | Cria um novo produto |
| `PUT` | `/api/products/{id}` | Atualiza um produto |
| `DELETE` | `/api/products/{id}` | Deleta um produto |

---

## 📋 Modelo de Produto

| Campo | Tipo | Regras |
|-------|------|--------|
| `id` | int | Auto-gerado |
| `name` | string | Obrigatório · 2–150 caracteres |
| `description` | string? | Opcional · máx. 500 caracteres |
| `price` | decimal | Obrigatório · entre R$ 0,01 e R$ 999.999,99 |
| `stock` | int | Obrigatório · valor ≥ 0 |
| `createdAt` | DateTime | Gerado automaticamente |
| `updatedAt` | DateTime? | Atualizado nos PUTs |

---

## 🧪 Exemplos de Uso

### Criar produto
```bash
curl -X POST http://localhost:5003/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Teclado Mecânico",
    "description": "RGB com switches blue",
    "price": 349.90,
    "stock": 50
  }'
```

**Resposta 201 Created:**
```json
{
  "id": 1,
  "name": "Teclado Mecânico",
  "description": "RGB com switches blue",
  "price": 349.90,
  "stock": 50,
  "createdAt": "2026-03-14T16:32:00Z",
  "updatedAt": null
}
```

### Listar todos os produtos
```bash
curl http://localhost:5003/api/products
```

### Atualizar produto
```bash
curl -X PUT http://localhost:5003/api/products/1 \
  -H "Content-Type: application/json" \
  -d '{"name": "Teclado Mecânico Pro", "price": 499.90, "stock": 30}'
```

### Deletar produto
```bash
curl -X DELETE http://localhost:5003/api/products/1
```

---

## ⚙️ Configuração

A connection string do banco fica em `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=shelf.db"
  }
}
```

Para usar outro banco (ex: PostgreSQL), troque o provider no `Program.cs` e atualize a connection string.

### Migrations (opcional)

As migrations são aplicadas automaticamente ao iniciar a app. Se precisar gerenciá-las manualmente:

```bash
dotnet ef migrations add NomeDaMigracao   # criar migração
dotnet ef database update                  # aplicar
dotnet ef migrations remove                # reverter última
```

---

## 🐳 Deploy com Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ShelfAPI.dll"]
```

```bash
docker build -t shelfapi .
docker run -p 8080:8080 shelfapi
```

---

## 🗂️ Estrutura do Projeto

```
ShelfAPI/
├── Program.cs           # Configuração da app + todos os endpoints
├── Models.cs            # Entidade Product, DTOs e AppDbContext
├── Migrations/          # Migrations do EF Core (auto-geradas)
├── appsettings.json     # Connection string e logging
└── ShelfAPI.csproj      # Dependências do projeto
```

---

## Licença

[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
