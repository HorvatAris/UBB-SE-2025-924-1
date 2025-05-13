namespace SteamHub.Tests.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Moq;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Repositories;
    using SteamHub.ApiContract.Services;
    using Xunit;

    public class UserServiceTests
    {
        private readonly UserService userService;
        private readonly Mock<IUserRepository> userRepositoryMock;

        public UserServiceTests()
        {
            this.userRepositoryMock = new Mock<IUserRepository>();
            this.userService = new UserService(this.userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenCalled_ShouldReturnCorrectNumberOfUsers()
        {
            var mockUsers = CreateMockUsers();
            userRepositoryMock.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = mockUsers });

            var result = await userService.GetAllUsersAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenCalled_ShouldMapFirstUserIdCorrectly()
        {
            var mockUsers = CreateMockUsers();
            userRepositoryMock.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = mockUsers });

            var result = await userService.GetAllUsersAsync();

            Assert.Equal(1, result[0].UserId);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenCalled_ShouldMapFirstUserNameCorrectly()
        {
            var mockUsers = CreateMockUsers();
            userRepositoryMock.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = mockUsers });

            var result = await userService.GetAllUsersAsync();

            Assert.Equal("user1", result[0].UserName);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenCalled_ShouldMapFirstUserEmailCorrectly()
        {
            var mockUsers = CreateMockUsers();
            userRepositoryMock.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = mockUsers });

            var result = await userService.GetAllUsersAsync();

            Assert.Equal("user1@example.com", result[0].Email);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenCalled_ShouldMapFirstUserWalletBalanceCorrectly()
        {
            var mockUsers = CreateMockUsers();
            userRepositoryMock.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = mockUsers });

            var result = await userService.GetAllUsersAsync();

            Assert.Equal(100f, result[0].WalletBalance);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenCalled_ShouldMapFirstUserPointsBalanceCorrectly()
        {
            var mockUsers = CreateMockUsers();
            userRepositoryMock.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = mockUsers });

            var result = await userService.GetAllUsersAsync();

            Assert.Equal(50, result[0].PointsBalance);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenCalled_ShouldMapFirstUserRoleToDeveloper()
        {
            var mockUsers = CreateMockUsers();
            userRepositoryMock.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = mockUsers });

            var result = await userService.GetAllUsersAsync();

            Assert.Equal(UserRole.Developer, result[0].UserRole);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenCalled_ShouldMapSecondUserIdCorrectly()
        {
            var mockUsers = CreateMockUsers();
            userRepositoryMock.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = mockUsers });

            var result = await userService.GetAllUsersAsync();

            Assert.Equal(2, result[1].UserId);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenCalled_ShouldMapSecondUserNameCorrectly()
        {
            var mockUsers = CreateMockUsers();
            userRepositoryMock.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = mockUsers });

            var result = await userService.GetAllUsersAsync();

            Assert.Equal("user2", result[1].UserName);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenCalled_ShouldMapSecondUserEmailCorrectly()
        {
            var mockUsers = CreateMockUsers();
            userRepositoryMock.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = mockUsers });

            var result = await userService.GetAllUsersAsync();

            Assert.Equal("user2@example.com", result[1].Email);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenCalled_ShouldMapSecondUserWalletBalanceCorrectly()
        {
            var mockUsers = CreateMockUsers();
            userRepositoryMock.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = mockUsers });

            var result = await userService.GetAllUsersAsync();

            Assert.Equal(200f, result[1].WalletBalance);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenCalled_ShouldMapSecondUserPointsBalanceCorrectly()
        {
            var mockUsers = CreateMockUsers();
            userRepositoryMock.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = mockUsers });

            var result = await userService.GetAllUsersAsync();

            Assert.Equal(80, result[1].PointsBalance);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenCalled_ShouldMapSecondUserRoleToUser()
        {
            var mockUsers = CreateMockUsers();
            userRepositoryMock.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = mockUsers });

            var result = await userService.GetAllUsersAsync();

            Assert.Equal(UserRole.User, result[1].UserRole);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenRepositoryReturnsEmptyList_ShouldReturnEmptyList()
        {
            userRepositoryMock.Setup(repo => repo.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = new List<UserResponse>() });

            var result = await userService.GetAllUsersAsync();

            Assert.Empty(result);
        }

        private static List<UserResponse> CreateMockUsers()
        {
            return new List<UserResponse>
            {
                new UserResponse
                {
                    UserId = 1,
                    UserName = "user1",
                    Email = "user1@example.com",
                    WalletBalance = 100f,
                    PointsBalance = 50,
                    Role = RoleEnum.Developer
                },
                new UserResponse
                {
                    UserId = 2,
                    UserName = "user2",
                    Email = "user2@example.com",
                    WalletBalance = 200f,
                    PointsBalance = 80,
                    Role = RoleEnum.User
                }
            };
        }
    }
}
