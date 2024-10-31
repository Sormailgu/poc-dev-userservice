using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using UserService.Models;
using UserService.Services;
using FinancialService.Client;
using FinancialService.Client.Models;
using ConfigureService.Client;
using ConfigureService.Client.Models;
using Xunit;

public class UserServiceTests
{
    private readonly IUserService _userService;
    private readonly FinancialServiceClient _financialServiceClient;
    private readonly ConfigureServiceClient _configureServiceClient;
    private readonly BackgroundSyncService _backgroundSyncService;

    public UserServiceTests()
    {
        _financialServiceClient = Substitute.For<FinancialServiceClient>();
        _configureServiceClient = Substitute.For<ConfigureServiceClient>();
        _backgroundSyncService = Substitute.For<BackgroundSyncService>();

        _userService = new UserService.Services.UserService(
            _financialServiceClient,
            _configureServiceClient,
            _backgroundSyncService
        );
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsCorrectUser()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, Name = "John Doe", Email = "john.doe@example.com", DateOfBirth = new DateTime(1990, 1, 1), IsActive = true };
        
        var accountBalanceResponse = new AccountBalanceResponseDto
        {
            UserId = userId,
            AccountId = Guid.NewGuid(),
            Balance = 1000,
            LastUpdated = DateTime.UtcNow
        };
        var accountTypeResponse = new List<AccountTypeResponseDto>
        {
            new AccountTypeResponseDto { Id = "1", Name = "Savings", Description = "Savings Account", IsActive = true }
        };

        _financialServiceClient.AccountBalance[userId].GetAsync().Returns(Task.FromResult(accountBalanceResponse));
        _configureServiceClient.Api.AccountType.GetAccountTypeList.GetAsync().Returns(Task.FromResult(accountTypeResponse));

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal(1000, result.UserWithBalance.Balance);
        Assert.Equal("Savings", result.UserAccountType.Name);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsNull_WhenUserNotFound()
    {
        // Arrange
        var userId = 99;

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_ThrowsException_WhenFinancialServiceFails()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, Name = "John Doe", Email = "john.doe@example.com", DateOfBirth = new DateTime(1990, 1, 1), IsActive = true };

        _financialServiceClient.AccountBalance[userId].GetAsync().Returns(Task.FromException<AccountBalanceResponseDto>(new Exception("Financial service error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.GetUserByIdAsync(userId));
    }

    [Fact]
    public async Task GetUserByIdAsync_ThrowsException_WhenConfigureServiceFails()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, Name = "John Doe", Email = "john.doe@example.com", DateOfBirth = new DateTime(1990, 1, 1), IsActive = true };
        var accountBalanceResponse = new AccountBalanceResponseDto
        {
            UserId = userId,
            AccountId = Guid.NewGuid(),
            Balance = 1000,
            LastUpdated = DateTime.UtcNow
        };

        _financialServiceClient.AccountBalance[userId].GetAsync().Returns(Task.FromResult(accountBalanceResponse));
        _configureServiceClient.Api.AccountType.GetAccountTypeList.GetAsync().Returns(Task.FromException<List<AccountTypeResponseDto>>(new Exception("Configure service error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _userService.GetUserByIdAsync(userId));
    }
}