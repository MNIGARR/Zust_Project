using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ZustSN.Entities;
using ZustSN.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connection = builder.Configuration.GetConnectionString("myconn");
builder.Services.AddDbContext<ZustIdentityDBContext>(options =>
{
    options.UseSqlServer(connection, b => b.MigrationsAssembly("ZustASP.Entities"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();