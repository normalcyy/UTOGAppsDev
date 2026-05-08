namespace OnlineBookstoreWinForms.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public int AuthorId { get; set; }
    public int CategoryId { get; set; }

    // Populated by repository joins — not persisted
    public string AuthorName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
}
