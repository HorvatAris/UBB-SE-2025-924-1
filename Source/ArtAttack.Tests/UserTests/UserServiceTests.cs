using Moq;
using NUnit.Framework;
using Steampunks.Services;
using Steampunks.Repository.UserRepository;
using Steampunks.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[TestFixture]
public class UserServiceTests
{
    private UserService _userService;
    private Mock<IUserRepository> _mockUserRepository;

    [SetUp]
    public void SetUp()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockUserRepository.Object);
    }

    [Test]
    public async Task GetAllUsersAsync_ReturnsUsersFromRepository()
    {
        // Arrange
        var user1 = new User("User1");
        user1.SetUserId(1);
        user1.SetWalletBalance(100);
        user1.SetPointBalance(50);
        user1.SetIsDeveloper(false);

        var user2 = new User("User2");
        user2.SetUserId(2);
        user2.SetWalletBalance(200);
        user2.SetPointBalance(75);
        user2.SetIsDeveloper(true);

        var expectedUsers = new List<User> { user1, user2 };

        _mockUserRepository.Setup(x => x.GetAllUsersAsync())
                          .ReturnsAsync(expectedUsers);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        _mockUserRepository.Verify(x => x.GetAllUsersAsync(), Times.Once);
    }

    [Test]
    public async Task GetUserByIdAsync_UserExists_ReturnsUser()
    {
        // Arrange
        var expectedUser = new User("TestUser");
        expectedUser.SetUserId(1);
        expectedUser.SetWalletBalance(150.5f);
        expectedUser.SetPointBalance(75.25f);
        expectedUser.SetIsDeveloper(true);

        _mockUserRepository.Setup(x => x.GetUserByIdAsync(1))
                          .ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("TestUser", result?.Username);
        Assert.AreEqual(1, result?.UserId);
        Assert.AreEqual(150.5f, result?.WalletBalance);
        Assert.AreEqual(75.25f, result?.PointBalance);
        Assert.IsTrue(result?.IsDeveloper);
    }

    [Test]
    public async Task GetUserByIdAsync_UserDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetUserByIdAsync(It.IsAny<int>()))
                          .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(999);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task UpdateUserAsync_ValidUser_ExecutesUpdate()
    {
        // Arrange
        var userToUpdate = new User("UpdatedUser");
        userToUpdate.SetUserId(42);
        userToUpdate.SetWalletBalance(100);
        userToUpdate.SetPointBalance(50);
        userToUpdate.SetIsDeveloper(false);

        _mockUserRepository.Setup(x => x.UpdateUserAsync(userToUpdate))
                          .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateUserAsync(userToUpdate);

        // Assert
        Assert.IsTrue(result);
        _mockUserRepository.Verify(x => x.UpdateUserAsync(userToUpdate), Times.Once);
    }

    [Test]
    public async Task UpdateUserAsync_FailedUpdate_ReturnsFalse()
    {
        // Arrange
        var userToUpdate = new User("UpdatedUser");
        userToUpdate.SetUserId(42);
        userToUpdate.SetWalletBalance(100);
        userToUpdate.SetPointBalance(50);
        userToUpdate.SetIsDeveloper(false);

        _mockUserRepository.Setup(x => x.UpdateUserAsync(userToUpdate))
                          .ReturnsAsync(false);

        // Act
        var result = await _userService.UpdateUserAsync(userToUpdate);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public async Task GetAllUsersAsync_EmptyRepository_ReturnsEmptyList()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetAllUsersAsync())
                          .ReturnsAsync(new List<User>());

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    [Test]
    public void UpdateUserAsync_NullUser_ThrowsException()
    {
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
        await _userService.UpdateUserAsync(null));

        Assert.AreEqual("user", ex.ParamName);
    }
}