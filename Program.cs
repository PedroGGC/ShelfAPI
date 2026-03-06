using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ShelfAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.SwaggerDoc("v1", new OpenApiInfo
{
    Title = "ShelfAPI", Version = "v1",
    Description = "API REST de produtos — CRUD com ASP.NET Core 8 + SQLite"
}));

var app = builder.Build();

// ── Auto-migrate ──
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o => { o.SwaggerEndpoint("/swagger/v1/swagger.json", "ShelfAPI v1"); o.RoutePrefix = string.Empty; });
}

app.UseHttpsRedirection();
app.UseCors();

// ── Helper ──
static ProductResponse ToDto(Product p) => new(p.Id, p.Name, p.Description, p.Price, p.Stock, p.CreatedAt, p.UpdatedAt);

// ── Endpoints ──
var api = app.MapGroup("/api/products").WithTags("Products");

api.MapGet("/", async (AppDbContext db) =>
{
    var products = await db.Products.AsNoTracking().OrderByDescending(p => p.CreatedAt).ToListAsync();
    return Results.Ok(products.Select(ToDto));
});

api.MapGet("/{id:int}", async (int id, AppDbContext db) =>
    await db.Products.FindAsync(id) is Product p
        ? Results.Ok(ToDto(p))
        : Results.NotFound(new { message = $"Produto com ID {id} não encontrado." }));

api.MapPost("/", async (CreateProductRequest req, AppDbContext db) =>
{
    var product = new Product
    {
        Name = req.Name, Description = req.Description,
        Price = req.Price, Stock = req.Stock, CreatedAt = DateTime.UtcNow
    };
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/api/products/{product.Id}", ToDto(product));
});

api.MapPut("/{id:int}", async (int id, UpdateProductRequest req, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null)
        return Results.NotFound(new { message = $"Produto com ID {id} não encontrado." });

    product.Name = req.Name;
    product.Description = req.Description;
    product.Price = req.Price;
    product.Stock = req.Stock;
    product.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.Ok(ToDto(product));
});

api.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null)
        return Results.NotFound(new { message = $"Produto com ID {id} não encontrado." });

    db.Products.Remove(product);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
