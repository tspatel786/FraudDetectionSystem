namespace FraudDetectionSystem.Models.Enums
{
    public enum TransactionType
    {
        Purchase = 1,
        Return = 2
    }

    public enum ItemCategory
    {
        Gold = 1,
        Diamond = 2,
        Coin = 3,
        Jewellery = 4
    }

    public enum StoreTypeEnum
    {
        FOFO = 0,
        COCO = 1
    }

    public enum DayType
    {
        Regular = 0,
        Weekend = 1,
        Festival = 2
    }

    public enum FraudCategory
    {
        StoreLevel,
        CustomerBehavior,
        Payment,
        Employee,
        ReturnOffer,
        StoreCustomerValidation
    }
}
