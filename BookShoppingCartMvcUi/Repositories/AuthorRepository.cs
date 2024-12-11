using Microsoft.EntityFrameworkCore;

namespace BookShoppingCartMvcUi.Repositories;

public class AuthorRepository: IAuthorRepository
{
    private readonly ApplicationDbContext _context;

    public AuthorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAuthor(Author author)
    {
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAuthor(Author author)
    {
        _context.Authors.Update(author);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAuthor(Author author)
    {
        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
    }

    public async Task<Author?> GetAuthorById(int id)
    {
        return await _context.Authors.FindAsync(id);
    }

    public async Task<IEnumerable<Author>> GetAuthor()
    {
        return await _context.Authors.ToListAsync();
    }
}

public interface IAuthorRepository
{
    Task AddAuthor(Author author);
    Task UpdateAuthor(Author author);
    Task<Author?> GetAuthorById(int id);
    Task DeleteAuthor(Author author);
    Task<IEnumerable<Author>> GetAuthor();
}

