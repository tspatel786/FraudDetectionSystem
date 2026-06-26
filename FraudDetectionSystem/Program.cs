using FraudDetectionSystem.Data;
using FraudDetectionSystem.ML.Prediction;
using FraudDetectionSystem.ML.Training;
using FraudDetectionSystem.Repository.Implementation;
using FraudDetectionSystem.Repository.Interface;
using FraudDetectionSystem.Services.Implementation;
using FraudDetectionSystem.Services.Interface;
using Microsoft.EntityFrameworkCore;

// Train commands (run without starting web server):
//   dotnet run -- train          (trains all 6 ML.NET models from database)
//   dotnet run -- train-all      (same as train)
//   dotnet run -- customer-train
//   dotnet run -- store-train
//   dotnet run -- payment-train
//   dotnet run -- employee-train
//   dotnet run -- return-train
//   dotnet run -- validation-train
if (args.Length > 0)
{
    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    var trainConnectionString = config.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("DefaultConnection missing in appsettings.json");

    switch (args[0].ToLowerInvariant())
    {
        case "train":
        case "train-all":
            MlTrainingOrchestrator.TrainAll(trainConnectionString);
            return;
        case "store-train":
            StoreFraudModelTrainer.Train(trainConnectionString);
            return;
        case "payment-train":
            PaymentFraudModelTrainer.Train(trainConnectionString);
            return;
        case "customer-train":
            CustomerBehaviorModelTrainer.Train(trainConnectionString);
            return;
        case "employee-train":
            EmployeeFraudModelTrainer.Train(trainConnectionString);
            return;
        case "return-train":
            ReturnOfferFraudModelTrainer.Train(trainConnectionString);
            return;
        case "validation-train":
            ValidationFraudModelTrainer.Train(trainConnectionString);
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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<PosDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IFraudDataRepository, FraudDataRepository>();
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
