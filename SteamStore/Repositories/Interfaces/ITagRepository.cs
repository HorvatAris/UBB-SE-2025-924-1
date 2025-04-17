// <copyright file="ITagRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamStore.Repositories;
using System.Collections.ObjectModel;
using SteamStore.Models;
public interface ITagRepository
{
     Collection<Tag> GetAllTags();
}