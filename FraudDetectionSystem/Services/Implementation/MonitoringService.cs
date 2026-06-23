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
using FraudDetectionSystem.Models.Enums;
using FraudDetectionSystem.Repository.Interface;
using FraudDetectionSystem.Services.Interface;

namespace FraudDetectionSystem.Services.Implementation
{
    public class MonitoringService : IMonitoringService
    {
        private readonly IMonitoringRepository _monitoringRepo;
        private readonly IStoreRepository _storeRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IFraudAlertRepository _alertRepo;
        private readonly StoreFraudPredictionService _storeMl;
        private readonly CustomerBehaviorPredictionService _customerMl;
        private readonly PaymentFraudPredictionService _paymentMl;
        private readonly EmployeeFraudPredictionService _employeeMl;
        private readonly ReturnOfferFraudPredictionService _returnOfferMl;
        private readonly ValidationFraudPredictionService _validationMl;

        public MonitoringService(
            IMonitoringRepository monitoringRepo,
            IStoreRepository storeRepo,
            IEmployeeRepository employeeRepo,
            IFraudAlertRepository alertRepo,
            StoreFraudPredictionService storeMl,
            CustomerBehaviorPredictionService customerMl,
            PaymentFraudPredictionService paymentMl,
            EmployeeFraudPredictionService employeeMl,
            ReturnOfferFraudPredictionService returnOfferMl,
            ValidationFraudPredictionService validationMl)
        {
            _monitoringRepo = monitoringRepo;
            _storeRepo = storeRepo;
            _employeeRepo = employeeRepo;
            _alertRepo = alertRepo;
            _storeMl = storeMl;
            _customerMl = customerMl;
            _paymentMl = paymentMl;
            _employeeMl = employeeMl;
            _returnOfferMl = returnOfferMl;
            _validationMl = validationMl;
        }

        public async Task<ProcessInvoiceResponse> ProcessInvoiceAsync(ProcessInvoiceRequest request)
        {
            var customer = await _monitoringRepo.GetCustomerAsync(request.CustomerId)
                ?? throw new InvalidOperationException($"Customer {request.CustomerId} not found.");
            var store = await _storeRepo.GetByIdAsync(request.StoreId)
                ?? throw new InvalidOperationException($"Store {request.StoreId} not found.");
            var employee = await _employeeRepo.GetByIdAsync(request.EmployeeId)
                ?? throw new InvalidOperationException($"Employee {request.EmployeeId} not found.");

            var date = request.TransactionDate.Date;
            var dayType = GetDayType(date);
            var threshold = await _storeRepo.GetThresholdAsync(store.Id, dayType);

            var totalSales = await _monitoringRepo.GetStoreDailySalesAsync(store.Id, date);
            var totalInvoices = await _monitoringRepo.GetStoreDailyInvoiceCountAsync(store.Id, date);
            var returnCount = await _monitoringRepo.GetStoreDailyReturnCountAsync(store.Id, date);
            var returnValue = await _monitoringRepo.GetStoreDailyReturnValueAsync(store.Id, date);

            if (request.TransactionType == TransactionType.Purchase)
            {
                totalSales += request.Amount;
                totalInvoices += 1;
            }
            else
            {
                returnCount += 1;
                returnValue += request.Amount;
                customer.ReturnCount += 1;
            }

            UpdateCustomerMetrics(customer, request);

            var storePrediction = _storeMl.Predict(new StoreFraudData
            {
                StoreId = store.Id,
                TotalSales = (float)totalSales,
                TotalInvoices = totalInvoices,
                ReturnCount = returnCount,
                ReturnValue = (float)returnValue,
                CustomerReturnCount = customer.ReturnCount,
                DayType = (float)dayType,
                SalesThreshold = (float)(threshold?.SalesThreshold ?? 300000),
                ReturnCountThreshold = threshold?.ReturnCountThreshold ?? 5,
                ReturnValueThreshold = (float)(threshold?.ReturnValueThreshold ?? 50000)
            });

            var categoryShift = ComputeCategoryShiftScore(customer, request.ItemCategory);
            var customerPrediction = _customerMl.Predict(new CustomerBehaviorData
            {
                VisitCount = customer.VisitCount,
                InvoiceCount = customer.InvoiceCount,
                AvgPurchase = (float)customer.AveragePurchase,
                LifetimeValue = (float)customer.LifetimeValue,
                HniScore = customer.IsHni ? 1f : (float)Math.Min(1, (double)customer.LifetimeValue / 500000),
                GoldRatio = customer.GoldPurchaseRatio,
                DiamondRatio = customer.DiamondPurchaseRatio,
                CoinRatio = customer.CoinPurchaseRatio,
                JewelleryRatio = customer.JewelleryPurchaseRatio,
                CategoryShiftScore = categoryShift,
                PurchasePatternScore = (float)Math.Min(1, customer.InvoiceCount / 50.0)
            });

            var nameMismatch = MlHelper.NameMismatchScore(request.InvoiceCustomerName, request.PaymentCustomerName);
            var paymentPrediction = _paymentMl.Predict(new PaymentFraudData
            {
                Amount = (float)request.Amount,
                PaymentMethod = request.PaymentMethod,
                Hour = request.TransactionDate.Hour,
                NameMismatchScore = nameMismatch,
                IsCash = request.PaymentMethod.Equals("CASH", StringComparison.OrdinalIgnoreCase) ? 1f : 0f,
                IsReturn = request.TransactionType == TransactionType.Return ? 1f : 0f
            });

            var empNameMatch = MlHelper.NameMismatchScore(employee.Name, request.InvoiceCustomerName);
            var isEmployeePurchase = customer.Name.Equals(employee.Name, StringComparison.OrdinalIgnoreCase) ? 1f : 0f;
            var employeePrediction = _employeeMl.Predict(new EmployeeFraudData
            {
                EmployeeId = employee.Id,
                SalesAmount = (float)request.Amount,
                NameMatchScore = empNameMatch,
                IsEmployeePurchase = isEmployeePurchase,
                EmployeePurchaseAmount = isEmployeePurchase * (float)request.Amount,
                IncentiveAmount = (float)(request.Amount * 0.02m),
                IncentiveRatio = 0.02f
            });

            var returnOfferPrediction = _returnOfferMl.Predict(new ReturnOfferFraudData
            {
                ReturnCount = customer.ReturnCount,
                ReturnValue = (float)returnValue,
                DaysSinceOffer = request.HasOfferApplied ? 3f : 30f,
                HadOffer = request.HasOfferApplied ? 1f : 0f,
                ReturnAfterOfferRatio = request.TransactionType == TransactionType.Return && request.HasOfferApplied ? 1f : 0f,
                SuspiciousPatternScore = (float)Math.Min(1, customer.ReturnCount / 10.0)
            });

            var crossStorePurchases = await _monitoringRepo.GetCrossStorePurchaseCountAsync(customer.Id, store.Id);
            var crossStoreReturns = await _monitoringRepo.GetCrossStoreReturnCountAsync(customer.Id, store.Id);
            var isNewCustomer = (DateTime.UtcNow - customer.FirstVisitDate).TotalDays < 30 ? 1f : 0f;

            var validationPrediction = _validationMl.Predict(new ValidationFraudData
            {
                HourOfTransaction = request.TransactionDate.Hour,
                StoreOpenHour = store.OpenHour,
                StoreCloseHour = store.CloseHour,
                StoreType = (float)store.StoreType,
                PreviousStoreType = (float)(store.PreviousStoreType ?? store.StoreType),
                CustomerIsNew = isNewCustomer,
                CrossStorePurchaseCount = crossStorePurchases,
                CrossStoreReturnCount = crossStoreReturns
            });

            var checks = BuildChecks(storePrediction, customerPrediction, paymentPrediction,
                employeePrediction, returnOfferPrediction, validationPrediction);

            var overallPercent = checks.Max(c => c.FraudProbabilityPercent);
            var isFraud = checks.Any(c => c.IsFraud);

            var transaction = new Transaction
            {
                InvoiceNumber = request.InvoiceNumber,
                CustomerId = request.CustomerId,
                StoreId = request.StoreId,
                EmployeeId = request.EmployeeId,
                TransactionType = request.TransactionType,
                ItemCategory = request.ItemCategory,
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod,
                Date = request.TransactionDate,
                InvoiceName = request.InvoiceCustomerName,
                PaymentName = request.PaymentCustomerName,
                HasOfferApplied = request.HasOfferApplied,
                IsFraud = isFraud,
                FraudProbabilityPercent = overallPercent
            };

            await _monitoringRepo.AddTransactionAsync(transaction);
            await _monitoringRepo.UpdateCustomerAsync(customer);

            await _storeRepo.UpsertDailySaleAsync(new StoreDailySale
            {
                StoreId = store.Id,
                Date = date,
                DayType = dayType,
                TotalSales = request.TransactionType == TransactionType.Purchase ? request.Amount : 0,
                TotalInvoices = request.TransactionType == TransactionType.Purchase ? 1 : 0
            });

            if (request.TransactionType == TransactionType.Return)
            {
                await _storeRepo.UpsertReturnMonitoringAsync(new StoreReturnMonitoring
                {
                    StoreId = store.Id,
                    Date = date,
                    ReturnCount = 1,
                    ReturnValue = request.Amount,
                    FrequentReturnCustomerCount = customer.ReturnCount > 3 ? 1 : 0
                });
            }

            foreach (var check in checks.Where(c => c.IsFraud))
            {
                await _alertRepo.AddAsync(new FraudAlert
                {
                    AlertType = check.AlertType,
                    Category = check.Category,
                    Reason = $"{check.CheckName} - ML probability {check.FraudProbabilityPercent}%",
                    CustomerId = customer.Id,
                    StoreId = store.Id,
                    EmployeeId = employee.Id,
                    TransactionId = transaction.Id,
                    IsFraud = check.IsFraud,
                    FraudProbabilityPercent = check.FraudProbabilityPercent,
                    CreatedOn = DateTime.UtcNow
                });

                if (check.Category == FraudCategory.StoreLevel.ToString())
                {
                    await _storeRepo.AddStoreAlertAsync(new StoreFraudAlert
                    {
                        StoreId = store.Id,
                        AlertType = check.AlertType,
                        Category = check.Category,
                        Reason = check.CheckName,
                        IsFraud = check.IsFraud,
                        FraudProbabilityPercent = check.FraudProbabilityPercent
                    });
                }
            }

            return new ProcessInvoiceResponse
            {
                TransactionId = transaction.Id,
                InvoiceNumber = transaction.InvoiceNumber,
                IsFraud = isFraud,
                OverallFraudProbabilityPercent = overallPercent,
                Checks = checks
            };
        }

        public Task<List<Transaction>> GetTransactionsWithScoresAsync() =>
            _monitoringRepo.GetAllTransactionsAsync();

        private static void UpdateCustomerMetrics(Customer customer, ProcessInvoiceRequest request)
        {
            customer.VisitCount += 1;
            if (request.TransactionType == TransactionType.Purchase)
            {
                customer.InvoiceCount += 1;
                customer.LifetimeValue += request.Amount;
                customer.AveragePurchase = customer.InvoiceCount > 0
                    ? customer.LifetimeValue / customer.InvoiceCount
                    : 0;
                customer.IsHni = customer.LifetimeValue >= 500000;

                var total = customer.GoldPurchaseRatio + customer.DiamondPurchaseRatio +
                            customer.CoinPurchaseRatio + customer.JewelleryPurchaseRatio + 1;
                switch (request.ItemCategory)
                {
                    case ItemCategory.Gold: customer.GoldPurchaseRatio = (customer.GoldPurchaseRatio * (total - 1) + 1) / total; break;
                    case ItemCategory.Diamond: customer.DiamondPurchaseRatio = (customer.DiamondPurchaseRatio * (total - 1) + 1) / total; break;
                    case ItemCategory.Coin: customer.CoinPurchaseRatio = (customer.CoinPurchaseRatio * (total - 1) + 1) / total; break;
                    case ItemCategory.Jewellery: customer.JewelleryPurchaseRatio = (customer.JewelleryPurchaseRatio * (total - 1) + 1) / total; break;
                }
            }

            customer.UpdatedAt = DateTime.UtcNow;
        }

        private static float ComputeCategoryShiftScore(Customer customer, ItemCategory category)
        {
            return category switch
            {
                ItemCategory.Diamond when customer.GoldPurchaseRatio > 0.5f => 0.8f,
                ItemCategory.Jewellery when customer.CoinPurchaseRatio > 0.5f => 0.75f,
                _ => 0.1f
            };
        }

        private static DayType GetDayType(DateTime date)
        {
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                return DayType.Weekend;
            if (date.Month == 10 || date.Month == 11)
                return DayType.Festival;
            return DayType.Regular;
        }

        private static List<FraudCheckResultDto> BuildChecks(
            StoreFraudPrediction store,
            CustomerBehaviorPrediction customer,
            PaymentFraudPrediction payment,
            EmployeeFraudPrediction employee,
            ReturnOfferFraudPrediction returnOffer,
            ValidationFraudPrediction validation)
        {
            var checks = new List<FraudCheckResultDto>();

            AddSectionChecks(checks, FraudCategory.StoreLevel, store.IsFraud, store.Probability, "STORE_ML",
                "Daily Sales Pattern Monitoring",
                "Regular Day / Weekend / Festival Threshold Configuration",
                "Daily Return Count & Value Monitoring",
                "Frequent Return Customer Detection");

            AddSectionChecks(checks, FraudCategory.CustomerBehavior, customer.IsFraud, customer.Probability, "CUSTOMER_ML",
                "Customer Visit & Invoice Pattern Analysis",
                "Average Purchase & Lifetime Value Analysis",
                "HNI Customer Monitoring",
                "Gold to Diamond Purchase Shift Detection",
                "Coin to Jewellery Purchase Shift Detection",
                "Customer Purchase Pattern Tracking");

            AddSectionChecks(checks, FraudCategory.Payment, payment.IsFraud, payment.Probability, "PAYMENT_ML",
                "Payment Method Tracking",
                "Invoice Name vs Payment Name Validation",
                "Cash Purchase & Return Tracking");

            AddSectionChecks(checks, FraudCategory.Employee, employee.IsFraud, employee.Probability, "EMPLOYEE_ML",
                "Salesperson and Customer Name Matching",
                "Employee Purchase Monitoring",
                "High Value Employee Purchase Alerts",
                "Incentive-Based Sales Monitoring");

            AddSectionChecks(checks, FraudCategory.ReturnOffer, returnOffer.IsFraud, returnOffer.Probability, "RETURN_OFFER_ML",
                "Return Analysis After Offers",
                "Suspicious Return Pattern Detection");

            AddSectionChecks(checks, FraudCategory.StoreCustomerValidation, validation.IsFraud, validation.Probability, "VALIDATION_ML",
                "Store Operating Period Validation",
                "FOFO / COCO Store Conversion Tracking",
                "Existing vs New Customer Validation",
                "Cross Store Purchase & Return Tracking");

            return checks;
        }

        private static void AddSectionChecks(
            List<FraudCheckResultDto> checks,
            FraudCategory category,
            bool isFraud,
            float probability,
            string alertType,
            params string[] checkNames)
        {
            var percent = MlHelper.ToPercent(probability);
            foreach (var name in checkNames)
            {
                checks.Add(new FraudCheckResultDto
                {
                    Category = category.ToString(),
                    CheckName = name,
                    IsFraud = isFraud,
                    FraudProbabilityPercent = percent,
                    AlertType = alertType
                });
            }
        }
    }
}
