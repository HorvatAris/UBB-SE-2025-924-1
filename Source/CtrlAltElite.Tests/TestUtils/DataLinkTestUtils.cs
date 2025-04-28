namespace SteamStore.Tests.TestUtils
{
    using Microsoft.Extensions.Configuration;
    using SteamStore.Data;

    public static class DataLinkTestUtils
    {
        private const string TESTSETTINGS_JSON = "testsettings.json";

        public static IDataLink GetDataLink()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(TESTSETTINGS_JSON)
                .Build();

            return new DataLink(configuration);
        }
    }
}
