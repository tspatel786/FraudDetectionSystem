using Microsoft.ML;
using FraudDetectionSystem.ML.Models;

namespace FraudDetectionSystem.ML.Training;

public class PaymentTrainer
{
    private readonly MLContext _mlContext = new();

    public void Train()
    {
        var data = _mlContext.Data.LoadFromTextFile<PaymentFraudData>(
            "Data/payment-fraud.csv",
            hasHeader: true,
            separatorChar: ',');

        var pipeline = _mlContext.Transforms
            .Categorical.OneHotEncoding(
                "PaymentMethodEncoded",
                nameof(PaymentFraudData.PaymentMethod))
            .Append(
                _mlContext.Transforms.Concatenate(
                    "Features",
                    nameof(PaymentFraudData.Amount),
                    "PaymentMethodEncoded"))
            .Append(
                _mlContext.BinaryClassification.Trainers.LightGbm());

        var model = pipeline.Fit(data);

        _mlContext.Model.Save(
            model,
            data.Schema,
            "MLModels/paymentModel.zip");
    }
}