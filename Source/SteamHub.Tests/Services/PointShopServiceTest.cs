namespace SteamHub.Tests.Services
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Threading.Tasks;
	using SteamHub.ApiContract.Models;
	using Moq;
	using SteamHub.ApiContract.Models.PointShopItem;
	using SteamHub.ApiContract.Models.User;
	using SteamHub.ApiContract.Models.UserPointShopItemInventory;
	using SteamHub.ApiContract.Services;
	using SteamHub.Tests.TestUtils;
	using Xunit;
	using SteamHub.ApiContract.Repositories;
    using SteamHub.ApiContract.Context.Repositories;

    public class PointShopServiceTest
	{
		private readonly User testUser;
		private readonly Mock<IPointShopItemRepository> itemRepositoryMock;
        private readonly Mock<IUserPointShopItemInventoryRepository> inventoryRepositoryMock;
        private readonly Mock<IUserRepository> userRepositoryMock;
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
				UserRole = UserRole.User
			};
			itemRepositoryMock = new Mock<IPointShopItemRepository>();
			inventoryRepositoryMock = new Mock<IUserPointShopItemInventoryRepository>();
			userRepositoryMock = new Mock<IUserRepository>();

			service = new PointShopService(
				itemRepositoryMock.Object,
				inventoryRepositoryMock.Object,
				userRepositoryMock.Object,
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

			itemRepositoryMock.Setup(proxy => proxy.GetPointShopItemsAsync())
				.ReturnsAsync(new GetPointShopItemsResponse
				{
					PointShopItems = new List<PointShopItemResponse>
					{
						new PointShopItemResponse() { PointShopItemId = 1, Name = "Item1", PointPrice = 100 },
						new PointShopItemResponse() { PointShopItemId = 2, Name = "Item2", PointPrice = 200 }
					}
				});

			var foundItems = await service.GetAllItemsAsync();

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

			inventoryRepositoryMock.Setup(proxy => proxy.GetUserInventoryAsync(testUser.UserId))
				.ReturnsAsync(inventory);
			itemRepositoryMock.Setup(proxy => proxy.GetPointShopItemsAsync())
				.ReturnsAsync(allItems);

			var foundItems = await service.GetUserItemsAsync();

			AssertUtils.AssertContainsEquivalent(foundItems, expectedItems);
		}

		[Fact]
		public async Task PurchaseItem_WhenUserHasEnoughPoints_ShouldDeductPointsAndUpdateUser()
		{
			var item = new PointShopItem { ItemIdentifier = 1, PointPrice = 100 };

			inventoryRepositoryMock.Setup(proxy => proxy.PurchaseItemAsync(It.IsAny<PurchasePointShopItemRequest>()))
				.Returns(Task.CompletedTask);
			userRepositoryMock.Setup(proxy => proxy.UpdateUserAsync(testUser.UserId, It.IsAny<UpdateUserRequest>()))
				.Returns(Task.CompletedTask);

			await service.PurchaseItemAsync(item);

			Assert.Equal(900, testUser.PointsBalance);
		}

		[Fact]
		public async Task ActivateItem_WhenItemValid_ShouldCallUpdateStatus()
		{
			var item = new PointShopItem { ItemIdentifier = 1 };

			inventoryRepositoryMock.Setup(proxy => proxy.UpdateItemStatusAsync(It.IsAny<UpdateUserPointShopItemInventoryRequest>()))
				.Returns(Task.CompletedTask);

			await service.ActivateItemAsync(item);

			inventoryRepositoryMock.Verify(proxy => proxy.UpdateItemStatusAsync(It.Is<UpdateUserPointShopItemInventoryRequest>(request => request.IsActive)), Times.Once);
		}

		[Fact]
		public async Task DeactivateItem_WhenItemValid_ShouldCallUpdateStatus()
		{
			var item = new PointShopItem { ItemIdentifier = 1 };

			inventoryRepositoryMock.Setup(proxy => proxy.UpdateItemStatusAsync(It.IsAny<UpdateUserPointShopItemInventoryRequest>()))
				.Returns(Task.CompletedTask);

			await service.DeactivateItemAsync(item);

			inventoryRepositoryMock.Verify(proxy => proxy.UpdateItemStatusAsync(It.Is<UpdateUserPointShopItemInventoryRequest>(request => !request.IsActive)), Times.Once);
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
			int itemId = 1;
            var activeItem = new PointShopItem { ItemIdentifier = itemId, IsActive = true };
			var userItems = new ObservableCollection<PointShopItem> { activeItem };

			inventoryRepositoryMock.Setup(proxy => proxy.UpdateItemStatusAsync(It.IsAny<UpdateUserPointShopItemInventoryRequest>())).Returns(Task.CompletedTask);

			var toggledItem = await service.ToggleActivationForItemAsync(itemId, userItems);

			Assert.Equal(itemId, toggledItem.ItemIdentifier);
		}

		[Fact]
		public void TryPurchaseItem_WhenValidNewTransaction_ShouldCreateTransaction()
		{
			var item = new PointShopItem { Name = "Item1", PointPrice = 100, ItemType = "Type1" };
			var transactions = new ObservableCollection<PointShopTransaction>();

			var result = service.TryPurchaseItem(item, transactions, testUser, out var transaction);

			Assert.True(result);
		}
	}
}