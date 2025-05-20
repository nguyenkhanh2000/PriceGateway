using CommonLib.Implementations;
using CommonLib.Interfaces;
using MonitorCore.Interfaces;
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

builder.Services.AddTransient<IPriceHandle,CPriceHandle>();
//builder.Services.AddTransient<IS6GApp, CS6GApp>();
//builder.Services.AddTransient<IErrorLogger, CErrorLogger>();
//builder.Services.AddTransient<ISqlLogger, CSqlLogger>();
//builder.Services.AddTransient<IInfoLogger, CInfoLogger>();
//builder.Services.AddTransient<IDebugLogger, CDebugLogger>();
//builder.Services.AddTransient<ICommon, CCommon>();
//builder.Services.AddTransient<IMonitor, CMonitor>();
//builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache(); // Cung cấp IDistributedCache - netcore 3.1 ngầm định thêm implementation mặc định cho IDistributedCache, cần thêm trước AddSession()
builder.Services.AddSession();

builder.Services.Configure<IISOptions>(options =>
{
    options.AutomaticAuthentication = false;
});

//Connect Redis
var redisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value;
builder.Services.AddSingleton<Lazy<ConnectionMultiplexer>>(sp => new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect($"{redisConnectionString}")));

//add signalR
builder.Services.AddSignalR();
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

app.Run();
