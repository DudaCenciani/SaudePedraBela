using Microsoft.EntityFrameworkCore;
using SaudePedraBela.Data;

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