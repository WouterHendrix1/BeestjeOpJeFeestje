using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using BeestjeOpJeFeestje.Controllers;
using BeestjeOpJeFeestje.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using Microsoft.AspNetCore.Http;

namespace BeestjeOpJeFeestje.Tests.Controllers
{
    public class AccountControllerTests
    {
        private AccountController CreateController(Mock<SignInManager<IdentityUser>> mockSignInManager)
        {
            return new AccountController(mockSignInManager.Object);
        }

        private Mock<SignInManager<IdentityUser>> GetMockSignInManager()
        {
            var userManager = new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null
            );

            return new Mock<SignInManager<IdentityUser>>(
                userManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(),
                null, null, null, null
            );
        }

        [Fact]
        public void Login_Get_ReturnsView()
        {
            // Arrange
            var mockSignInManager = GetMockSignInManager();
            var controller = CreateController(mockSignInManager);

            // Act
            var result = controller.Login();

            // Assert

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_Post_ValidCredentials_RedirectsToHome()
        {
            // Arrange
            var mockSignInManager = GetMockSignInManager();
            mockSignInManager.Setup(s => s.PasswordSignInAsync("test@example.com", "Password123", false, false))
                .ReturnsAsync(SignInResult.Success);

            var controller = CreateController(mockSignInManager);
            var model = new LoginViewModel { Email = "test@example.com", Password = "Password123" };

            // Act
            var result = await controller.Login(model);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public async Task Login_Post_InvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            var mockSignInManager = GetMockSignInManager();
            mockSignInManager.Setup(s => s.PasswordSignInAsync("fail@example.com", "WrongPassword", false, false))
                .ReturnsAsync(SignInResult.Failed);

            var controller = CreateController(mockSignInManager);
            var model = new LoginViewModel { Email = "fail@example.com", Password = "WrongPassword" };

            // Act
            var result = await controller.Login(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid); // Because an error is added
            Assert.True(controller.ModelState.ErrorCount > 0);
        }

        [Fact]
        public async Task LogOut_CallsSignOut_AndRedirectsToHome()
        {
            // Arrange
            var mockSignInManager = GetMockSignInManager();
            mockSignInManager.Setup(s => s.SignOutAsync()).Returns(Task.CompletedTask);

            var controller = CreateController(mockSignInManager);

            // Act
            var result = await controller.LogOut();

            // Assert
            mockSignInManager.Verify(s => s.SignOutAsync(), Times.Once);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }
    }
}
