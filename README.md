# 🗄️ ShelfAPI

API REST completa de **produtos** (CRUD) construída com **ASP.NET Core 8**, **Entity Framework Core** e **SQLite**.

---

## 🚀 Funcionalidades

| Feature                    | Detalhes                                  |
| -------------------------- | ----------------------------------------- |
| **CRUD completo**          | GET, POST, PUT, DELETE em `/api/products` |
| **Entity Framework Core**  | ORM com SQLite                            |
| **Data Annotations**       | Validação automática nos DTOs             |
| **DTOs**                   | Request/Response separados do modelo      |
| **Swagger/OpenAPI**        | Documentação interativa na raiz `/`       |
| **Migrations automáticas** | Banco criado ao iniciar a app             |
| **CORS habilitado**        | Aceita qualquer origem em dev             |

---

## 📁 Estrutura do Projeto

```
ShelfAPI/
├── Controllers/
│   └── ProductsController.cs    # Endpoints CRUD
├── Data/
│   └── AppDbContext.cs           # Contexto do EF Core
├── DTOs/
│   └── ProductDto.cs             # Request e Response DTOs
├── Models/
│   └── Product.cs                # Entidade Product
├── Migrations/                   # Migrations geradas pelo EF Core
├── Program.cs                    # Configuração da aplicação
├── appsettings.json              # Connection string + logging
└── ShelfAPI.csproj               # Dependências do projeto
```

---

## ⚙️ Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- (Opcional) [EF Core CLI](https://learn.microsoft.com/ef/core/cli/dotnet) — `dotnet tool install --global dotnet-ef`

---

## 🏃 Como Rodar

```bash
# 1. Clone o repositório
git clone https://github.com/seu-usuario/ShelfAPI.git
cd ShelfAPI

# 2. Restaure os pacotes
dotnet restore

# 3. Execute a aplicação
dotnet run
```

A API estará disponível em **https://localhost:5001** (ou **http://localhost:5000**).
O Swagger UI estará acessível na **raiz** (`/`).

---

## 📡 Endpoints

| Método   | Rota                 | Descrição               |
| -------- | -------------------- | ----------------------- |
| `GET`    | `/api/products`      | Lista todos os produtos |
| `GET`    | `/api/products/{id}` | Busca produto por ID    |
| `POST`   | `/api/products`      | Cria um novo produto    |
| `PUT`    | `/api/products/{id}` | Atualiza um produto     |
| `DELETE` | `/api/products/{id}` | Deleta um produto       |

### Exemplo — Criar Produto

```bash
curl -X POST https://localhost:5001/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Teclado Mecânico",
    "description": "Teclado mecânico RGB com switches blue",
    "price": 349.90,
    "stock": 50
  }'
```

**Resposta (201 Created):**

```json
{
  "id": 1,
  "name": "Teclado Mecânico",
  "description": "Teclado mecânico RGB com switches blue",
  "price": 349.9,
  "stock": 50,
  "createdAt": "2026-03-06T19:16:00Z",
  "updatedAt": null
}
```

---

## 🗃️ Banco de Dados

O SQLite é usado por padrão. O arquivo `shelf.db` é criado automaticamente na raiz do projeto ao iniciar a aplicação (migrations automáticas).

Para alterar a connection string, edite o `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=shelf.db"
  }
}
```

### Comandos de Migração

```bash
# Criar uma nova migração
dotnet ef migrations add NomeDaMigracao

# Aplicar migrations manualmente
dotnet ef database update

# Reverter a última migração
dotnet ef migrations remove
```

---

## 🚢 Deploy

### Docker (opcional)

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

### Azure App Service

```bash
dotnet publish -c Release -o ./publish
az webapp deploy --resource-group MeuGrupo --name shelf-api --src-path ./publish
```

---

## 📄 Licença

MIT
