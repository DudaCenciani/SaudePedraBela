using Microsoft.EntityFrameworkCore;
using SaudePedraBela.Data;
using SaudePedraBela.Models;
using SaudePedraBela.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SaudePedraBelaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SaudePedraBelaContext")
        ?? throw new InvalidOperationException("Connection string 'SaudePedraBelaContext' not found.")));

builder.Services.AddControllersWithViews();

// Session precisa de tempo de expiração
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8); // sessão dura 8 horas
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 1. Vincula a seção do appsettings com a classe DropboxSettings
builder.Services.Configure<DropboxSettings>(builder.Configuration.GetSection("DropboxSettings"));

// 2. Registra o serviço para ser usado em Controllers ou outras classes
builder.Services.AddScoped<DropboxService>();



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();