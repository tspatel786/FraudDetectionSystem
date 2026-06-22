using FraudDetectionSystem.Data;
using FraudDetectionSystem.ML.Prediction;
using FraudDetectionSystem.ML.Training;
using FraudDetectionSystem.Repository.Implementation;
using FraudDetectionSystem.Repository.Interface;
using FraudDetectionSystem.Services.Implementation;
using FraudDetectionSystem.Services.Interface;
using Microsoft.EntityFrameworkCore;

// Run "dotnet run -- train" to (re)train the ML.NET fraud model from Data/fraud-data.csv
// and write it to MLModels/fraudModel.zip, without starting the web server.
if (args.Length > 0 && args[0].Equals("train", StringComparison.OrdinalIgnoreCase))
{
    ModelTrainer.Train();
    return;
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Kisna Jewelry - Fraudulent Transaction Monitoring API", Version = "v1" });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

builder.Services.AddScoped<IFraudAlertRepository, FraudAlertRepository>();
builder.Services.AddScoped<IFraudAlertService, FraudAlertService>();

builder.Services.AddSingleton<FraudPredictionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
