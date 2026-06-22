using FraudDetectionSystem.ML.Models;
using Microsoft.ML;

namespace FraudDetectionSystem.ML.Training
{

    public class StoreTrainer
    {
        private readonly MLContext _mlContext;

        public StoreTrainer()
        {
            _mlContext = new MLContext(seed: 1);
        }

        public void Train()
        {
            var data = _mlContext.Data.LoadFromTextFile<StoreFraudData>(
                "Data/store-fraud.csv",
                hasHeader: true,
                separatorChar: ',');

            var pipeline = _mlContext.Transforms
                .Concatenate("Features",
                    nameof(StoreFraudData.DailySalesAmount),
                    nameof(StoreFraudData.DailySalesCount),
                    nameof(StoreFraudData.DailyReturnCount),
                    nameof(StoreFraudData.DailyReturnAmount),
                    nameof(StoreFraudData.Avg30DaySales),
                    nameof(StoreFraudData.Avg30DayReturns))
                .Append(
                    _mlContext.BinaryClassification.Trainers.LightGbm(
                        labelColumnName: "Label",
                        featureColumnName: "Features"));

            var model = pipeline.Fit(data);

            _mlContext.Model.Save(
                model,
                data.Schema,
                "MLModels/storeModel.zip");
        }
    }
}