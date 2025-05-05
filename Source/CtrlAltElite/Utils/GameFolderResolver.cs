// <copyright file="GameFolderResolver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public static class GameFolderResolver
{
    private static readonly Dictionary<string, string> TitleToFolder;

    static GameFolderResolver()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "gamefolders.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            TitleToFolder = JsonSerializer.Deserialize<Dictionary<string, string>>(json) !;
        }
        else
        {
            TitleToFolder = new Dictionary<string, string>();
        }
    }

    public static string GetFolderName(string gameTitle)
    {
        if (TitleToFolder.TryGetValue(gameTitle.ToLower(), out string folderName))
        {
            return folderName;
        }

        // Fallback to normalized folder name
        return gameTitle.ToLower().Replace(" ", string.Empty).Replace(":", string.Empty);
    }
}
