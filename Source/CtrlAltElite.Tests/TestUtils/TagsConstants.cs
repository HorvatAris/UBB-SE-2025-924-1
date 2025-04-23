using SteamStore.Models;

namespace SteamStore.Tests.TestUtils;

public abstract class TagsConstants
{
    private const int INCREMENT_COUNTER = 1;

    private static readonly string[] TAG_NAMES = new[]
    {
        "Rogue-Like",
        "Third-Person Shooter",
        "Multiplayer",
        "Horror",
        "First-Person Shooter",
        "Action",
        "Platformer",
        "Adventure",
        "Puzzle",
        "Exploration",
        "Sandbox",
        "Survival",
        "Arcade",
        "RPG",
        "Racing"
    };

    public static readonly Tag[] ALL_TAGS = TAG_NAMES
        .Select((name, index) => new Tag
        {
            TagId = index + INCREMENT_COUNTER,
            Tag_name = name
        })
        .ToArray();
    public static List<string> GetTagsName => ALL_TAGS.Select(tag => tag.Tag_name).ToList();
}