﻿// <copyright file="Tag.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Models.Tag
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Tag
    {
        public const int NOTCOMPUTED = -11111;

        public Tag()
        {
        }

        public int TagId { get; set; }

        public string Tag_name { get; set; }

        public int NumberOfUserGamesWithTag { get; set; }
    }
}
