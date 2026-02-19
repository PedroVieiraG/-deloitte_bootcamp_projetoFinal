using ApiMoniEquipamentosPesados.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Padrão restaurado: Enums serão lidos e enviados como números (0, 1, 2...)
builder.Services.AddControllers();

// Configuração do PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Verifica e aplica migrations pendentes ao iniciar (útil para o Docker)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); 
}

app.MapControllers();

app.Run();