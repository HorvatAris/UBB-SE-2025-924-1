using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Context;
using SteamHub.Api.Context.Repositories;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.User;

using EntityRoleEnum = SteamHub.Api.Entities.RoleEnum;
using ContractRoleEnum = SteamHub.ApiContract.Models.User.RoleEnum;


using Xunit;

namespace SteamHub.Tests.Repositories
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly DataContext _mockContext;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockContext = new DataContext(options, null);
            _repository = new UserRepository(_mockContext);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = 1,
                    UserName = "Alice",
                    Email = "alice@example.com",
                    RoleId = EntityRoleEnum.User,
                    WalletBalance = (float)100.0m,
                    PointsBalance = 500
                },
                new User
                {
                    UserId = 2,
                    UserName = "Bob",
                    Email = "bob@example.com",
                    RoleId = EntityRoleEnum.Developer,
                    WalletBalance = (float)200.0m,
                    PointsBalance = 1000
                }
            };

            _mockContext.Users.AddRange(users);
            _mockContext.SaveChanges();
        }

        public void Dispose()
        {
            _mockContext.Dispose();
        }

        [Fact]
        public async Task GetUsersAsync_WhenNoREstrictions_ReturnsAllUsers()
        {
            var result = await _repository.GetUsersAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Users.Count);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ReturnsUser()
        {
            var result = await _repository.GetUserByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Alice", result.UserName);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithInvalidId_ReturnsNull()
        {
            var result = await _repository.GetUserByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUserAsync_WhenValidInput_AddsNewUserSuccessfully()
        {
            _mockContext.Users.RemoveRange(_mockContext.Users);
            await _mockContext.SaveChangesAsync();

            var request = new CreateUserRequest
            {
                UserName = "Charlie",
                Email = "charlie@example.com",
                Role = ContractRoleEnum.Developer,
                WalletBalance = (float)50.0m,
                PointsBalance = 250
            };

            var response = await _repository.CreateUserAsync(request);

            Assert.NotNull(response);
            var newUser = await _mockContext.Users.FindAsync(response.UserId);
            Assert.Equal("Charlie", newUser.UserName);
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidId_UpdatesSuccessfully()
        {
            var request = new UpdateUserRequest
            {
                UserName = "Alice Updated",
                Email = "alice.updated@example.com",
                Role = ContractRoleEnum.User,
                WalletBalance = (float)150.0m,
                PointsBalance = 750
            };

            await _repository.UpdateUserAsync(1, request);
            var updatedUser = await _mockContext.Users.FindAsync(1);

            Assert.Equal("Alice Updated", updatedUser.UserName);
            Assert.Equal("alice.updated@example.com", updatedUser.Email);
            Assert.Equal((int)ContractRoleEnum.User, (int)updatedUser.RoleId);
        }

        [Fact]
        public async Task UpdateUserAsync_WithInvalidId_ThrowsException()
        {
            var request = new UpdateUserRequest
            {
                UserName = "Ghost",
                Email = "ghost@example.com",
                Role = ContractRoleEnum.User,
                WalletBalance = 0,
                PointsBalance = 0
            };

            await Assert.ThrowsAsync<Exception>(() => _repository.UpdateUserAsync(999, request));
        }
    }
}
