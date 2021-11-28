using ByteartRetail.Common.DataAccess;
using ByteartRetail.DataAccess.Mongo;
using MongoDB.Driver;

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
builder.Services.AddScoped<IDataAccessObject>(serviceProvider => new MongoDataAccessObject(new MongoUrl(mongoConnectionString), mongoDatabase));

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
