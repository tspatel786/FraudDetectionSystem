using Microsoft.ML;
using FraudDetectionSystem.ML.Models;

namespace FraudDetectionSystem.ML.Training;

public class CustomerTrainer
{
    private readonly MLContext _mlContext = new();

    public void Train()
    {
        var data = _mlContext.Data.LoadFromTextFile<CustomerFraudData>(
            "Data/customer-fraud.csv",
            hasHeader: true,
            separatorChar: ',');

        var pipeline = _mlContext.Transforms
            .Concatenate("Features",
                nameof(CustomerFraudData.VisitFrequency),
                nameof(CustomerFraudData.AveragePurchase),
                nameof(CustomerFraudData.LifetimeValue),
                nameof(CustomerFraudData.GoldPurchaseCount),
                nameof(CustomerFraudData.DiamondPurchaseCount),
                nameof(CustomerFraudData.CoinPurchaseCount))
            .Append(
                _mlContext.BinaryClassification.Trainers.LightGbm());

        var model = pipeline.Fit(data);

        _mlContext.Model.Save(
            model,
            data.Schema,
            "MLModels/customerModel.zip");
    }
}