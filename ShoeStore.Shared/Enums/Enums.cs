namespace ShoeStore.Shared.Enums
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Processing,
        Shipped,
        Delivered,
        Cancelled,
        Refunded
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        UPI,
        NetBanking,
        COD
    }

    public enum AddressType
    {
        Home,
        Work,
        Other
    }
    public enum UserRole
    {
        ADMIN,
        CLIENT
    }
}
