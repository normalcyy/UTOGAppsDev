namespace OnlineBookstoreWinForms.Models;

public class WishlistItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public decimal BookPrice { get; set; }
    public DateTime AddedAt { get; set; }
}
