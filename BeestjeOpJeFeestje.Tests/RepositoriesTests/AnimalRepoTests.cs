using BeestjeOpJeFeestje.Data;
using BeestjeOpJeFeestje.Data.DatabaseModels;
using BeestjeOpJeFeestje.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeestjeOpJeFeestje.Tests.RepositoriesTests;
public class AnimalRepositoryTests
{
    private DbContextOptions<DatabaseContext> _options;
    private DatabaseContext _context;
    private AnimalRepository _repository;

    public AnimalRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<DatabaseContext>()
                    .UseInMemoryDatabase(databaseName: "AnimalTestDb")
                    .Options;

        _context = new DatabaseContext(_options);
        _repository = new AnimalRepository(_context);

        // Ensure database is created and animals are seeded
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllAnimals()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        var expectedAnimalsCount = 17;
        Assert.Equal(expectedAnimalsCount, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsAnimalWhenExists()
    {
        // Arrange
        var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Name == "Olifant");

        // Act
        var result = await _repository.GetByIdAsync(animal.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Olifant", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullWhenNotFound()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAnimalsByIdsAsync_ReturnsAnimals()
    {
        // Arrange
        var animalIds = await _context.Animals
                                       .Where(a => a.Type == AnimalType.Jungle)
                                       .Select(a => a.Id)
                                       .ToListAsync();

        // Act
        var result = await _repository.GetAnimalsByIdsAsync(animalIds);
        // Assert
        Assert.Equal(animalIds.Count, result.Count());
    }

    [Fact]
    public async Task AddAsync_AddsAnimalToDatabase()
    {
        // Arrange
        var newAnimal = new Animal { Name = "Test Animal", Type = AnimalType.Farm, Price = 50, ImageUrl = "/images/testanimal.jpg" };

        // Act
        await _repository.AddAsync(newAnimal);

        // Assert
        var addedAnimal = await _context.Animals.FindAsync(newAnimal.Id);
        Assert.NotNull(addedAnimal);
        Assert.Equal("Test Animal", addedAnimal.Name);

    }

    [Fact]
    public async Task UpdateAsync_UpdatesAnimalInDatabase()
    {
        // Arrange
        var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Name == "Hond");

        animal.Name = "Updated Dog";

        // Act
        await _repository.UpdateAsync(animal);

        // Assert
        var updatedAnimal = await _context.Animals.FindAsync(animal.Id);
        Assert.Equal("Updated Dog", updatedAnimal.Name);
    }

    [Fact]
    public async Task DeleteAsync_DeletesAnimalFromDatabase()
    {
        // Arrange
        var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Name == "Ezel");

        // Act
        await _repository.DeleteAsync(animal.Id);

        // Assert
        var deletedAnimal = await _context.Animals.FindAsync(animal.Id);
        Assert.Null(deletedAnimal);

        // Clean up
        _context.Animals.Add(animal); // Re-add the animal for cleanup
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
