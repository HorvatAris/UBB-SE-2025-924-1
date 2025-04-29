using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public static class GameFolderResolver
{
    private static readonly Dictionary<string, string> titleToFolder;

    static GameFolderResolver()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "gamefolders.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            titleToFolder = JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;
        }
        else
        {
            titleToFolder = new Dictionary<string, string>();
        }
    }

    public static string GetFolderName(string gameTitle)
    {
        if (titleToFolder.TryGetValue(gameTitle.ToLower(), out string folderName))
        {
            return folderName;
        }

        // Fallback to normalized folder name
        return gameTitle.ToLower().Replace(" ", "").Replace(":", "");
    }
}
