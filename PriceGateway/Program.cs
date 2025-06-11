using CommonLib.Implementations;
using CommonLib.Interfaces;
using MonitorCore.Interfaces;
using PriceGateway.BLL;
using PriceGateway.Hubs;
using PriceGateway.Implementations;
using PriceGateway.Interfaces;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAppLogger();
builder.Services.AddHttpClient();

builder.Services.AddDistributedMemoryCache(); // Cung cấp IDistributedCache - netcore 3.1 ngầm định thêm implementation mặc định cho IDistributedCache, cần thêm trước AddSession()
builder.Services.AddSession();

builder.Services.Configure<IISOptions>(options =>
{
    options.AutomaticAuthentication = false;
});

//Connect Redis 250
var redisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value;
builder.Services.AddSingleton<Lazy<ConnectionMultiplexer>>(sp => new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect($"{redisConnectionString}")));
//Connect Redis Sentinel
var redisConnectionString2 = builder.Configuration.GetSection("Redis:ConnectionString_NewAPP").Value;
builder.Services.AddSingleton<Lazy<ConnectionMultiplexer>>(sp =>
    new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(redisConnectionString2)));

//add signalR
builder.Services.AddSignalR();

builder.Services.AddSignalR()
    .AddMessagePackProtocol(); // hỗ trợ MessagePack

builder.Services.AddTransient<IPriceHandle, CPriceHandle>();
builder.Services.AddSingleton<IPriceGateway, CPriceGateway>();
builder.Services.AddHostedService<PriceGatewayListenerService>();
builder.Services.AddSingleton<IClientConnectionStore, ClientConnectionStore>();


//builder.Services.AddHostedService<RealtimeDataPusher>();
string[] DomainCors = builder.Configuration.GetSection("DomainCors").Value.Split(",");
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(DomainCors)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseAuthorization();

app.UseSession();

app.MapControllers();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("PriceGateway");
    });
});
//map signalR hub endpoint
app.MapHub<HubEx>("/HubKhanhNV");
app.MapHub<Hub_HSX>("/HubHSX");
app.MapHub<Hub_HNX>("/HubHNX");
app.MapHub<ChannelHub>("/channelHub");

app.Run();
