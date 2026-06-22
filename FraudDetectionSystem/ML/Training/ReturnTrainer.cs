using Microsoft.ML;
using FraudDetectionSystem.ML.Models;

namespace FraudDetectionSystem.ML.Training;

public class ReturnTrainer
{
    private readonly MLContext _mlContext = new();

    public void Train()
    {
        var data = _mlContext.Data.LoadFromTextFile<ReturnFraudData>(
            "Data/return-fraud.csv",
            hasHeader: true,
            separatorChar: ',');

        var pipeline = _mlContext.Transforms
            .Concatenate("Features",
                nameof(ReturnFraudData.PurchaseAmount),
                nameof(ReturnFraudData.ReturnAmount),
                nameof(ReturnFraudData.DaysBetweenPurchaseAndReturn))
            .Append(
                _mlContext.BinaryClassification.Trainers.LightGbm());

        var model = pipeline.Fit(data);

        _mlContext.Model.Save(
            model,
            data.Schema,
            "MLModels/returnModel.zip");
    }
}