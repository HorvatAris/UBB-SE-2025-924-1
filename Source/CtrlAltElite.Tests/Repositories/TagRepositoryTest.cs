namespace SteamStore.Tests.Repositories
{
    using SteamStore.Repositories;
    using SteamStore.Tests.TestUtils;
    public class TagRepositoryTest
    {
        private const int EXPECTED_TAG_COUNT = 15;
        private readonly TagRepository subject = new TagRepository(DataLinkTestUtils.GetDataLink());

        [Fact]
        public void GetAllTags()
        {
            var tags = subject.GetAllTags();
            Assert.Equal(EXPECTED_TAG_COUNT, tags.Count);
            var tagNames = tags.Select(tag => tag.Tag_name).ToList();
            Assert.Equal(TagsConstants.GetTagsName, tagNames);
        }
    }
}
