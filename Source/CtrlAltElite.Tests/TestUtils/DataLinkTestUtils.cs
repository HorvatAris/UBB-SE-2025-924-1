using Microsoft.Extensions.Configuration;
using SteamStore.Data;

namespace SteamStore.Tests.TestUtils;

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