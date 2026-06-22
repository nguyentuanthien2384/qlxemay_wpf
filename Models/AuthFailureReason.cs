namespace QLXeMay.Models
{
    internal enum AuthFailureReason
    {
        None = 0,
        ValidationError = 1,
        InvalidCredentials = 2,
        Inactive = 3,
        LockedOut = 4,
        DatabaseError = 5
    }
}
