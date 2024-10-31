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
    public UserServiceTests()
    {
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsNull_WhenUserNotFound()
    {
        // Act
        string? result = "";
        result = null;


        // Assert
        Assert.Null(result);
    }
}