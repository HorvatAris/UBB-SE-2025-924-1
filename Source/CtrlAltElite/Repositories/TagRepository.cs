// <copyright file="TagRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Repositories;
using System.Collections.ObjectModel;
using System.Data;
using SteamStore.Constants;
using SteamStore.Data;
using SteamStore.Models;


public class TagRepository : ITagRepository
{
    private readonly IDataLink dataLink;

    public TagRepository(IDataLink dataLink)
    {
        this.dataLink = dataLink;
    }

    public Collection<Tag> GetAllTags()
    {
        var tags = new Collection<Tag>();
        var allTags = this.dataLink.ExecuteReader(SqlConstants.GetAllTagsProcedure);
        foreach (DataRow row in allTags.Rows)
        {
            var tag = new Tag
            {
                TagId = (int)row[SqlConstants.TagIdColumn],
                Tag_name = (string)row[SqlConstants.TagNameColumn],
                NumberOfUserGamesWithTag = Tag.NOTCOMPUTED,
            };
            tags.Add(tag);
        }

        return tags;
    }
}