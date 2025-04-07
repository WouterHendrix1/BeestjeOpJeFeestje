using BeestjeOpJeFeestje.Data.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BeestjeOpJeFeestje.Tests.ControllerTests;
public class CustomersControllerTests
{
    private readonly Mock<ICustomerRepository> _mockRepo;
    private readonly CustomersController _controller;

    public CustomersControllerTests()
    {
        _mockRepo = new Mock<ICustomerRepository>();
        _controller = new CustomersController(_mockRepo.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewWithAllCustomers()
    {
        var customers = new List<Customer> { new Customer { Id = 1, Name = "John Doe" } };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(customers, viewResult.Model);
    }

    [Fact]
    public void Create_Get_ReturnsViewWithCustomerCards()
    {
        var result = _controller.Create();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.True(viewResult.ViewData.ContainsKey("CustomerCards"));
    }

    [Fact]
    public async Task Create_Post_InvalidModel_ReturnsView()
    {
        _controller.ModelState.AddModelError("Email", "Required");

        var result = await _controller.Create(new Customer());

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<Customer>(viewResult.Model);
    }

    [Fact]
    public async Task Edit_Get_ValidId_ReturnsView()
    {
        var customer = new Customer { Id = 1 };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

        var result = await _controller.Edit(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(customer, viewResult.Model);
        Assert.True(viewResult.ViewData.ContainsKey("CustomerCards"));
    }

    [Fact]
    public async Task Edit_Get_InvalidId_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Customer?)null);

        var result = await _controller.Edit(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ValidModel_CallsUpdateAndRedirects()
    {
        var customer = new Customer { Id = 1 };

        var result = await _controller.Edit(customer);

        _mockRepo.Verify(r => r.UpdateAsync(customer), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsView()
    {
        _controller.ModelState.AddModelError("Name", "Required");

        var result = await _controller.Edit(new Customer());

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<Customer>(viewResult.Model);
    }

    [Fact]
    public async Task Details_ValidId_ReturnsView()
    {
        var customer = new Customer { Id = 1 };
        _mockRepo.Setup(r => r.GetCustomerCompleteAsync(1)).ReturnsAsync(customer);

        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(customer, viewResult.Model);
    }

    [Fact]
    public async Task Details_InvalidId_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetCustomerCompleteAsync(1)).ReturnsAsync((Customer?)null);

        var result = await _controller.Details(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_Get_ValidId_ReturnsView()
    {
        var customer = new Customer { Id = 1 };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

        var result = await _controller.Delete(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(customer, viewResult.Model);
    }

    [Fact]
    public async Task Delete_Get_InvalidId_ReturnsNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Customer?)null);

        var result = await _controller.Delete(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_CallsDeleteAndRedirects()
    {
        var result = await _controller.DeleteConfirmed(1);

        _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
}
