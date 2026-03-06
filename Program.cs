using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ShelfAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// ── EF Core + SQLite ──
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Controllers ──
builder.Services.AddControllers();

// ── CORS ──
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ── Swagger / OpenAPI ──
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ShelfAPI",
        Version = "v1",
        Description = "API REST de produtos — CRUD completo com ASP.NET Core 8 + SQLite",
        Contact = new OpenApiContact
        {
            Name = "ShelfAPI",
            Url = new Uri("https://github.com/seu-usuario/ShelfAPI")
        }
    });
});

var app = builder.Build();

// ── Migrations automáticas ──
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ── Middleware pipeline ──
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ShelfAPI v1");
        options.RoutePrefix = string.Empty;          // Swagger na raiz "/"
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();
app.Run();
