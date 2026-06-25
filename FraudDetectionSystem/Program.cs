using FraudDetectionSystem.Data;
using FraudDetectionSystem.ML.Prediction;
using FraudDetectionSystem.ML.Training;
using FraudDetectionSystem.Repository.Implementation;
using FraudDetectionSystem.Repository.Interface;
using FraudDetectionSystem.Services.Implementation;
using FraudDetectionSystem.Services.Interface;
using Microsoft.EntityFrameworkCore;

// Train commands (run without starting web server):
//   dotnet run -- train-all
//   dotnet run -- store-train
//   dotnet run -- payment-train
if (args.Length > 0)
{
    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    var connectionString = config.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("DefaultConnection missing in appsettings.json");

    switch (args[0].ToLowerInvariant())
    {
        case "train-all":
            MlTrainingOrchestrator.TrainAll(connectionString);
            return;
        case "train":
            ModelTrainer.Train();
            return;
        case "store-train":
            StoreFraudModelTrainer.Train();
            return;
        case "payment-train":
            PaymentFraudModelTrainer.Train();
            return;
        case "customer-train":
            CustomerBehaviorModelTrainer.Train(connectionString);  
            return;
        case "employee-train":
            EmployeeFraudModelTrainer.Train();
            return;
        case "return-train":
            ReturnOfferFraudModelTrainer.Train();
            return;
        case "validation-train":
            ValidationFraudModelTrainer.Train();
            return;
    }
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "POS-2.0 Project - Fraudulent Transaction Monitoring API", Version = "v1" });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IFraudDetectionService, FraudDetectionService>();

builder.Services.AddSingleton<StoreFraudPredictionService>();
builder.Services.AddSingleton<CustomerBehaviorPredictionService>();
builder.Services.AddSingleton<PaymentFraudPredictionService>();
builder.Services.AddSingleton<EmployeeFraudPredictionService>();
builder.Services.AddSingleton<ReturnOfferFraudPredictionService>();
builder.Services.AddSingleton<ValidationFraudPredictionService>();

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
