using Microsoft.ML;
using FraudDetectionSystem.ML.Models;

namespace FraudDetectionSystem.ML.Training;

public class EmployeeTrainer
{
    private readonly MLContext _mlContext = new();

    public void Train()
    {
        var data = _mlContext.Data.LoadFromTextFile<EmployeeFraudData>(
            "Data/employee-fraud.csv",
            hasHeader: true,
            separatorChar: ',');

        var pipeline = _mlContext.Transforms
            .Concatenate("Features",
                nameof(EmployeeFraudData.SalesCount),
                nameof(EmployeeFraudData.SalesAmount),
                nameof(EmployeeFraudData.UniqueCustomers),
                nameof(EmployeeFraudData.EmployeePurchaseCount),
                nameof(EmployeeFraudData.HighValueSalesCount))
            .Append(
                _mlContext.BinaryClassification.Trainers.LightGbm());

        var model = pipeline.Fit(data);

        _mlContext.Model.Save(
            model,
            data.Schema,
            "MLModels/employeeModel.zip");
    }
}