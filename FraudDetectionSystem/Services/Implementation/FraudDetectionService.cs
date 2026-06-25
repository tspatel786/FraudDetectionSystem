using FraudDetectionSystem.Data;
using FraudDetectionSystem.ML.Common;
using FraudDetectionSystem.ML.Models.Customer;
using FraudDetectionSystem.ML.Models.Employee;
using FraudDetectionSystem.ML.Models.Payment;
using FraudDetectionSystem.ML.Models.ReturnOffer;
using FraudDetectionSystem.ML.Models.Store;
using FraudDetectionSystem.ML.Models.Validation;
using FraudDetectionSystem.ML.Prediction;
using FraudDetectionSystem.Models;
using FraudDetectionSystem.Models.Dtos;
using FraudDetectionSystem.Models.Enum;
using FraudDetectionSystem.Services.Interface;
using Microsoft.Data.SqlClient;

namespace FraudDetectionSystem.Services.Implementation
{
    public class FraudDetectionService : IFraudDetectionService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly CustomerBehaviorPredictionService _customerMl;
        private readonly PaymentFraudPredictionService _paymentMl;
        private readonly EmployeeFraudPredictionService _employeeMl;
        private readonly ReturnOfferFraudPredictionService _returnOfferMl;
        private readonly StoreFraudPredictionService _storeMl;
        private readonly ValidationFraudPredictionService _validationMl;

        public FraudDetectionService(
            AppDbContext db,
            IConfiguration config,
            CustomerBehaviorPredictionService customerMl,
            PaymentFraudPredictionService paymentMl,
            EmployeeFraudPredictionService employeeMl,
            ReturnOfferFraudPredictionService returnOfferMl,
            StoreFraudPredictionService storeMl,
            ValidationFraudPredictionService validationMl)
        {
            _db = db;
            _config = config;
            _customerMl = customerMl;
            _paymentMl = paymentMl;
            _employeeMl = employeeMl;
            _returnOfferMl = returnOfferMl;
            _storeMl = storeMl;
            _validationMl = validationMl;
        }
        
        //Here i am calling all model and passing request to that model
        public async Task<FraudDetectionResult> CheckAllAsync(FraudDetectionRequest request)
        {
            var result = new FraudDetectionResult();
            var results = new List<FraudModelResult>();

            try
            {
                results.Add(await CheckCustomerBehaviorAsync(request));
                results.Add(await CheckPaymentAsync(request));
                results.Add(await CheckEmployeeAsync(request));
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
        private async Task<FraudModelResult> CheckCustomerBehaviorAsync(
            FraudDetectionRequest request)
        {
            var data = await LoadCustomerDataAsync(request.CustomerId);

            if (data == null)
                return NotChecked(FraudType.CustomerBehavior);

            var prediction = _customerMl.Predict(data);
            var flags = new List<string>();

            if (data.ReturnPercentage > 0.5f)
                flags.Add($"High return ratio: {data.ReturnPercentage:P0}");

            if (data.CashPaymentPercentage > 0.8f)
                flags.Add($"Mostly cash payments: {data.CashPaymentPercentage:P0}");

            if (data.ReturnAmount > data.TotalPurchaseAmount * 0.6f)
                flags.Add($"Return amount is 60%+ of total purchases");

            if (data.LedgerCreditAmount > data.LedgerDebitAmount * 2)
                flags.Add("Ledger credit far exceeds debit");

            if (data.AverageDaysBetweenPurchase < 3 && data.PurchaseCount > 5)
                flags.Add($"Buying every {data.AverageDaysBetweenPurchase:F1} days");

            return BuildResult(FraudType.CustomerBehavior, prediction.IsFraud,
                prediction.Probability, flags);
        }

      
        private Task<FraudModelResult> CheckPaymentAsync(FraudDetectionRequest request)
        {
            var nameMismatch = MlHelper.NameMismatchScore(
                request.InvoiceCustomerName, request.EmployeeName);

            var prediction = _paymentMl.Predict(new PaymentFraudData
            {
                Amount = (float)request.Amount,
                PaymentMethod = request.PaymentMethod,
                Hour = request.TransactionDate.Hour,
                NameMismatchScore = nameMismatch,
                IsCash = request.PaymentMethod
                                        .Equals("CASH", StringComparison.OrdinalIgnoreCase) ? 1f : 0f,
                IsReturn = request.IsReturn ? 1f : 0f
            });

            var flags = new List<string>();

            if (nameMismatch > 0.5f)
                flags.Add($"Name mismatch score: {nameMismatch:F2}");

            if (request.PaymentMethod.Equals("CASH", StringComparison.OrdinalIgnoreCase)
                && request.Amount > 100000)
                flags.Add($"Large cash payment: {request.Amount:N0}");

            if (request.IsReturn && request.PaymentMethod
                    .Equals("CASH", StringComparison.OrdinalIgnoreCase))
                flags.Add("Cash return — untraceable refund");

            return Task.FromResult(BuildResult(FraudType.Payment,
                prediction.IsFraud, prediction.Probability, flags));
        }

        private async Task<FraudModelResult> CheckEmployeeAsync(
    FraudDetectionRequest request)
        {
            var data = await LoadEmployeeDataAsync(request.EmployeeId);

            if (data == null)
                return NotChecked(FraudType.Employee);

            var prediction = _employeeMl.Predict(data);

            var flags = new List<string>();

            if (data.ReturnPercentage > 0.40f)
                flags.Add($"High return ratio : {data.ReturnPercentage:P0}");

            if (data.EmployeePurchaseCount > 2)
                flags.Add($"Employee purchased {data.EmployeePurchaseCount} times.");

            if (data.EmployeePurchaseAmount > 100000)
                flags.Add($"Employee purchase amount : {data.EmployeePurchaseAmount:N0}");

            if (data.IncentiveAmount > 10000)
                flags.Add($"High incentive : {data.IncentiveAmount:N0}");

            if (data.AverageInvoiceAmount > 100000)
                flags.Add($"Very high average invoice : {data.AverageInvoiceAmount:N0}");

            return BuildResult(
                FraudType.Employee,
                prediction.IsFraud,
                prediction.Probability,
                flags);
        }

        private async Task<FraudModelResult> CheckReturnOfferAsync(FraudDetectionRequest request)
        {
            var returnCount = await GetCustomerReturnCountAsync(request.CustomerId);
            var returnValue = request.IsReturn ? (float)request.Amount : 0f;

            var prediction = _returnOfferMl.Predict(new ReturnOfferFraudData
            {
                ReturnCount = returnCount,
                ReturnValue = returnValue,
                DaysSinceOffer = request.HasOffer ? 3f : 30f,
                HadOffer = request.HasOffer ? 1f : 0f,
                ReturnAfterOfferRatio = request.IsReturn && request.HasOffer ? 1f : 0f,
                SuspiciousPatternScore = Math.Min(1f, returnCount / 10f)
            });

            var flags = new List<string>();

            if (request.IsReturn && request.HasOffer)
                flags.Add("Return happening right after offer was applied");

            if (returnCount > 5)
                flags.Add($"Customer has {returnCount} returns total");

            if (returnValue > 50000)
                flags.Add($"High value return: {returnValue:N0}");

            return await Task.FromResult(BuildResult(FraudType.ReturnOffer,
                prediction.IsFraud, prediction.Probability, flags));
        }

      
        private async Task<FraudModelResult> CheckStoreAsync(FraudDetectionRequest request)
        {
            var todaySales = await GetStoreTodaySalesAsync(request.StoreId);
            var todayReturns = await GetStoreTodayReturnsAsync(request.StoreId);

            var prediction = _storeMl.Predict(new StoreFraudData
            {
                StoreId = request.StoreId,
                TotalSales = (float)(todaySales + request.Amount),
                TotalInvoices = 1,
                ReturnCount = (float)todayReturns,
                ReturnValue = request.IsReturn ? (float)request.Amount : 0f,
                CustomerReturnCount = await GetCustomerReturnCountAsync(request.CustomerId),
                DayType = GetDayType(request.TransactionDate),
                SalesThreshold = 500000f,
                ReturnCountThreshold = 10f,
                ReturnValueThreshold = 100000f
            });

            var flags = new List<string>();

            if (todaySales > 500000)
                flags.Add($"Store daily sales very high: {todaySales:N0}");

            if (todayReturns > 10)
                flags.Add($"Store has {todayReturns} returns today");

            return await Task.FromResult(BuildResult(FraudType.StoreLevel,
                prediction.IsFraud, prediction.Probability, flags));
        }

       
        private async Task<FraudModelResult> CheckValidationAsync(FraudDetectionRequest request)
        {
            var prediction = _validationMl.Predict(new ValidationFraudData
            {
                HourOfTransaction = request.TransactionDate.Hour,
                StoreOpenHour = 10f,
                StoreCloseHour = 21f,
                StoreType = 1f,
                PreviousStoreType = 1f,
                CustomerIsNew = await IsNewCustomerAsync(request.CustomerId) ? 1f : 0f,
                CrossStorePurchaseCount = await GetCrossStorePurchasesAsync(
                                            request.CustomerId, request.StoreId),
                CrossStoreReturnCount = await GetCrossStoreReturnsAsync(
                                            request.CustomerId, request.StoreId)
            });

            var flags = new List<string>();

            if (request.TransactionDate.Hour < 10 || request.TransactionDate.Hour > 21)
                flags.Add($"Transaction outside store hours: {request.TransactionDate.Hour}:00");

            return await Task.FromResult(BuildResult(FraudType.Validation,
                prediction.IsFraud, prediction.Probability, flags));
        }

        private async Task<bool> SaveAlertAsync(
            FraudDetectionRequest request, FraudModelResult r)
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
                Status = "Open",
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
                RiskLevel = GetRiskLevel(probability),
                Flags = flags
            };

        private static FraudModelResult NotChecked(FraudType type) =>
            new() { FraudType = type, IsFraud = false, RiskLevel = "SAFE" };

        private static string GetRiskLevel(float probability) => probability switch
        {
            >= 0.80f => "HIGH",
            >= 0.50f => "MEDIUM",
            >= 0.30f => "LOW",
            _ => "SAFE"
        };

        private static float GetDayType(DateTime date) =>
            date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ? 2f :
            date.Month is 10 or 11 ? 3f : 1f;

        // ── DB helper methods 

        private async Task<CustomerBehaviorData?> LoadCustomerDataAsync(long customerId)
        {
            var cs = _config.GetConnectionString("DefaultConnection")!;

            const string sql = """
                SELECT
                    CAST(COUNT(DISTINCT so.Id) AS FLOAT)                    AS PurchaseCount,
                    COALESCE(SUM(so.FinalSoAmount), 0)                      AS TotalPurchaseAmount,
                    COALESCE(AVG(so.FinalSoAmount), 0)                      AS AveragePurchaseAmount,
                    CAST(COUNT(DISTINCT rso.Id) AS FLOAT)                   AS ReturnCount,
                    COALESCE(SUM(rso.TotalReturnSaleOrderAmount), 0)        AS ReturnAmount,
                    CASE WHEN COUNT(DISTINCT so.Id)=0 THEN 0
                         ELSE CAST(COUNT(DISTINCT rso.Id) AS FLOAT)
                              /COUNT(DISTINCT so.Id) END                    AS ReturnPercentage,
                    COALESCE(SUM(CASE WHEN cl.TransactionType='CASH' THEN 1.0 ELSE 0 END)
                        /NULLIF(COUNT(DISTINCT so.Id),0),0)                 AS CashPaymentPercentage,
                    COALESCE(SUM(CASE WHEN cl.TransactionType='BANK' THEN 1.0 ELSE 0 END)
                        /NULLIF(COUNT(DISTINCT so.Id),0),0)                 AS BankPaymentPercentage,
                    COALESCE(SUM(CASE WHEN cl.TransactionType='CARD' THEN 1.0 ELSE 0 END)
                        /NULLIF(COUNT(DISTINCT so.Id),0),0)                 AS CardPaymentPercentage,
                    COALESCE(DATEDIFF(DAY,MIN(so.SoDate),MAX(so.SoDate))
                        /NULLIF(COUNT(DISTINCT so.Id)-1,0),0)               AS AverageDaysBetweenPurchase,
                    COALESCE(DATEDIFF(DAY,MAX(so.SoDate),GETDATE()),0)      AS LastPurchaseDays,
                    COALESCE(SUM(CASE WHEN cl.TransactionType='DEBIT'
                        THEN cl.TransactionAmount ELSE 0 END),0)            AS LedgerDebitAmount,
                    COALESCE(SUM(CASE WHEN cl.TransactionType='CREDIT'
                        THEN cl.TransactionAmount ELSE 0 END),0)            AS LedgerCreditAmount,
                    CAST(0 AS BIT)                                          AS IsFraud
                FROM Customer c
                LEFT JOIN SalesOrder so ON so.CustomerId=c.Id AND so.Deleted=0
                LEFT JOIN ReturnSalesOrder rso ON rso.CustomerId=c.Id AND rso.Deleted=0
                LEFT JOIN CustomerLedger cl ON cl.CustomerId=c.Id AND cl.Deleted=0
                WHERE c.Id=@id AND c.Deleted=0
                GROUP BY c.Id
                """;

            await using var conn = new SqlConnection(cs);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", customerId);
            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            var d = new CustomerBehaviorData
            {
                PurchaseCount = Convert.ToSingle(reader["PurchaseCount"]),
                TotalPurchaseAmount = Convert.ToSingle(reader["TotalPurchaseAmount"]),
                AveragePurchaseAmount = Convert.ToSingle(reader["AveragePurchaseAmount"]),
                ReturnCount = Convert.ToSingle(reader["ReturnCount"]),
                ReturnAmount = Convert.ToSingle(reader["ReturnAmount"]),
                ReturnPercentage = Convert.ToSingle(reader["ReturnPercentage"]),
                CashPaymentPercentage = Convert.ToSingle(reader["CashPaymentPercentage"]),
                BankPaymentPercentage = Convert.ToSingle(reader["BankPaymentPercentage"]),
                CardPaymentPercentage = Convert.ToSingle(reader["CardPaymentPercentage"]),
                AverageDaysBetweenPurchase = Convert.ToSingle(reader["AverageDaysBetweenPurchase"]),
                LastPurchaseDays = Convert.ToSingle(reader["LastPurchaseDays"]),
                LedgerDebitAmount = Convert.ToSingle(reader["LedgerDebitAmount"]),
                LedgerCreditAmount = Convert.ToSingle(reader["LedgerCreditAmount"]),
                IsFraud = false
            };

            return d.PurchaseCount > 0 ? d : null;
        }

        private async Task<EmployeeFraudData?> LoadEmployeeDataAsync(long employeeId)
        {
            var cs = _config.GetConnectionString("DefaultConnection")!;

            const string sql = """
        SELECT
            CAST(sp.Id AS FLOAT) AS EmployeeId,

            COALESCE(SUM(so.FinalSoAmount), 0) AS TotalSales,

            CAST(COUNT(DISTINCT so.Id) AS FLOAT) AS TotalInvoices,

            COALESCE(AVG(so.FinalSoAmount), 0) AS AverageInvoiceAmount,

            CAST(COUNT(DISTINCT rso.Id) AS FLOAT) AS ReturnCount,

            COALESCE(SUM(rso.TotalReturnSaleOrderAmount), 0) AS ReturnAmount,

            CASE
                WHEN COUNT(DISTINCT so.Id) = 0 THEN 0
                ELSE CAST(COUNT(DISTINCT rso.Id) AS FLOAT)
                     / COUNT(DISTINCT so.Id)
            END AS ReturnPercentage,

            CAST(
                COUNT(DISTINCT CASE
                    WHEN so.CustomerId IN
                    (
                        SELECT Id
                        FROM Customer
                        WHERE MobileNo = sp.MobileNo
                    )
                    THEN so.Id
                END) AS FLOAT
            ) AS EmployeePurchaseCount,

            COALESCE(
                SUM(
                    CASE
                        WHEN so.CustomerId IN
                        (
                            SELECT Id
                            FROM Customer
                            WHERE MobileNo = sp.MobileNo
                        )
                        THEN so.FinalSoAmount
                        ELSE 0
                    END
                ),
            0) AS EmployeePurchaseAmount,

            COALESCE(SUM(so.FinalSoAmount) * 0.02,0) AS IncentiveAmount,

            CAST(0.02 AS FLOAT) AS IncentiveRatio,

            CAST(0 AS BIT) AS IsFraud

        FROM SalesPerson sp

        LEFT JOIN SalesOrder so
            ON so.SalesPersonId = sp.Id
           AND so.Deleted = 0

        LEFT JOIN ReturnSalesOrder rso
            ON rso.SalesPersonId = sp.Id
           AND rso.Deleted = 0

        WHERE sp.Id = @EmployeeId
          AND sp.Deleted = 0

        GROUP BY
            sp.Id,
            sp.MobileNo
        """;

            await using var conn = new SqlConnection(cs);
            await conn.OpenAsync();

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            var data = new EmployeeFraudData
            {
                EmployeeId = Convert.ToSingle(reader["EmployeeId"]),
                TotalSales = Convert.ToSingle(reader["TotalSales"]),
                TotalInvoices = Convert.ToSingle(reader["TotalInvoices"]),
                AverageInvoiceAmount = Convert.ToSingle(reader["AverageInvoiceAmount"]),
                ReturnCount = Convert.ToSingle(reader["ReturnCount"]),
                ReturnAmount = Convert.ToSingle(reader["ReturnAmount"]),
                ReturnPercentage = Convert.ToSingle(reader["ReturnPercentage"]),
                EmployeePurchaseCount = Convert.ToSingle(reader["EmployeePurchaseCount"]),
                EmployeePurchaseAmount = Convert.ToSingle(reader["EmployeePurchaseAmount"]),
                IncentiveAmount = Convert.ToSingle(reader["IncentiveAmount"]),
                IncentiveRatio = Convert.ToSingle(reader["IncentiveRatio"]),
                IsFraud = false
            };

            return data.TotalInvoices > 0 ? data : null;
        }

        private async Task<float> GetCustomerReturnCountAsync(long customerId)
        {
            var cs = _config.GetConnectionString("DefaultConnection")!;
            await using var conn = new SqlConnection(cs);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM ReturnSalesOrder WHERE CustomerId=@id AND Deleted=0", conn);
            cmd.Parameters.AddWithValue("@id", customerId);
            return Convert.ToSingle(await cmd.ExecuteScalarAsync());
        }

        private async Task<decimal> GetStoreTodaySalesAsync(long storeId)
        {
            var cs = _config.GetConnectionString("DefaultConnection")!;
            await using var conn = new SqlConnection(cs);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(
                "SELECT COALESCE(SUM(FinalSoAmount),0) FROM SalesOrder WHERE StoreId=@id AND CAST(SoDate AS DATE)=CAST(GETDATE() AS DATE) AND Deleted=0", conn);
            cmd.Parameters.AddWithValue("@id", storeId);
            return Convert.ToDecimal(await cmd.ExecuteScalarAsync());
        }

        private async Task<int> GetStoreTodayReturnsAsync(long storeId)
        {
            var cs = _config.GetConnectionString("DefaultConnection")!;
            await using var conn = new SqlConnection(cs);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM ReturnSalesOrder WHERE StoreId=@id AND CAST(ReturnSaleOrderDate AS DATE)=CAST(GETDATE() AS DATE) AND Deleted=0", conn);
            cmd.Parameters.AddWithValue("@id", storeId);
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        private async Task<bool> IsNewCustomerAsync(long customerId)
        {
            var cs = _config.GetConnectionString("DefaultConnection")!;
            await using var conn = new SqlConnection(cs);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(
                "SELECT CreatedOn FROM Customer WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", customerId);
            var createdOn = await cmd.ExecuteScalarAsync();
            if (createdOn == null) return false;
            return (DateTime.UtcNow - Convert.ToDateTime(createdOn)).TotalDays < 30;
        }

        private async Task<float> GetCrossStorePurchasesAsync(long customerId, long storeId)
        {
            var cs = _config.GetConnectionString("DefaultConnection")!;
            await using var conn = new SqlConnection(cs);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM SalesOrder WHERE CustomerId=@cid AND StoreId!=@sid AND Deleted=0", conn);
            cmd.Parameters.AddWithValue("@cid", customerId);
            cmd.Parameters.AddWithValue("@sid", storeId);
            return Convert.ToSingle(await cmd.ExecuteScalarAsync());
        }

        private async Task<float> GetCrossStoreReturnsAsync(long customerId, long storeId)
        {
            var cs = _config.GetConnectionString("DefaultConnection")!;
            await using var conn = new SqlConnection(cs);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM ReturnSalesOrder WHERE CustomerId=@cid AND StoreId!=@sid AND Deleted=0", conn);
            cmd.Parameters.AddWithValue("@cid", customerId);
            cmd.Parameters.AddWithValue("@sid", storeId);
            return Convert.ToSingle(await cmd.ExecuteScalarAsync());
        }
    }
}