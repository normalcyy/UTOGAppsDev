using OnlineBookstoreWinForms.Data.Interfaces;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Data.Repositories;

public class InMemoryBookRepository : IBookRepository
{
    private readonly List<Book> _books = new List<Book>();
    private readonly List<OrderItem> _orderItems = new List<OrderItem>();
    private int _nextId = 1;

    public InMemoryBookRepository()
    {
        var seeds = new[]
        {
            ("Harry Potter and the Sorcerer's Stone", "978-0590353427", 14.99m, 50,
                "The first novel in J.K. Rowling's Harry Potter series.",
                new DateTime(1997, 6, 26), 1, 1),
            ("Animal Farm", "978-0451526342", 8.99m, 40,
                "A satirical allegorical novella by George Orwell.",
                new DateTime(1945, 8, 17), 2, 1),
            ("Nineteen Eighty-Four", "978-0451524935", 9.99m, 35,
                "A dystopian social science fiction novel by George Orwell.",
                new DateTime(1949, 6, 8), 2, 1),
            ("Clean Code", "978-0132350884", 39.99m, 20,
                "A handbook of agile software craftsmanship by Robert C. Martin.",
                new DateTime(2008, 8, 1), 3, 3),
            ("The Clean Coder", "978-0137081073", 34.99m, 15,
                "A code of conduct for professional programmers.",
                new DateTime(2011, 5, 13), 3, 3),
            ("A Brief History of Time", "978-0553380163", 12.99m, 30,
                "A landmark volume in science writing by Stephen Hawking.",
                new DateTime(1988, 4, 1), 4, 4),
            ("The Grand Design", "978-0553805376", 13.99m, 25,
                "Stephen Hawking and Leonard Mlodinow explore the laws of nature.",
                new DateTime(2010, 9, 7), 4, 4),
            ("Sapiens", "978-0062316097", 16.99m, 45,
                "A brief history of humankind by Yuval Noah Harari.",
                new DateTime(2011, 1, 1), 5, 2),
            ("Homo Deus", "978-0062464316", 17.99m, 40,
                "A brief history of tomorrow by Yuval Noah Harari.",
                new DateTime(2015, 9, 4), 5, 2),
            ("21 Lessons for the 21st Century", "978-0525512172", 15.99m, 38,
                "How to survive and thrive in a bewildering world, by Yuval Noah Harari.",
                new DateTime(2018, 9, 4), 5, 2),
        };

        foreach (var (title, isbn, price, stock, desc, published, authorId, categoryId) in seeds)
        {
            _books.Add(new Book
            {
                Id            = _nextId++,
                Title         = title,
                ISBN          = isbn,
                Price         = price,
                Stock         = stock,
                Description   = desc,
                PublishedDate = published,
                AuthorId      = authorId,
                CategoryId    = categoryId,
            });
        }
    }

    private static readonly Dictionary<int, string> AuthorNames = new Dictionary<int, string>
    {
        { 1, "J.K. Rowling" },
        { 2, "George Orwell" },
        { 3, "Robert C. Martin" },
        { 4, "Stephen Hawking" },
        { 5, "Yuval Noah Harari" },
    };

    private static readonly Dictionary<int, string> CategoryNames = new Dictionary<int, string>
    {
        { 1, "Fiction" },
        { 2, "Non-Fiction" },
        { 3, "Academic" },
        { 4, "Science" },
        { 5, "History" },
    };

    private static Book Hydrate(Book b)
    {
        b.AuthorName   = AuthorNames.TryGetValue(b.AuthorId, out var a) ? a : string.Empty;
        b.CategoryName = CategoryNames.TryGetValue(b.CategoryId, out var c) ? c : string.Empty;
        return b;
    }

    public Task<List<Book>> GetAllAsync() =>
        Task.FromResult(_books.Select(Hydrate).ToList());

    public Task<Book?> GetByIdAsync(int id)
    {
        var book = _books.FirstOrDefault(b => b.Id == id);
        return Task.FromResult(book == null ? null : Hydrate(book));
    }

    public Task<List<Book>> SearchAsync(string? title, string? authorName, int? categoryId, decimal? minPrice, decimal? maxPrice)
    {
        var results = _books.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(title))
            results = results.Where(b => b.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0);

        if (!string.IsNullOrWhiteSpace(authorName))
        {
            results = results.Where(b =>
                AuthorNames.TryGetValue(b.AuthorId, out var an) &&
                an.IndexOf(authorName, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        if (categoryId.HasValue)
            results = results.Where(b => b.CategoryId == categoryId.Value);

        if (minPrice.HasValue)
            results = results.Where(b => b.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            results = results.Where(b => b.Price <= maxPrice.Value);

        return Task.FromResult(results.Select(Hydrate).ToList());
    }

    public Task<Book> AddAsync(Book book)
    {
        book.Id = _nextId++;
        _books.Add(book);
        return Task.FromResult(Hydrate(book));
    }

    public Task UpdateAsync(Book book)
    {
        var idx = _books.FindIndex(b => b.Id == book.Id);
        if (idx >= 0) _books[idx] = book;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _books.RemoveAll(b => b.Id == id);
        return Task.CompletedTask;
    }

    public void RegisterOrderItems(IEnumerable<OrderItem> items)
    {
        _orderItems.AddRange(items);
    }

    public Task<bool> HasOrderItemsAsync(int bookId) =>
        Task.FromResult(_orderItems.Any(oi => oi.BookId == bookId));
}
