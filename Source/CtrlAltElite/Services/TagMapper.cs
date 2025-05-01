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
