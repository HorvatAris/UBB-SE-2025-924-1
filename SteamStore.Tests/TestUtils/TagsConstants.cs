using SteamStore.Models;

namespace SteamStore.Tests.TestUtils;

public abstract class TagsConstants
{
    private const string TAG_NAME_1 = "Rogue-Like";
    private const string TAG_NAME_2 = "Third-Person Shooter";
    private const string TAG_NAME_3 = "Multiplayer";
    private const string TAG_NAME_4 = "Horror";
    private const string TAG_NAME_5 = "First-Person Shooter";
    private const string TAG_NAME_6 = "Action";
    private const string TAG_NAME_7 = "Platformer";
    private const string TAG_NAME_8 = "Adventure";
    private const string TAG_NAME_9 = "Puzzle";
    private const string TAG_NAME_10 = "Exploration";
    private const string TAG_NAME_11 = "Sandbox";
    private const string TAG_NAME_12 = "Survival";
    private const string TAG_NAME_13 = "Arcade";
    private const string TAG_NAME_14 = "RPG";
    private const string TAG_NAME_15 = "Racing";
    private const int INCREMENT_COUNTER = 1;

    private static readonly string[] TAG_NAMES = new[]
    {
        TAG_NAME_1,
        TAG_NAME_2,
        TAG_NAME_3,
        TAG_NAME_4,
        TAG_NAME_5,
        TAG_NAME_6,
        TAG_NAME_7,
        TAG_NAME_8,
        TAG_NAME_9,
        TAG_NAME_10,
        TAG_NAME_11,
        TAG_NAME_12,
        TAG_NAME_13,
        TAG_NAME_14,
        TAG_NAME_15
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