using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Models.Developer
{
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Tag;
    public class UpdateGameWithTagsRequest
    {
        public Game Game { get; set; }
        public List<Tag> SelectedTags { get; set; }
    }

}
