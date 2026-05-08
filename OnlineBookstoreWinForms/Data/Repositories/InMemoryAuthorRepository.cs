using OnlineBookstoreWinForms.Data.Interfaces;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Data.Repositories;

public class InMemoryAuthorRepository : IAuthorRepository
{
    private readonly List<Author> _authors = new List<Author>();
    private int _nextId = 1;

    public InMemoryAuthorRepository()
    {
        var seeds = new[]
        {
            ("J.K. Rowling",      "British author best known for the Harry Potter fantasy series."),
            ("George Orwell",     "English novelist and essayist known for Animal Farm and Nineteen Eighty-Four."),
            ("Robert C. Martin", "Software engineer and author known as 'Uncle Bob', advocate of clean code."),
            ("Stephen Hawking",  "Theoretical physicist, cosmologist, and author of A Brief History of Time."),
            ("Yuval Noah Harari","Israeli historian and professor, author of the Sapiens trilogy."),
        };
        foreach (var (name, bio) in seeds)
            _authors.Add(new Author { Id = _nextId++, Name = name, Bio = bio });
    }

    public Task<List<Author>> GetAllAsync() => Task.FromResult(_authors.ToList());

    public Task<Author?> GetByIdAsync(int id) =>
        Task.FromResult(_authors.FirstOrDefault(a => a.Id == id));

    public Task<Author> AddAsync(Author author)
    {
        author.Id = _nextId++;
        _authors.Add(author);
        return Task.FromResult(author);
    }

    public Task UpdateAsync(Author author)
    {
        var idx = _authors.FindIndex(a => a.Id == author.Id);
        if (idx >= 0) _authors[idx] = author;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _authors.RemoveAll(a => a.Id == id);
        return Task.CompletedTask;
    }
}
