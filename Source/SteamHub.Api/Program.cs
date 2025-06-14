using System.Text.Json.Serialization;
using SteamHub.Api.Context;
using SteamHub.Api.Context.Repositories;
using SteamHub.ApiContract.Context.Repositories;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.ServiceProxies;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>();

builder.Services.AddScoped<IPointShopItemRepository, PointShopItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IUserPointShopItemInventoryRepository, UserPointShopItemInventoryRepository>();
builder.Services.AddScoped<IUsersGamesRepository, UsersGamesRepository>();
builder.Services.AddScoped<IStoreTransactionRepository, StoreTransactionRepository>();
builder.Services.AddScoped<IItemTradeRepository, ItemTradeRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IUserInventoryRepository, UserInventoryRepository>();
builder.Services.AddScoped<IItemTradeDetailRepository, ItemTradeDetailRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IUserGameService, UserGameService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IDeveloperService, DeveloperService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IPointShopService, PointShopService>();
builder.Services.AddScoped<ITradeService, TradeService>();
builder.Services.AddScoped<IMarketplaceService, MarketplaceService>();


builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();