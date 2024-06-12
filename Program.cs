using System.Diagnostics;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using sistema_barbearia.Data;
using sistema_barbearia.Services;


var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("The connection string 'DefaultConnection' is missing or invalid.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(connectionString));

    
builder.Services.AddControllers(); 

builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization();    

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();

var app = builder.Build();

var loginUrl = "http://localhost:5078/api/account/login";


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
} else 
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// app.UseHttpsRedirection();
// app.MapDefaultControllerRoute();
// app.MapControllers();
// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Account}/{action=Login}");
app.MapControllerRoute(
    name: "default",
    pattern: "/{controller=Account}/{action=Login}");

Process.Start(new ProcessStartInfo
{
    FileName = loginUrl,
    UseShellExecute = true
});

app.Run();
