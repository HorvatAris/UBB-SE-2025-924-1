namespace SteamStore.Tests.Services
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Threading.Tasks;
	using CtrlAltElite.Models;
	using CtrlAltElite.ServiceProxies;
	using Moq;
	using SteamHub.ApiContract.Models.PointShopItem;
	using SteamHub.ApiContract.Models.User;
	using SteamHub.ApiContract.Models.UserPointShopItemInventory;
	using SteamStore.Models;
	using SteamStore.Services;
	using SteamStore.Tests.TestUtils;
	using Xunit;

	public class PointShopServiceTest
	{
		private readonly User testUser;
		private readonly Mock<IPointShopItemServiceProxy> itemProxyMock;
		private readonly Mock<IUserPointShopItemInventoryServiceProxy> inventoryProxyMock;
		private readonly Mock<IUserServiceProxy> userServiceProxyMock;
		private readonly PointShopService service;

		public PointShopServiceTest()
		{
			testUser = new User
			{
				UserId = 1,
				UserName = "John",
				PointsBalance = 1000,
				Email = "test@example.com",
				WalletBalance = 50,
				UserRole = User.Role.User
			};

			itemProxyMock = new Mock<IPointShopItemServiceProxy>();
			inventoryProxyMock = new Mock<IUserPointShopItemInventoryServiceProxy>();
			userServiceProxyMock = new Mock<IUserServiceProxy>();

			service = new PointShopService(
				itemProxyMock.Object,
				inventoryProxyMock.Object,
				userServiceProxyMock.Object,
				testUser);
		}

		[Fact]
		public void GetCurrentUser_WhenCalled_ShouldReturnInjectedUser()
		{
			var user = service.GetCurrentUser();
			Assert.Equal(testUser.UserId, user.UserId);
		}

		[Fact]
		public async Task GetAllItems_WhenItemsExist_ShouldReturnMappedItems()
		{
			var expectedItems = new PointShopItem[]
			{
				new PointShopItem() { ItemIdentifier = 1, Name = "Item1", PointPrice = 100 },
				new PointShopItem() { ItemIdentifier = 2, Name = "Item2", PointPrice = 200 }
			};
			itemProxyMock.Setup(proxy => proxy.GetPointShopItemsAsync())
				.ReturnsAsync(new GetPointShopItemsResponse
				{
					PointShopItems = new List<PointShopItemResponse>
					{
						new PointShopItemResponse() { PointShopItemId = 1, Name = "Item1", PointPrice = 100 },
						new PointShopItemResponse() { PointShopItemId = 2, Name = "Item2", PointPrice = 200 }
					}
				});

			var foundItems = await service.GetAllItems();

			AssertUtils.AssertContainsEquivalent(foundItems, expectedItems);
		}

		[Fact]
		public async Task GetUserItems_WhenInventoryExists_ShouldReturnUserOwnedItems()
		{
			var inventory = new GetUserPointShopItemInventoryResponse
			{
				UserPointShopItemsInventory = new List<UserPointShopItemInventoryResponse>
				{
					new UserPointShopItemInventoryResponse() { PointShopItemId = 1, IsActive = true }
				}
			};
			var allItems = new GetPointShopItemsResponse
			{
				PointShopItems = new List<PointShopItemResponse>
				{
					new PointShopItemResponse() { PointShopItemId = 1, Name = "Item1", PointPrice = 100 }
				}
			};
			var expectedItems = new PointShopItem[]
			{
				new PointShopItem() { ItemIdentifier = 1, Name = "Item1", PointPrice = 100, IsActive = true }
			};

			inventoryProxyMock.Setup(proxy => proxy.GetUserInventoryAsync(testUser.UserId))
				.ReturnsAsync(inventory);
			itemProxyMock.Setup(proxy => proxy.GetPointShopItemsAsync())
				.ReturnsAsync(allItems);

			var foundItems = await service.GetUserItems();

			AssertUtils.AssertContainsEquivalent(foundItems, expectedItems);
		}

		[Fact]
		public async Task PurchaseItem_WhenUserHasEnoughPoints_ShouldDeductPointsAndUpdateUser()
		{
			var item = new PointShopItem { ItemIdentifier = 1, PointPrice = 100 };

			inventoryProxyMock.Setup(proxy => proxy.PurchaseItemAsync(It.IsAny<PurchasePointShopItemRequest>()))
				.Returns(Task.CompletedTask);

			userServiceProxyMock.Setup(proxy => proxy.UpdateUserAsync(testUser.UserId, It.IsAny<UpdateUserRequest>()))
				.Returns(Task.CompletedTask);

			await service.PurchaseItem(item);

			Assert.Equal(900, testUser.PointsBalance);
		}

		[Fact]
		public void CanUserPurchaseItem_WhenUserAlreadyOwnsItem_ShouldReturnFalse()
		{
			var selectedItem = new PointShopItem { ItemIdentifier = 1, PointPrice = 100 };
			var userItems = new List<PointShopItem> { selectedItem };

			var canPurchaseItem = service.CanUserPurchaseItem(testUser, selectedItem, userItems);

			Assert.False(canPurchaseItem);
		}

		[Fact]
		public async Task ToggleActivationForItem_WhenItemIsActive_ShouldDeactivateIt()
		{
			var activeItem = new PointShopItem { ItemIdentifier = 1, IsActive = true };
			var userItems = new ObservableCollection<PointShopItem> { activeItem };

			inventoryProxyMock.Setup(proxy => proxy.UpdateItemStatusAsync(It.IsAny<UpdateUserPointShopItemInventoryRequest>()))
				.Returns(Task.CompletedTask);

			var toggledItem = await service.ToggleActivationForItem(1, userItems);

			Assert.Equal(1, toggledItem.ItemIdentifier);
		}
	}
}
