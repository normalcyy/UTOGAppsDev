namespace OnlineBookstoreWinForms;

public static class Constants
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string User  = "User";
    }

    public static class OrderStatus
    {
        public const string Pending    = "Pending";
        public const string Processing = "Processing";
        public const string Shipped    = "Shipped";
        public const string Delivered  = "Delivered";
        public const string Cancelled  = "Cancelled";

        public static readonly string[] All =
            { Pending, Processing, Shipped, Delivered, Cancelled };
    }
}
