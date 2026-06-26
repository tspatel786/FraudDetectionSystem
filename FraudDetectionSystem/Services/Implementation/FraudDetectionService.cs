using FraudDetectionSystem.Data;
using FraudDetectionSystem.ML.Common;
using FraudDetectionSystem.ML.Models.Payment;
using FraudDetectionSystem.ML.Models.ReturnOffer;
using FraudDetectionSystem.ML.Models.Store;
using FraudDetectionSystem.ML.Models.Validation;
using FraudDetectionSystem.ML.Prediction;
using FraudDetectionSystem.Models;
using FraudDetectionSystem.Models.Dtos;
using FraudDetectionSystem.Models.Enum;
using FraudDetectionSystem.Repository.Interface;
using FraudDetectionSystem.Services.Interface;

namespace FraudDetectionSystem.Services.Implementation
{
    public class FraudDetectionService : IFraudDetectionService
    {
        private readonly AppDbContext _db;
        private readonly IFraudDataRepository _fraudData;
        private readonly CustomerBehaviorPredictionService _customerMl;
        private readonly PaymentFraudPredictionService _paymentMl;
        private readonly EmployeeFraudPredictionService _employeeMl;
        private readonly ReturnOfferFraudPredictionService _returnOfferMl;
        private readonly StoreFraudPredictionService _storeMl;
        private readonly ValidationFraudPredictionService _validationMl;

        public FraudDetectionService(
            AppDbContext db,
            IFraudDataRepository fraudData,
            CustomerBehaviorPredictionService customerMl,
            PaymentFraudPredictionService paymentMl,
            EmployeeFraudPredictionService employeeMl,
            ReturnOfferFraudPredictionService returnOfferMl,
            StoreFraudPredictionService storeMl,
            ValidationFraudPredictionService validationMl)
        {
            _db = db;
            _fraudData = fraudData;
            _customerMl = customerMl;
            _paymentMl = paymentMl;
            _employeeMl = employeeMl;
            _returnOfferMl = returnOfferMl;
            _storeMl = storeMl;
            _validationMl = validationMl;
        }

        //This method will call then all 6 models will run
        public async Task<FraudDetectionResult> CheckAllAsync(FraudDetectionRequest request)
        {
            var result = new FraudDetectionResult();
            var results = new List<FraudModelResult>();

            try
            {
                results.Add(await CheckCustomerBehaviorAsync(request.CustomerId));
                results.Add(await CheckPaymentAsync(request));
                results.Add(await CheckEmployeeAsync(request.EmployeeId));
                results.Add(await CheckReturnOfferAsync(request));
                results.Add(await CheckStoreAsync(request));
                results.Add(await CheckValidationAsync(request));

                result.Results = results;
                result.IsAnyFraudDetected = results.Any(r => r.IsFraud);
                result.TotalAlertsRaised = 0;

                foreach (var r in results.Where(r => r.IsFraud))
                {
                    var saved = await SaveAlertAsync(request, r);
                    if (saved) result.TotalAlertsRaised++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FraudDetectionService error: {ex.Message}");
            }

            return result;
        }

        public async Task<FraudModelResult> CheckCustomerBehaviorAsync(long customerId)
        {
            var data = await _fraudData.GetCustomerBehaviorDataAsync(customerId);
            if (data == null)
                return NotChecked(FraudType.CustomerBehavior);

            var prediction = _customerMl.Predict(data);
            var flags = new List<string>();

            if (data.ReturnPercentage > FraudThresholds.CustomerHighReturnRatio)
                flags.Add($"High return ratio: {data.ReturnPercentage:P0}");

            if (data.CashPaymentPercentage > FraudThresholds.HighCashPaymentRatio)
                flags.Add($"Mostly cash payments: {data.CashPaymentPercentage:P0}");

            if (data.ReturnAmount > data.TotalPurchaseAmount * FraudThresholds.HighReturnAmountRatio)
                flags.Add("Return amount is 60%+ of total purchases");

            if (data.LedgerCreditAmount > data.LedgerDebitAmount * 2)
                flags.Add("Ledger credit far exceeds debit");

            if (data.AverageDaysBetweenPurchase < 3 && data.PurchaseCount > 5)
                flags.Add($"Buying every {data.AverageDaysBetweenPurchase:F1} days");

            return BuildResult(FraudType.CustomerBehavior, prediction.IsFraud,
                prediction.Probability, flags);
        }

        public async Task<FraudModelResult> CheckPaymentAsync(FraudDetectionRequest request)
        {
            var customerName = await _fraudData.GetCustomerNameAsync(request.CustomerId);
            var nameMismatch = MlHelper.NameMismatchScore(
                request.InvoiceCustomerName, customerName);

            var isCash = EnumExtensions.IsCashPayment(request.PaymentMethod);

            var prediction = _paymentMl.Predict(new PaymentFraudData
            {
                Amount = (float)request.Amount,
                PaymentMethod = request.PaymentMethod,
                Hour = request.TransactionDate.Hour,
                NameMismatchScore = nameMismatch,
                IsCash = isCash ? 1f : 0f,
                IsReturn = request.IsReturn ? 1f : 0f
            });

            var flags = new List<string>();

            if (nameMismatch > 0.5f)
                flags.Add($"Invoice vs customer name mismatch score: {nameMismatch:F2}");

            if (isCash && request.Amount > (decimal)FraudThresholds.LargeCashPaymentAmount)
                flags.Add($"Large cash payment: {request.Amount:N0}");

            if (request.IsReturn && isCash)
                flags.Add("Cash return — untraceable refund");

            return BuildResult(FraudType.Payment, prediction.IsFraud, prediction.Probability, flags);
        }

        public async Task<FraudModelResult> CheckEmployeeAsync(long EmployeeId)
        {
            var data = await _fraudData.GetEmployeeFraudDataAsync(EmployeeId);
            if (data == null)
                return NotChecked(FraudType.Employee);

            var prediction = _employeeMl.Predict(data);
            var flags = new List<string>();

            if (data.ReturnPercentage > FraudThresholds.EmployeeHighReturnRatio)
                flags.Add($"High return ratio: {data.ReturnPercentage:P0}");

            if (data.EmployeePurchaseCount > 2)
                flags.Add($"Employee purchased {data.EmployeePurchaseCount} times");

            if (data.EmployeePurchaseAmount > FraudThresholds.LargeCashPaymentAmount)
                flags.Add($"Employee purchase amount: {data.EmployeePurchaseAmount:N0}");

            if (data.IncentiveAmount > 10_000)
                flags.Add($"High incentive: {data.IncentiveAmount:N0}");

            if (data.AverageInvoiceAmount > FraudThresholds.LargeCashPaymentAmount)
                flags.Add($"Very high average invoice: {data.AverageInvoiceAmount:N0}");

            return BuildResult(FraudType.Employee, prediction.IsFraud, prediction.Probability, flags);
        }

        public async Task<FraudModelResult> CheckReturnOfferAsync(FraudDetectionRequest request)
        {
            var returnCount = await _fraudData.GetCustomerReturnCountAsync(request.CustomerId);
            var returnValue = request.IsReturn ? (float)request.Amount : 0f;
            var hadOffer = request.HasOffer || await _fraudData.CustomerHadRecentOfferAsync(request.CustomerId);
            var daysSinceOffer = hadOffer
                ? await _fraudData.GetDaysSinceLastOfferAsync(request.CustomerId)
                : 30;

            var prediction = _returnOfferMl.Predict(new ReturnOfferFraudData
            {
                ReturnCount = returnCount,
                ReturnValue = returnValue,
                DaysSinceOffer = daysSinceOffer,
                HadOffer = hadOffer ? 1f : 0f,
                ReturnAfterOfferRatio = request.IsReturn && hadOffer ? 1f : 0f,
                SuspiciousPatternScore = Math.Min(1f, returnCount / 10f)
            });

            var flags = new List<string>();

            if (request.IsReturn && hadOffer)
                flags.Add("Return happening right after offer was applied");

            if (returnCount > FraudThresholds.HighReturnCount)
                flags.Add($"Customer has {returnCount} returns total");

            if (returnValue > FraudThresholds.HighReturnValue)
                flags.Add($"High value return: {returnValue:N0}");

            return BuildResult(FraudType.ReturnOffer, prediction.IsFraud, prediction.Probability, flags);
        }

        public async Task<FraudModelResult> CheckStoreAsync(FraudDetectionRequest request)
        {
            var todaySales = await _fraudData.GetStoreTodaySalesAsync(request.StoreId);
            var todayReturns = await _fraudData.GetStoreTodayReturnsAsync(request.StoreId);
            var todayInvoices = await _fraudData.GetStoreTodayInvoiceCountAsync(request.StoreId);
            var customerReturnCount = await _fraudData.GetCustomerReturnCountAsync(request.CustomerId);

            var prediction = _storeMl.Predict(new StoreFraudData
            {
                StoreId = request.StoreId,
                TotalSales = (float)(todaySales + request.Amount),
                TotalInvoices = todayInvoices + 1,
                ReturnCount = todayReturns,
                ReturnValue = request.IsReturn ? (float)request.Amount : 0f,
                CustomerReturnCount = customerReturnCount,
                DayType = EnumExtensions.GetDayType(request.TransactionDate).ToFloat(),
                SalesThreshold = FraudThresholds.StoreSalesThreshold,
                ReturnCountThreshold = FraudThresholds.StoreReturnCountThreshold,
                ReturnValueThreshold = FraudThresholds.StoreReturnValueThreshold
            });

            var flags = new List<string>();

            if (todaySales > (decimal)FraudThresholds.StoreSalesThreshold)
                flags.Add($"Store daily sales very high: {todaySales:N0}");

            if (todayReturns > FraudThresholds.StoreReturnCountThreshold)
                flags.Add($"Store has {todayReturns} returns today");

            return BuildResult(FraudType.StoreLevel, prediction.IsFraud, prediction.Probability, flags);
        }

        public async Task<FraudModelResult> CheckValidationAsync(FraudDetectionRequest request)
        {
            var storeType = await _fraudData.GetStoreTypeAsync(request.StoreId);

            var prediction = _validationMl.Predict(new ValidationFraudData
            {
                HourOfTransaction = request.TransactionDate.Hour,
                StoreOpenHour = FraudThresholds.StoreOpenHour,
                StoreCloseHour = FraudThresholds.StoreCloseHour,
                StoreType = storeType,
                PreviousStoreType = storeType,
                CustomerIsNew = await _fraudData.IsNewCustomerAsync(request.CustomerId) ? 1f : 0f,
                CrossStorePurchaseCount = await _fraudData.GetCrossStorePurchasesAsync(
                    request.CustomerId, request.StoreId),
                CrossStoreReturnCount = await _fraudData.GetCrossStoreReturnsAsync(
                    request.CustomerId, request.StoreId)
            });

            var flags = new List<string>();

            if (request.TransactionDate.Hour < FraudThresholds.StoreOpenHour
                || request.TransactionDate.Hour > FraudThresholds.StoreCloseHour)
                flags.Add($"Transaction outside store hours: {request.TransactionDate.Hour}:00");

            return BuildResult(FraudType.Validation, prediction.IsFraud, prediction.Probability, flags);
        }

        private async Task<bool> SaveAlertAsync(FraudDetectionRequest request, FraudModelResult r)
        {
            var today = DateTime.Today;
            var exists = _db.FraudAlerts.Any(a =>
                a.CustomerId == (int)request.CustomerId &&
                a.AlertType == r.FraudType.ToString() &&
                a.CreatedOn >= today &&
                !a.Deleted);

            if (exists) return false;

            _db.FraudAlerts.Add(new FraudAlert
            {
                AlertNo = $"{r.FraudType}-{request.CustomerId}-{DateTime.Now:yyyyMMddHHmmss}",
                AlertType = r.FraudType.ToString(),
                Category = r.FraudType.ToString(),
                Reason = r.Flags.Count > 0
                    ? string.Join(" | ", r.Flags)
                    : $"ML detected fraud - {r.ProbabilityPercent}%",
                CustomerId = (int)request.CustomerId,
                StoreId = (int)request.StoreId,
                EmployeeId = (int)request.EmployeeId,
                SalesOrderId = request.SalesOrderId,
                IsFraud = true,
                FraudProbabilityPercent = r.ProbabilityPercent,
                RiskLevel = r.RiskLevel,
                Status = AlertStatus.Open.ToDisplayString(),
                CreatedOn = DateTime.Now,
                Deleted = false
            });

            await _db.SaveChangesAsync();
            return true;
        }

        private static FraudModelResult BuildResult(
            FraudType type, bool isFraud, float probability, List<string> flags) =>
            new()
            {
                FraudType = type,
                IsFraud = isFraud,
                Probability = probability,
                ProbabilityPercent = MlHelper.ToPercent(probability),
                RiskLevel = EnumExtensions.ToRiskLevel(probability).ToDisplayString(),
                Flags = flags
            };

        private static FraudModelResult NotChecked(FraudType type) =>
            new() { FraudType = type, IsFraud = false, RiskLevel = RiskLevel.Safe.ToDisplayString() };
    }
}
