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
        public async Task GetAllUsersAsync_WhenCalled_ReturnsTwoUsers()
        {
            var userResponseList = CreateMockUsers();

            this.userServiceProxyMock
                .Setup(proxy => proxy.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = userResponseList });

            var result = await this.userService.GetAllUsersAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllUsersAsync_FirstUser_HasCorrectId()
        {
            var userResponseList = CreateMockUsers();

            this.userServiceProxyMock
                .Setup(proxy => proxy.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = userResponseList });

            var result = await this.userService.GetAllUsersAsync();

            Assert.Equal(1, result[0].UserId);
        }

        [Fact]
        public async Task GetAllUsersAsync_FirstUser_HasCorrectName()
        {
            var userResponseList = CreateMockUsers();

            this.userServiceProxyMock
                .Setup(proxy => proxy.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = userResponseList });

            var result = await this.userService.GetAllUsersAsync();

            Assert.Equal("user1", result[0].UserName);
        }

        [Fact]
        public async Task GetAllUsersAsync_FirstUser_HasCorrectEmail()
        {
            var userResponseList = CreateMockUsers();

            this.userServiceProxyMock
                .Setup(proxy => proxy.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = userResponseList });

            var result = await this.userService.GetAllUsersAsync();

            Assert.Equal("user1@example.com", result[0].Email);
        }

        [Fact]
        public async Task GetAllUsersAsync_FirstUser_HasCorrectWalletBalance()
        {
            var userResponseList = CreateMockUsers();

            this.userServiceProxyMock
                .Setup(proxy => proxy.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = userResponseList });

            var result = await this.userService.GetAllUsersAsync();

            Assert.Equal(100f, result[0].WalletBalance);
        }

        [Fact]
        public async Task GetAllUsersAsync_FirstUser_HasCorrectPointsBalance()
        {
            var userResponseList = CreateMockUsers();

            this.userServiceProxyMock
                .Setup(proxy => proxy.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = userResponseList });

            var result = await this.userService.GetAllUsersAsync();

            Assert.Equal(50, result[0].PointsBalance);
        }

        [Fact]
        public async Task GetAllUsersAsync_FirstUser_HasCorrectRole()
        {
            var userResponseList = CreateMockUsers();

            this.userServiceProxyMock
                .Setup(proxy => proxy.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = userResponseList });

            var result = await this.userService.GetAllUsersAsync();

            Assert.Equal(User.Role.Developer, result[0].UserRole);
        }

        [Fact]
        public async Task GetAllUsersAsync_SecondUser_HasCorrectId()
        {
            var userResponseList = CreateMockUsers();

            this.userServiceProxyMock
                .Setup(proxy => proxy.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = userResponseList });

            var result = await this.userService.GetAllUsersAsync();

            Assert.Equal(2, result[1].UserId);
        }

        [Fact]
        public async Task GetAllUsersAsync_SecondUser_HasCorrectName()
        {
            var userResponseList = CreateMockUsers();

            this.userServiceProxyMock
                .Setup(proxy => proxy.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = userResponseList });

            var result = await this.userService.GetAllUsersAsync();

            Assert.Equal("user2", result[1].UserName);
        }

        [Fact]
        public async Task GetAllUsersAsync_SecondUser_HasCorrectEmail()
        {
            var userResponseList = CreateMockUsers();

            this.userServiceProxyMock
                .Setup(proxy => proxy.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = userResponseList });

            var result = await this.userService.GetAllUsersAsync();

            Assert.Equal("user2@example.com", result[1].Email);
        }

        [Fact]
        public async Task GetAllUsersAsync_SecondUser_HasCorrectWalletBalance()
        {
            var userResponseList = CreateMockUsers();

            this.userServiceProxyMock
                .Setup(proxy => proxy.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = userResponseList });

            var result = await this.userService.GetAllUsersAsync();

            Assert.Equal(200f, result[1].WalletBalance);
        }

        [Fact]
        public async Task GetAllUsersAsync_SecondUser_HasCorrectPointsBalance()
        {
            var userResponseList = CreateMockUsers();

            this.userServiceProxyMock
                .Setup(proxy => proxy.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = userResponseList });

            var result = await this.userService.GetAllUsersAsync();

            Assert.Equal(80, result[1].PointsBalance);
        }

        [Fact]
        public async Task GetAllUsersAsync_SecondUser_HasCorrectRole()
        {
            var userResponseList = CreateMockUsers();

            this.userServiceProxyMock
                .Setup(proxy => proxy.GetUsersAsync())
                .ReturnsAsync(new GetUsersResponse { Users = userResponseList });

            var result = await this.userService.GetAllUsersAsync();

            Assert.Equal(User.Role.User, result[1].UserRole);
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
