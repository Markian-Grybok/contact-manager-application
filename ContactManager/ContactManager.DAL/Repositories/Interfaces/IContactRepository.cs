using ContactManager.DAL.Entities;

namespace ContactManager.DAL.Repositories.Interfaces;

public interface IContactRepository
{
    Task<IEnumerable<Contacts>> GetAllAsync();

    Task<Contacts?> GetByIdAsync(int id);

    Task CreateRangeAsync(IEnumerable<Contacts> entities);

    void Update(Contacts entity);

    void UpdateRange(IEnumerable<Contacts> entities);

    void Delete(Contacts entity);
}
