using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Tag;
using SteamStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlAltElite.Services
{
    //public static Game MapToGame(GameDetailedResponse game)
    //{
    //    return new Game
    //    {
    //        GameId = game.Identifier,
    //        Status = game.Status.ToString(),
    //        GameDescription = game.Description,
    //        ImagePath = game.ImagePath,
    //        GameTitle = game.Name,
    //        Price = game.Price,
    //        RecommendedRequirements = game.RecommendedRequirements,
    //        MinimumRequirements = game.MinimumRequirements,
    //        Discount = game.Discount,
    //        NumberOfRecentPurchases = game.NumberOfRecentPurchases,
    //        Rating = game.Rating,
    //        TrailerPath = game.TrailerPath,
    //        GameplayPath = game.GameplayPath,
    //        PublisherIdentifier = game.PublisherUserIdentifier,
    //        Tags = game.Tags.Select(t => t.TagName).ToArray(),
    //        TagScore = Game.NOTCOMPUTED,
    //    };
    //}
    public class TagMapper
    {
        public static Tag MapToTag(TagSummaryResponse tag)
        {
            return new Tag
            {
                TagId = tag.TagId,
                Tag_name = tag.TagName,
            };
        }
    }
}
