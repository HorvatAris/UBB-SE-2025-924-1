// <copyright file="TagMapper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CtrlAltElite.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Tag;
    using SteamStore.Models;

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
