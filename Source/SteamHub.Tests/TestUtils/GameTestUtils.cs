namespace SteamHub.Tests.TestUtils
{
	using System.Data;
	using SteamHub.ApiContract.Models;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Tag;

    public static class GameTestUtils
    {
        private const string APPROVED_STATUS = "Approved";
        private const int DISCOUNT_DECIMAL_COUNT = 0;
        private const int DESCRIPTION_SIZE = 100;
        private const string PENDING_STATUS = "Pending";
        private const int NAME_SIZE = 50;
        private const int STARTING_PRICE = 0;
        private const int MAX_PRICE = 1000;
        private const int PRICE_DECIMAL_COUNT = 2;
        private const int RATING_DECIMAL_COUNT = 2;
        private const int REQUIREMENTS_SIZE = 100;
        private const int STARTING_RATING = 0;
        private const int MAX_RATING = 10;
        private const int PUBLISHER_IDENTIFIER = 1;
        private const int STARTING_DISCOUNT = 0;
        private const int MAX_DISCOUNT = 100;
        private const int INDEX_FOR_RANDOM_TAGS = 1;

        private static readonly Random Random = new Random();

        public static Game CreateRandomGame()
        {
            return new Game
            {
                GameId = (int)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                GameTitle = CommonTestUtils.RandomName(NAME_SIZE),
                GameDescription = CommonTestUtils.RandomName(DESCRIPTION_SIZE),
                ImagePath = CommonTestUtils.RandomPath(),
                Price = CommonTestUtils.RandomNumber(STARTING_PRICE, MAX_PRICE, PRICE_DECIMAL_COUNT),
                TrailerPath = CommonTestUtils.RandomPath(),
                GameplayPath = CommonTestUtils.RandomPath(),
                MinimumRequirements = CommonTestUtils.RandomName(REQUIREMENTS_SIZE),
                RecommendedRequirements = CommonTestUtils.RandomName(REQUIREMENTS_SIZE),
                Status = CommonTestUtils.RandomElement(new[] { APPROVED_STATUS, PENDING_STATUS }),
                Tags = null,
                Rating = CommonTestUtils.RandomNumber(STARTING_RATING, MAX_RATING, RATING_DECIMAL_COUNT),
                Discount = CommonTestUtils.RandomNumber(STARTING_DISCOUNT, MAX_DISCOUNT, DISCOUNT_DECIMAL_COUNT),
                PublisherIdentifier = PUBLISHER_IDENTIFIER
            };
        }

        public static Tag[] RandomTags()
        {
            var allTags = TagsConstants.ALL_TAGS;

            var shuffled = allTags.OrderBy(tag => Random.Next()).ToList();

            var subsetSize = Random.Next(INDEX_FOR_RANDOM_TAGS, allTags.Length + INDEX_FOR_RANDOM_TAGS);

            return shuffled.Take(subsetSize).OrderBy(tag => tag.Tag_name).ToArray();
        }
    }
}
