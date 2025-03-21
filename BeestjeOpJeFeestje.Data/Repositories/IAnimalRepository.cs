using BeestjeOpJeFeestje.Data.DatabaseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAnimalRepository
{
    Task<IEnumerable<Animal>> GetAllAsync();
    Task<Animal?> GetByIdAsync(int id);

    Task<List<Animal>> GetAnimalsByIdsAsync (List<int> ids);
    Task<Animal?> GetAnimalCompleteAsync(int id);
    Task AddAsync(Animal animal);
    Task UpdateAsync(Animal animal);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
