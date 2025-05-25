using SteamHub.ApiContract.Context.Repositories;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Proxies;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.ServiceProxies;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web;
using SteamHub.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();


builder.Services.AddHttpContextAccessor();


builder.Services.AddScoped<IUserDetails, WebUserDetails>();

builder.Services.AddScoped<IUserRepository, UserRepositoryProxy>();
builder.Services.AddScoped<IGameRepository, GameRepositoryProxy>();
builder.Services.AddScoped<IItemTradeRepository, ItemTradeRepositoryProxy>();
builder.Services.AddScoped<IItemTradeDetailRepository, ItemTradeDetailsRepositoryProxy>();
builder.Services.AddScoped<IUsersGamesRepository, UserGamesRepositoryProxy>();
builder.Services.AddScoped<IItemRepository, ItemRepositoryProxy>();
builder.Services.AddScoped<ITagRepository, TagRepositoryProxy>();
builder.Services.AddScoped<IPointShopItemRepository, PointShopItemRepositoryProxy>();
builder.Services.AddScoped<IUserPointShopItemInventoryRepository, UserPointShopItemInventoryRepositoryProxy>();
builder.Services.AddScoped<IUserInventoryRepository, UserInventoryRepositoryProxy>();

builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddScoped<IUserService, UserServiceProxy>();
builder.Services.AddScoped<IGameService, GameServiceProxy>();
builder.Services.AddScoped<IUserGameService, UserGameServiceProxy>();
builder.Services.AddScoped<ICartService, CartServiceProxy>();
builder.Services.AddScoped<IDeveloperService, DeveloperServiceProxy>();
builder.Services.AddScoped<IPointShopService, PointShopServiceProxy>();
builder.Services.AddScoped<IInventoryService, InventoryServiceProxy>();
builder.Services.AddScoped<ITradeService, TradeServiceProxy>();

builder.Services.AddScoped<IMarketplaceService, MarketplaceService>();



var apiBaseUri = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);

builder.Services.AddHttpClient("AuthApi", client =>
{
    client.BaseAddress = apiBaseUri;
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    
}).ConfigurePrimaryHttpMessageHandler(() => new NoSslCertificateValidationHandler());


builder.Services.AddHttpClient("SteamHubApi", client =>
{
    client.BaseAddress = apiBaseUri;
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    
}).ConfigurePrimaryHttpMessageHandler(() => new NoSslCertificateValidationHandler());


builder.Services.AddAuthentication("SteamHubAuth")
    .AddCookie("SteamHubAuth", options =>
    {
        options.LoginPath = "/Authentication/Login";
        options.AccessDeniedPath = "/Authentication/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    });

builder.Services.AddAuthorization();

builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); // Set timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization(); // Always place this after UseAuthentication
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=HomePage}/{action=Index}/{id?}");
// app.MapRazorPages();

app.Run();
