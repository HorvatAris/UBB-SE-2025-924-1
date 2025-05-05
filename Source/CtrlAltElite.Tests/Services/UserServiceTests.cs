namespace CtrlAltElite.Tests.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CtrlAltElite.Models;
    using CtrlAltElite.ServiceProxies;
    using CtrlAltElite.Services;
    using Moq;
    using SteamHub.ApiContract.Models.User;
    using Xunit;

    public class UserServiceTests
    {
        private readonly UserService userService;
        private readonly Mock<IUserServiceProxy> userServiceProxyMock;

        public UserServiceTests()
        {
            this.userServiceProxyMock = new Mock<IUserServiceProxy>();
            this.userService = new UserService(this.userServiceProxyMock.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenCalled_ShouldReturnMappedUsers()
        {
            // Arrange
            var userResponseList = new List<UserResponse>
            {
                new UserResponse
                {
                    UserId = 1,
                    UserName = "user1",
                    Email = "user1@example.com",
                    WalletBalance = 100f,
                    PointsBalance = 50,
                    Role = (RoleEnum)1 // Developer
                },
                new UserResponse
                {
                    UserId = 2,
                    UserName = "user2",
                    Email = "user2@example.com",
                    WalletBalance = 200f,
                    PointsBalance = 80,
                    Role = (RoleEnum)0 // User
                }
            };

            this.userServiceProxyMock
                .Setup(proxy => proxy.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = userResponseList });

            var result = await this.userService.GetAllUsersAsync();

            Assert.Equal(2, result.Count);

            Assert.Equal(1, result[0].UserId);
            Assert.Equal("user1", result[0].UserName);
            Assert.Equal("user1@example.com", result[0].Email);
            Assert.Equal(100f, result[0].WalletBalance);
            Assert.Equal(50, result[0].PointsBalance);
            Assert.Equal(User.Role.Developer, result[0].UserRole);

            Assert.Equal(2, result[1].UserId);
            Assert.Equal("user2", result[1].UserName);
            Assert.Equal("user2@example.com", result[1].Email);
            Assert.Equal(200f, result[1].WalletBalance);
            Assert.Equal(80, result[1].PointsBalance);
            Assert.Equal(User.Role.User, result[1].UserRole);
        }
    }
}
