using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SteamHub.Api.Context;
using SteamHub.Api.Context.Repositories;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.Tag;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SteamHub.Tests.RepositoriesTests
{
    public class TagRepositoryTests
    {
        private readonly DataContext _context;
        private readonly TagRepository _repository;

        public TagRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var inMemorySettings = new Dictionary<string, string>
            {
                { "SomeSetting", "SomeValue" }
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _context = new DataContext(options, configuration);
            _repository = new TagRepository(_context);

            SeedData();
        }

        private void SeedData()
        {
            _context.Tags.AddRange(
                new Tag { TagId = 1, TagName = "Action" },
                new Tag { TagId = 2, TagName = "Adventure" }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllTagsAsync_WhenCalled_ReturnsAllTags()
        {
            var result = await _repository.GetAllTagsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Tags.Count);
        }

        [Fact]
        public async Task GetTagByIdAsync_WhenCalledWithExistingId_ReturnsTag()
        {
            var result = await _repository.GetTagByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Action", result.TagName);
        }

        [Fact]
        public async Task GetTagByIdAsync_WhenCalledWithInvalidId_ReturnsNull()
        {
            var result = await _repository.GetTagByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateTagAsync_WhenCalledWithNewName_CreatesTag()
        {
            var request = new CreateTagRequest
            {
                TagName = "RPG"
            };

            var response = await _repository.CreateTagAsync(request);

            var created = await _context.Tags.FindAsync(response.TagId);
            Assert.NotNull(created);
            Assert.Equal("RPG", created.TagName);
        }

        [Fact]
        public async Task CreateTagAsync_WhenCalledWithExistingName_ThrowsException()
        {
            var request = new CreateTagRequest
            {
                TagName = "Action"
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _repository.CreateTagAsync(request));
        }

        [Fact]
        public async Task UpdateTagAsync_WhenCalledWithExistingId_UpdatesTag()
        {
            var request = new UpdateTagRequest
            {
                TagName = "Action-Updated"
            };

            await _repository.UpdateTagAsync(1, request);

            var updated = await _context.Tags.FindAsync(1);
            Assert.Equal("Action-Updated", updated.TagName);
        }

        [Fact]
        public async Task UpdateTagAsync_WhenCalledWithInvalidId_ThrowsException()
        {
            var request = new UpdateTagRequest
            {
                TagName = "DoesNotExist"
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _repository.UpdateTagAsync(999, request));
        }

        [Fact]
        public async Task DeleteTagAsync_henCalledWithValidId_DeletesTag()
        {
            await _repository.DeleteTagAsync(2);

            var deleted = await _context.Tags.FindAsync(2);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteTagAsync_WhenCalledWithInvalidId_ThrowsException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.DeleteTagAsync(999));
        }
    }
}
