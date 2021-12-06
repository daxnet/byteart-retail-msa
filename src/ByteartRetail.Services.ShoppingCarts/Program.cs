using ByteartRetail.Common.DataAccess;
using ByteartRetail.Common.Messaging;
using ByteartRetail.DataAccess.Mongo;
using ByteartRetail.Messaging.RabbitMQ;
using ByteartRetail.Services.ShoppingCarts.ServiceClients;
using MongoDB.Driver;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MongoDB connection
var configuration = builder.Configuration;
var mongoConnectionString = configuration["mongo:connectionString"];
var mongoDatabase = configuration["mongo:database"];
builder.Services.AddScoped<IDataAccessObject>(_ => new MongoDataAccessObject(new MongoUrl(mongoConnectionString), mongoDatabase));
builder.Services.AddHttpClient<ProductCatalogServiceClient>(config =>
{
    config.BaseAddress = new Uri(builder.Configuration["serviceClient:productCatalog:url"]);
    config.Timeout = TimeSpan.FromMinutes(1);
});

// Configure RabbitMQ
var rabbitMqServer = configuration["rabbit:server"];
var connectionFactory = new ConnectionFactory { HostName = rabbitMqServer };
builder.Services.AddSingleton<IEventPublisher>(sp => new RabbitMQMessagePublisher(connectionFactory, "byteartretail",
    "fanout", sp.GetRequiredService<ILogger<RabbitMQMessagePublisher>>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder =>
{
    builder.AllowAnyOrigin();
    builder.AllowAnyHeader();
    builder.AllowAnyMethod();
});


app.UseAuthorization();

app.MapControllers();

app.Run();
