using System.Diagnostics;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using sistema_barbearia.Data;
using sistema_barbearia.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("The connection string 'DefaultConnection' is missing or invalid.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString));

builder.Services.AddControllersWithViews();

var tokenKey = builder.Configuration["TokenKey"] ?? "defaultTokenKey";
var key = Encoding.UTF8.GetBytes(tokenKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

// Configurar sessão
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // Adiciona redirecionamento para HTTPS
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // Habilitar sessões
app.UseAuthentication(); // Habilitar autenticação
app.UseAuthorization(); // Habilitar autorização

app.MapControllerRoute(
    name: "default",
    pattern: "/{controller=Account}/{action=Login}/{id?}");

// Redirecionar para a página de login ao iniciar
var loginUrl = "http://localhost:5078/api/account/login";
Process.Start(new ProcessStartInfo
{
    FileName = loginUrl,
    UseShellExecute = true
});

app.Run();
