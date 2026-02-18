using ContactManager.DAL.Entities;
using ContactManager.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.DAL.Repositories.Realizations;

public class ContactRepository : IContactRepository
{
    private readonly ContactManagerDbContext _context;

    public ContactRepository(ContactManagerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Contacts>> GetAllAsync()
    {
        return await _context.Contacts.ToListAsync();
    }

    public async Task<Contacts?> GetByIdAsync(int id)
    {
        return await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task CreateRangeAsync(IEnumerable<Contacts> entities)
    {
        await _context.Contacts.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public void Update(Contacts entity)
    {
        _context.Contacts.Update(entity);
        _context.SaveChanges();
    }

    public void UpdateRange(IEnumerable<Contacts> entities)
    {
        _context.Contacts.UpdateRange(entities);
        _context.SaveChanges();
    }

    public void Delete(Contacts entity)
    {
        _context.Contacts.Remove(entity);
        _context.SaveChanges();
    }
}
