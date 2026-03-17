using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using MyPortfolio.Data.Concrete;
using MyPortfolio.Data.Abstract;
using MyPortfolio.Entities.Concrete;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddScoped<MyPortfolio.Services.EmailService>();
builder.Services.AddSingleton<MyPortfolio.Services.TelegramService>();
builder.Services.AddHostedService<MyPortfolio.Services.TelegramPollingService>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddSignalR();


// PostgreSQL i�in tarih format� d�zeltmesi 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddDbContext<MyPortfolioContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

// --- IDENTITY SETTINGS ---
builder.Services.AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<MyPortfolioContext>()
    .AddDefaultTokenProviders();

// Password Rules
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
});

// Cookie Settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(100);
    options.LoginPath = "/Admin/Login/Index";
    options.AccessDeniedPath = "/Admin/Login/Index";
    options.SlidingExpiration = true;

    // --- 404 KALKANI: Yetkisiz /Admin erişimlerini 404 ile yanıtla ---
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/Admin"))
            {
                context.Response.StatusCode = 404;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            if (context.Request.Path.StartsWithSegments("/Admin"))
            {
                context.Response.StatusCode = 404;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }
    };
});
// --- END IDENTITY SETTINGS ---

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Session for AdminShieldMiddleware hijacking protection
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Forwarded Headers for Nginx Proxy (SignalR etc.)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseStatusCodePagesWithReExecute("/Error/{0}"); // Middleware ile cakisma onlendi

app.UseMiddleware<MyPortfolio.Middleware.GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseMiddleware<MyPortfolio.Middleware.AdminShieldMiddleware>();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<MyPortfolio.Hubs.PortfolioHub>("/portfolioHub");

// --- ROL SEED: "Admin" rolünü oluştur ve mevcut kullanıcılara ata ---
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new AppRole { Name = "Admin" });
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var allUsers = userManager.Users.ToList();
    foreach (var user in allUsers)
    {
        if (!await userManager.IsInRoleAsync(user, "Admin"))
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}

app.Run();
