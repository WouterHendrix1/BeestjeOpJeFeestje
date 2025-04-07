using BeestjeOpJeFeestje.Data.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BeestjeOpJeFeestje.Tests.ControllerTests;
public class AnimalsControllerTests
{
    private readonly Mock<IAnimalRepository> _mockRepo;
    private readonly AnimalsController _controller;

    public AnimalsControllerTests()
    {
        _mockRepo = new Mock<IAnimalRepository>();
        _controller = new AnimalsController(_mockRepo.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewWithAllAnimals()
    {
        // Arrange
        var animals = new List<Animal> { new Animal { Id = 1, Name = "Beestje" } };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(animals);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(animals, viewResult.Model);
    }

    [Fact]
    public void Create_Get_ReturnsViewWithCategories()
    {
        // Act
        var result = _controller.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult);
        Assert.True(viewResult.ViewData.ContainsKey("Categories"));
    }

    [Fact]
    public async Task Create_Post_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var animal = new Animal { Id = 1, Name = "Beestje" };

        // Act
        var result = await _controller.Create(animal);

        // Assert
        _mockRepo.Verify(r => r.AddAsync(animal), Times.Once);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsView()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _controller.Create(new Animal());

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<Animal>(viewResult.Model);
    }

    [Fact]
    public async Task Edit_Get_ValidId_ReturnsViewWithAnimal()
    {
        // Arrange
        var animal = new Animal { Id = 1, Name = "Beestje" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(animal);

        // Act
        var result = await _controller.Edit(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(animal, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Get_InvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Animal?)null);

        // Act
        var result = await _controller.Edit(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var animal = new Animal { Id = 1, Name = "Beestje" };

        // Act
        var result = await _controller.Edit(animal);

        // Assert
        _mockRepo.Verify(r => r.UpdateAsync(animal), Times.Once);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsView()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _controller.Edit(new Animal());

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<Animal>(viewResult.Model);
    }

    [Fact]
    public async Task Details_ValidId_ReturnsView()
    {
        // Arrange
        var animal = new Animal { Id = 1 };
        _mockRepo.Setup(r => r.GetAnimalCompleteAsync(1)).ReturnsAsync(animal);

        // Act
        var result = await _controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(animal, viewResult.Model);
    }

    [Fact]
    public async Task Details_InvalidId_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetAnimalCompleteAsync(1)).ReturnsAsync((Animal?)null);

        var result = await _controller.Details(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_Get_ValidId_ReturnsView()
    {
        var animal = new Animal { Id = 1 };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(animal);

        var result = await _controller.Delete(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(animal, viewResult.Model);
    }

    [Fact]
    public async Task Delete_Get_InvalidId_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Animal?)null);

        var result = await _controller.Delete(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_CallsDeleteAndRedirects()
    {
        var result = await _controller.DeleteConfirmed(1);

        _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }
}
