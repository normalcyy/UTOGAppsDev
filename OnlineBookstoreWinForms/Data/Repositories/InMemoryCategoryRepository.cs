using OnlineBookstoreWinForms.Data.Interfaces;
using OnlineBookstoreWinForms.Models;

namespace OnlineBookstoreWinForms.Data.Repositories;

public class InMemoryCategoryRepository : ICategoryRepository
{
    private readonly List<Category> _categories = new List<Category>();
    private int _nextId = 1;

    public InMemoryCategoryRepository()
    {
        foreach (var name in new[] { "Fiction", "Non-Fiction", "Academic", "Science", "History" })
            _categories.Add(new Category { Id = _nextId++, Name = name });
    }

    public Task<List<Category>> GetAllAsync() =>
        Task.FromResult(_categories.ToList());

    public Task<Category?> GetByIdAsync(int id) =>
        Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));

    public Task<Category> AddAsync(Category category)
    {
        category.Id = _nextId++;
        _categories.Add(category);
        return Task.FromResult(category);
    }

    public Task UpdateAsync(Category category)
    {
        var idx = _categories.FindIndex(c => c.Id == category.Id);
        if (idx >= 0) _categories[idx] = category;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _categories.RemoveAll(c => c.Id == id);
        return Task.CompletedTask;
    }
}
