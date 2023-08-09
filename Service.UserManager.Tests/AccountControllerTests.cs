using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Service.UserManager.Controllers;
using Service.UserManager.Models;
using Service.UserManager.Repositories;
using Service.UserManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.UserManager.Tests;

public class AccountControllerTests
{
    private Mock<ILogger<AccountController>> _logger;
    private Mock<IUserAccountRepository> _userAccountRepo;

    public AccountControllerTests()
    {
        _logger = new Mock<ILogger<AccountController>>();
        _userAccountRepo = new Mock<IUserAccountRepository>();
    }

    [Fact]
    public async void GetUser_WithCorrectId_ReturnsOKResult()
    {
        // Arrange
        var returnedUserAccount = new UserAccount
        {
            Email = "test@user.com",
            Password = null,
            Enabled = true,
            Confirmed = false
        };
        _userAccountRepo.Setup(repo => repo.CreateUser(It.IsAny<CreateUserViewModel>())).ReturnsAsync(returnedUserAccount);

        // Act
        var controller = new AccountController(_logger.Object, _userAccountRepo.Object);
        var response = await controller.CreateUser(new CreateUserViewModel { Email = "test@user.com", Password = "abc123" });

        // Assert
        var userAccount = response.Value;

        userAccount.Should().NotBeNull().And.BeOfType<UserAccount>();

        userAccount.Confirmed.Should().BeFalse();
        userAccount.Enabled.Should().BeTrue();
        userAccount.Email.Should().BeEquivalentTo("test@user.com");
        userAccount.Password.Should().BeNull();
    }

    [Fact]
    public async void GetUser_WithExceptionWhileCreatingUser_ReturnsErrorMessage()
    {
        // Arrange
        _userAccountRepo.Setup(repo => repo.CreateUser(It.IsAny<CreateUserViewModel>())).ThrowsAsync(new Exception("Test Exception"));

        // Act
        var controller = new AccountController(_logger.Object, _userAccountRepo.Object);
        var response = await controller.CreateUser(new CreateUserViewModel { Email = "test@user.com", Password = "abc123" });

        // Assert
        response.Result.Should().NotBeNull();
        response.Result.Should().BeOfType<ObjectResult>();
        var responseObject = response.Result as ObjectResult;
        responseObject.Value.Should().Be("Error occurred while creating user. Our staff has been notified.");
        responseObject.StatusCode.Should().Be(400);
    }
}
