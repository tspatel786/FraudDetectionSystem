using FraudDetectionSystem.Data;
using FraudDetectionSystem.ML.Common;
using FraudDetectionSystem.ML.Models.Customer;
using FraudDetectionSystem.ML.Models.Employee;
using FraudDetectionSystem.ML.Models.Payment;
using FraudDetectionSystem.ML.Models.ReturnOffer;
using FraudDetectionSystem.ML.Models.Store;
using FraudDetectionSystem.ML.Models.Validation;
using FraudDetectionSystem.Models.Dtos;
using FraudDetectionSystem.Models.Enum;
using FraudDetectionSystem.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace FraudDetectionSystem.Repository.Implementation
{
    public class FraudDataRepository : IFraudDataRepository
    {
        private readonly PosDbContext _posDb;

        public FraudDataRepository(PosDbContext posDb)
        {
            _posDb = posDb;
        }

        public async Task<CustomerBehaviorData?> GetCustomerBehaviorDataAsync(long customerId)
        {
            var customer = await _posDb.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == customerId && !c.Deleted);

            if (customer == null) return null;

            var purchases = await _posDb.SalesOrders
                .AsNoTracking()
                .Where(so => so.CustomerId == customerId && !so.Deleted)
                .ToListAsync();

            if (purchases.Count == 0) return null;

            var returns = await _posDb.ReturnSalesOrders
                .AsNoTracking()
                .Where(r => r.CustomerId == customerId && !r.Deleted)
                .ToListAsync();

            var ledger = await _posDb.CustomerLedgers
                .AsNoTracking()
                .Where(cl => cl.CustomerId == customerId && !cl.Deleted)
                .ToListAsync();

            return BuildCustomerBehaviorData(purchases, returns, ledger, labelForTraining: false);
        }

        public List<CustomerBehaviorData> LoadCustomerBehaviorTrainingData()
        {
            var purchasesByCustomer = _posDb.SalesOrders
                .AsNoTracking()
                .Where(so => !so.Deleted && so.CustomerId != null)
                .GroupBy(so => so.CustomerId!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            if (purchasesByCustomer.Count == 0)
                return new List<CustomerBehaviorData>();

            var customerIds = purchasesByCustomer.Keys.ToList();

            var returnsByCustomer = _posDb.ReturnSalesOrders
                .AsNoTracking()
                .Where(r => !r.Deleted && r.CustomerId != null && customerIds.Contains(r.CustomerId.Value))
                .GroupBy(r => r.CustomerId!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            var ledgerByCustomer = _posDb.CustomerLedgers
                .AsNoTracking()
                .Where(cl => !cl.Deleted && customerIds.Contains(cl.CustomerId))
                .GroupBy(cl => cl.CustomerId)
                .ToDictionary(g => g.Key, g => g.ToList());

            return customerIds
                .Select(customerId =>
                {
                    var purchases = purchasesByCustomer[customerId];
                    returnsByCustomer.TryGetValue(customerId, out var returns);
                    ledgerByCustomer.TryGetValue(customerId, out var ledger);
                    return BuildCustomerBehaviorData(
                        purchases,
                        returns ?? new List<Data.Entities.ReturnSalesOrder>(),
                        ledger ?? new List<Data.Entities.CustomerLedger>(),
                        labelForTraining: true);
                })
                .ToList();
        }

        public async Task<EmployeeFraudData?> GetEmployeeFraudDataAsync(long employeeId)
        {
            var employee = await _posDb.SalesPersons
                .AsNoTracking()
                .FirstOrDefaultAsync(sp => sp.Id == employeeId && !sp.Deleted);

            if (employee == null) return null;

            var sales = await _posDb.SalesOrders
                .AsNoTracking()
                .Where(so => so.SalesPersonId == employeeId && !so.Deleted)
                .ToListAsync();

            if (sales.Count == 0) return null;

            var returns = await _posDb.ReturnSalesOrders
                .AsNoTracking()
                .Where(r => r.SalesPersonId == employeeId && !r.Deleted)
                .ToListAsync();

            var employeeCustomerIds = await _posDb.Customers
                .AsNoTracking()
                .Where(c => !c.Deleted && c.MobileNo == employee.MobileNo)
                .Select(c => c.Id)
                .ToListAsync();

            return BuildEmployeeFraudData(employee, sales, returns, employeeCustomerIds, labelForTraining: false);
        }

        public List<EmployeeFraudData> LoadEmployeeFraudTrainingData()
        {
            var employees = _posDb.SalesPersons
                .AsNoTracking()
                .Where(sp => !sp.Deleted)
                .ToList();

            var salesByEmployee = _posDb.SalesOrders
                .AsNoTracking()
                .Where(so => !so.Deleted && so.SalesPersonId != null)
                .GroupBy(so => so.SalesPersonId!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            var returnsByEmployee = _posDb.ReturnSalesOrders
                .AsNoTracking()
                .Where(r => !r.Deleted && r.SalesPersonId != null)
                .GroupBy(r => r.SalesPersonId!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            var customersByMobile = _posDb.Customers
                .AsNoTracking()
                .Where(c => !c.Deleted)
                .GroupBy(c => c.MobileNo)
                .ToDictionary(g => g.Key, g => g.Select(c => c.Id).ToList());

            return employees
                .Where(e => salesByEmployee.ContainsKey(e.Id))
                .Select(employee =>
                {
                    var sales = salesByEmployee[employee.Id];
                    returnsByEmployee.TryGetValue(employee.Id, out var returns);
                    customersByMobile.TryGetValue(employee.MobileNo ?? string.Empty, out var customerIds);
                    return BuildEmployeeFraudData(
                        employee,
                        sales,
                        returns ?? new List<Data.Entities.ReturnSalesOrder>(),
                        customerIds ?? new List<long>(),
                        labelForTraining: true);
                })
                .ToList();
        }

        public List<StoreFraudData> LoadStoreFraudTrainingData()
        {
            var today = DateTime.Today;

            var dailySales = _posDb.SalesOrders
                .AsNoTracking()
                .Where(so => !so.Deleted)
                .GroupBy(so => new { so.StoreId, Date = so.SoDate.Date })
                .Select(g => new
                {
                    g.Key.StoreId,
                    g.Key.Date,
                    TotalSales = g.Sum(x => x.FinalSoAmount),
                    TotalInvoices = g.Count()
                })
                .ToList();

            var dailyReturns = _posDb.ReturnSalesOrders
                .AsNoTracking()
                .Where(r => !r.Deleted)
                .GroupBy(r => new { r.StoreId, Date = r.ReturnSaleOrderDate.Date })
                .Select(g => new
                {
                    g.Key.StoreId,
                    g.Key.Date,
                    ReturnCount = g.Count(),
                    ReturnValue = g.Sum(x => x.TotalReturnSaleOrderAmount)
                })
                .ToDictionary(x => (x.StoreId, x.Date), x => x);

            return dailySales.Select(sale =>
            {
                dailyReturns.TryGetValue((sale.StoreId, sale.Date), out var ret);
                var returnCount = ret?.ReturnCount ?? 0;
                var returnValue = (float)(ret?.ReturnValue ?? 0);
                var dayType = EnumExtensions.GetDayType(sale.Date);

                var isFraud =
                    returnCount > FraudThresholds.StoreReturnCountThreshold
                    || returnValue > FraudThresholds.StoreReturnValueThreshold
                    || (float)sale.TotalSales > FraudThresholds.StoreSalesThreshold * 2;

                return new StoreFraudData
                {
                    StoreId = sale.StoreId,
                    TotalSales = (float)sale.TotalSales,
                    TotalInvoices = sale.TotalInvoices,
                    ReturnCount = returnCount,
                    ReturnValue = returnValue,
                    CustomerReturnCount = returnCount,
                    DayType = dayType.ToFloat(),
                    SalesThreshold = FraudThresholds.StoreSalesThreshold,
                    ReturnCountThreshold = FraudThresholds.StoreReturnCountThreshold,
                    ReturnValueThreshold = FraudThresholds.StoreReturnValueThreshold,
                    IsFraud = isFraud
                };
            }).ToList();
        }

        public List<PaymentFraudData> LoadPaymentFraudTrainingData()
        {
            var sales = _posDb.SalesOrders
                .AsNoTracking()
                .Where(so => !so.Deleted && so.CustomerId != null)
                .Include(so => so.Customer)
                .ToList();

            var ledgerByOrder = _posDb.CustomerLedgers
                .AsNoTracking()
                .Where(cl => !cl.Deleted && cl.SalesOrderId != null)
                .GroupBy(cl => cl.SalesOrderId!.Value)
                .ToDictionary(g => g.Key, g => g.First().TransactionType ?? PaymentMethodType.Other.ToString());

            var records = new List<PaymentFraudData>();

            foreach (var order in sales)
            {
                var paymentMethod = ledgerByOrder.TryGetValue(order.Id, out var pm)
                    ? pm
                    : PaymentMethodType.Cash.ToString();

                var customerName = $"{order.Customer?.FirstName} {order.Customer?.LastName}".Trim();
                var nameMismatch = MlHelper.NameMismatchScore(customerName, customerName);
                var isCash = EnumExtensions.IsCashPayment(paymentMethod) ? 1f : 0f;

                var isFraud =
                    (isCash > 0 && (float)order.FinalSoAmount > FraudThresholds.LargeCashPaymentAmount)
                    || (nameMismatch > 0.5f && isCash > 0);

                records.Add(new PaymentFraudData
                {
                    Amount = (float)order.FinalSoAmount,
                    PaymentMethod = paymentMethod,
                    Hour = order.SoDate.Hour,
                    NameMismatchScore = nameMismatch,
                    IsCash = isCash,
                    IsReturn = 0f,
                    IsFraud = isFraud
                });
            }

            var returns = _posDb.ReturnSalesOrders
                .AsNoTracking()
                .Where(r => !r.Deleted && r.CustomerId != null)
                .Include(r => r.Customer)
                .ToList();

            foreach (var ret in returns)
            {
                var paymentMethod = ret.PaymentType ?? PaymentMethodType.Cash.ToString();
                var customerName = $"{ret.Customer?.FirstName} {ret.Customer?.LastName}".Trim();
                var isCash = EnumExtensions.IsCashPayment(paymentMethod) ? 1f : 0f;

                var isFraud = isCash > 0 && (float)ret.TotalReturnSaleOrderAmount > FraudThresholds.HighReturnValue;

                records.Add(new PaymentFraudData
                {
                    Amount = (float)ret.TotalReturnSaleOrderAmount,
                    PaymentMethod = paymentMethod,
                    Hour = ret.ReturnSaleOrderDate.Hour,
                    NameMismatchScore = 0f,
                    IsCash = isCash,
                    IsReturn = 1f,
                    IsFraud = isFraud
                });
            }

            return records;
        }

        public List<ReturnOfferFraudData> LoadReturnOfferFraudTrainingData()
        {
            var returns = _posDb.ReturnSalesOrders
                .AsNoTracking()
                .Where(r => !r.Deleted && r.CustomerId != null)
                .Include(r => r.SalesOrder)
                .ToList();

            var returnCountByCustomer = returns
                .GroupBy(r => r.CustomerId!.Value)
                .ToDictionary(g => g.Key, g => g.Count());

            return returns.Select(ret =>
            {
                var customerId = ret.CustomerId!.Value;
                var returnCount = returnCountByCustomer[customerId];
                var hadOffer = ret.SalesOrder != null
                               && (ret.SalesOrder.DiscountAmount > 0
                                   || ret.SalesOrder.StoreDiscountOffer > 0
                                   || ret.SalesOrder.CouponCode > 0);

                var daysSinceOffer = hadOffer && ret.SalesOrder != null
                    ? (float)(ret.ReturnSaleOrderDate - ret.SalesOrder.SoDate).TotalDays
                    : 30f;

                var returnAfterOfferRatio = hadOffer ? 1f : 0f;
                var suspiciousScore = Math.Min(1f, returnCount / 10f);

                var isFraud =
                    (hadOffer && daysSinceOffer <= 7)
                    || returnCount > FraudThresholds.HighReturnCount
                    || suspiciousScore > 0.5f;

                return new ReturnOfferFraudData
                {
                    ReturnCount = returnCount,
                    ReturnValue = (float)ret.TotalReturnSaleOrderAmount,
                    DaysSinceOffer = daysSinceOffer,
                    HadOffer = hadOffer ? 1f : 0f,
                    ReturnAfterOfferRatio = returnAfterOfferRatio,
                    SuspiciousPatternScore = suspiciousScore,
                    IsFraud = isFraud
                };
            }).ToList();
        }

        public List<ValidationFraudData> LoadValidationFraudTrainingData()
        {
            var orders = _posDb.SalesOrders
                .AsNoTracking()
                .Where(so => !so.Deleted && so.CustomerId != null)
                .Include(so => so.Customer)
                .ToList();

            var storeTypes = _posDb.Stores
                .AsNoTracking()
                .Where(s => !s.Deleted)
                .ToDictionary(s => s.Id, s => (float)(s.StoreTypeId ?? 0));

            var purchasesByCustomerStore = orders
                .GroupBy(so => so.CustomerId!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(x => x.StoreId).ToDictionary(x => x.Key, x => x.Count()));

            var returns = _posDb.ReturnSalesOrders
                .AsNoTracking()
                .Where(r => !r.Deleted && r.CustomerId != null)
                .GroupBy(r => r.CustomerId!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(x => x.StoreId).ToDictionary(x => x.Key, x => x.Count()));

            return orders.Select(order =>
            {
                var customerId = order.CustomerId!.Value;
                var crossStorePurchases = purchasesByCustomerStore.TryGetValue(customerId, out var storeMap)
                    ? storeMap.Where(kv => kv.Key != order.StoreId).Sum(kv => kv.Value)
                    : 0;

                var crossStoreReturns = returns.TryGetValue(customerId, out var returnMap)
                    ? returnMap.Where(kv => kv.Key != order.StoreId).Sum(kv => kv.Value)
                    : 0;

                var customerIsNew = order.Customer != null
                    && (DateTime.UtcNow - order.Customer.CreatedOn).TotalDays < FraudThresholds.NewCustomerDays;

                var hour = order.SoDate.Hour;
                var isFraud =
                    hour < FraudThresholds.StoreOpenHour
                    || hour > FraudThresholds.StoreCloseHour
                    || (crossStorePurchases > 3 && crossStoreReturns > 2);

                return new ValidationFraudData
                {
                    HourOfTransaction = hour,
                    StoreOpenHour = FraudThresholds.StoreOpenHour,
                    StoreCloseHour = FraudThresholds.StoreCloseHour,
                    StoreType = storeTypes.GetValueOrDefault(order.StoreId, 0),
                    PreviousStoreType = storeTypes.GetValueOrDefault(order.StoreId, 0),
                    CustomerIsNew = customerIsNew ? 1f : 0f,
                    CrossStorePurchaseCount = crossStorePurchases,
                    CrossStoreReturnCount = crossStoreReturns,
                    IsFraud = isFraud
                };
            }).ToList();
        }

        public async Task<float> GetCustomerReturnCountAsync(long customerId) =>
            await _posDb.ReturnSalesOrders
                .AsNoTracking()
                .CountAsync(r => r.CustomerId == customerId && !r.Deleted);

        public async Task<decimal> GetStoreTodaySalesAsync(long storeId) =>
            await _posDb.SalesOrders
                .AsNoTracking()
                .Where(so => so.StoreId == storeId && !so.Deleted && so.SoDate.Date == DateTime.Today)
                .SumAsync(so => so.FinalSoAmount);

        public async Task<int> GetStoreTodayReturnsAsync(long storeId) =>
            await _posDb.ReturnSalesOrders
                .AsNoTracking()
                .CountAsync(r => r.StoreId == storeId && !r.Deleted && r.ReturnSaleOrderDate.Date == DateTime.Today);

        public async Task<int> GetStoreTodayInvoiceCountAsync(long storeId) =>
            await _posDb.SalesOrders
                .AsNoTracking()
                .CountAsync(so => so.StoreId == storeId && !so.Deleted && so.SoDate.Date == DateTime.Today);

        public async Task<bool> IsNewCustomerAsync(long customerId)
        {
            var createdOn = await _posDb.Customers
                .AsNoTracking()
                .Where(c => c.Id == customerId)
                .Select(c => (DateTime?)c.CreatedOn)
                .FirstOrDefaultAsync();

            if (createdOn == null) return false;
            return (DateTime.UtcNow - createdOn.Value).TotalDays < FraudThresholds.NewCustomerDays;
        }

        public async Task<float> GetCrossStorePurchasesAsync(long customerId, long storeId) =>
            await _posDb.SalesOrders
                .AsNoTracking()
                .CountAsync(so => so.CustomerId == customerId && so.StoreId != storeId && !so.Deleted);

        public async Task<float> GetCrossStoreReturnsAsync(long customerId, long storeId) =>
            await _posDb.ReturnSalesOrders
                .AsNoTracking()
                .CountAsync(r => r.CustomerId == customerId && r.StoreId != storeId && !r.Deleted);

        public async Task<string> GetCustomerNameAsync(long customerId)
        {
            var customer = await _posDb.Customers
                .AsNoTracking()
                .Where(c => c.Id == customerId)
                .Select(c => new { c.FirstName, c.LastName })
                .FirstOrDefaultAsync();

            return customer == null
                ? string.Empty
                : $"{customer.FirstName} {customer.LastName}".Trim();
        }

        public async Task<float> GetStoreTypeAsync(long storeId)
        {
            var storeTypeId = await _posDb.Stores
                .AsNoTracking()
                .Where(s => s.Id == storeId && !s.Deleted)
                .Select(s => s.StoreTypeId)
                .FirstOrDefaultAsync();

            return (float)(storeTypeId ?? 0);
        }

        public async Task<bool> CustomerHadRecentOfferAsync(long customerId)
        {
            var cutoff = DateTime.Today.AddDays(-30);
            return await _posDb.SalesOrders
                .AsNoTracking()
                .AnyAsync(so => so.CustomerId == customerId
                                && !so.Deleted
                                && so.SoDate >= cutoff
                                && (so.DiscountAmount > 0 || so.StoreDiscountOffer > 0 || so.CouponCode > 0));
        }

        public async Task<int> GetDaysSinceLastOfferAsync(long customerId)
        {
            var lastOfferDate = await _posDb.SalesOrders
                .AsNoTracking()
                .Where(so => so.CustomerId == customerId
                             && !so.Deleted
                             && (so.DiscountAmount > 0 || so.StoreDiscountOffer > 0 || so.CouponCode > 0))
                .OrderByDescending(so => so.SoDate)
                .Select(so => (DateTime?)so.SoDate)
                .FirstOrDefaultAsync();

            if (lastOfferDate == null) return 30;
            return (int)(DateTime.Today - lastOfferDate.Value.Date).TotalDays;
        }

        private static CustomerBehaviorData BuildCustomerBehaviorData(
            List<Data.Entities.SalesOrder> purchases,
            List<Data.Entities.ReturnSalesOrder> returns,
            List<Data.Entities.CustomerLedger> ledger,
            bool labelForTraining)
        {
            var purchaseCount = purchases.Count;
            var returnCount = returns.Count;
            var totalPurchase = (float)purchases.Sum(p => p.FinalSoAmount);
            var returnAmount = (float)returns.Sum(r => r.TotalReturnSaleOrderAmount);
            var returnPercentage = purchaseCount == 0 ? 0f : (float)returnCount / purchaseCount;

            var cashCount = ledger.Count(l => EnumExtensions.IsLedgerType(l.TransactionType, LedgerTransactionType.Cash));
            var bankCount = ledger.Count(l => EnumExtensions.IsLedgerType(l.TransactionType, LedgerTransactionType.Bank));
            var cardCount = ledger.Count(l => EnumExtensions.IsLedgerType(l.TransactionType, LedgerTransactionType.Card));

            var minDate = purchases.Min(p => p.SoDate);
            var maxDate = purchases.Max(p => p.SoDate);
            var avgDaysBetween = purchaseCount <= 1
                ? 0f
                : (float)(maxDate - minDate).TotalDays / (purchaseCount - 1);

            var lastPurchaseDays = (float)(DateTime.Today - maxDate.Date).TotalDays;
            var ledgerDebit = (float)ledger
                .Where(l => EnumExtensions.IsLedgerType(l.TransactionType, LedgerTransactionType.Debit))
                .Sum(l => l.TransactionAmount);
            var ledgerCredit = (float)ledger
                .Where(l => EnumExtensions.IsLedgerType(l.TransactionType, LedgerTransactionType.Credit))
                .Sum(l => l.TransactionAmount);

            var isFraud = labelForTraining && purchaseCount > 0 && returnPercentage > FraudThresholds.CustomerHighReturnRatio;

            return new CustomerBehaviorData
            {
                PurchaseCount = purchaseCount,
                TotalPurchaseAmount = totalPurchase,
                AveragePurchaseAmount = purchaseCount == 0 ? 0 : totalPurchase / purchaseCount,
                ReturnCount = returnCount,
                ReturnAmount = returnAmount,
                ReturnPercentage = returnPercentage,
                CashPaymentPercentage = purchaseCount == 0 ? 0 : (float)cashCount / purchaseCount,
                BankPaymentPercentage = purchaseCount == 0 ? 0 : (float)bankCount / purchaseCount,
                CardPaymentPercentage = purchaseCount == 0 ? 0 : (float)cardCount / purchaseCount,
                AverageDaysBetweenPurchase = avgDaysBetween,
                LastPurchaseDays = lastPurchaseDays,
                LedgerDebitAmount = ledgerDebit,
                LedgerCreditAmount = ledgerCredit,
                IsFraud = isFraud
            };
        }

        private static EmployeeFraudData BuildEmployeeFraudData(
            Data.Entities.SalesPerson employee,
            List<Data.Entities.SalesOrder> sales,
            List<Data.Entities.ReturnSalesOrder> returns,
            List<long> employeeCustomerIds,
            bool labelForTraining)
        {
            var totalInvoices = sales.Count;
            var returnCount = returns.Count;
            var totalSales = (float)sales.Sum(s => s.FinalSoAmount);
            var returnAmount = (float)returns.Sum(r => r.TotalReturnSaleOrderAmount);
            var returnPercentage = totalInvoices == 0 ? 0f : (float)returnCount / totalInvoices;

            var employeePurchases = sales.Where(s => s.CustomerId != null && employeeCustomerIds.Contains(s.CustomerId.Value)).ToList();
            var employeePurchaseCount = employeePurchases.Count;
            var employeePurchaseAmount = (float)employeePurchases.Sum(s => s.FinalSoAmount);
            var incentiveAmount = totalSales * FraudThresholds.EmployeeIncentiveRatio;

            var isFraud = labelForTraining && (
                returnPercentage > FraudThresholds.EmployeeHighReturnRatio
                || employeePurchaseCount > 2
                || employeePurchaseAmount > FraudThresholds.LargeCashPaymentAmount
                || incentiveAmount > 10_000);

            return new EmployeeFraudData
            {
                EmployeeId = employee.Id,
                TotalSales = totalSales,
                TotalInvoices = totalInvoices,
                AverageInvoiceAmount = totalInvoices == 0 ? 0 : totalSales / totalInvoices,
                ReturnCount = returnCount,
                ReturnAmount = returnAmount,
                ReturnPercentage = returnPercentage,
                EmployeePurchaseCount = employeePurchaseCount,
                EmployeePurchaseAmount = employeePurchaseAmount,
                IncentiveAmount = incentiveAmount,
                IncentiveRatio = FraudThresholds.EmployeeIncentiveRatio,
                IsFraud = isFraud
            };
        }
    }
}
