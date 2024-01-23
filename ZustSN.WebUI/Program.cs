using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ZustSN.Entities;
using ZustSN.WebUI.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connection = builder.Configuration.GetConnectionString("myconn");
builder.Services.AddDbContext<ZustIdentityDBContext>(options =>
{
    options.UseSqlServer(connection, b => b.MigrationsAssembly("ZustSN.WebUI"));
});


builder.Services.AddIdentity<ZustIdentityUser, ZustIdentityRole>()
    .AddEntityFrameworkStores<ZustIdentityDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IPasswordHasher<ZustIdentityUser>, PasswordHasher<ZustIdentityUser>>();

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddSignalR();
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

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("Default", "{controller=Account}/{action=Login}/{id?}");
    endpoints.MapHub<ChatHub>("/chathub");
});
app.Run();