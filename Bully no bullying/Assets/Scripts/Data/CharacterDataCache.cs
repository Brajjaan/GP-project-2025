using System.Collections.Generic;
using UnityEngine;

public static class CharacterDataCache
{
    private static readonly Dictionary<string, SO_Character> cache = new();

    /// <summary>
    /// Get or create a cached character instance.
    /// </summary>
    public static SO_Character GetOrCreate(SO_Character baseData)
    {
        if (baseData == null) return null;

        string key = baseData.CharacterName;

        // If already cached, return the existing instance
        if (cache.TryGetValue(key, out var existing))
            return existing;

        // Otherwise, create a runtime copy and load saved data
        var newInstance = Object.Instantiate(baseData);

        LoadCharacterData(newInstance);
        cache[key] = newInstance;
        return newInstance;
    }

    /// <summary>
    /// Save the character's current state into PlayerPrefs.
    /// </summary>
    public static void SaveCharacterData(SO_Character character)
    {
        if (character == null) return;

        SaveData.SaveRelationshipPoints(character.CharacterName, character.CurrentRelationshipPoints);
        SaveData.SaveUsedDialogues(character.CharacterName, character.UsedDialogues.ConvertAll(d => d.DialogName));
    }

    /// <summary>
    /// Load saved data for a given character (relationship points + used dialogues).
    /// </summary>
    public static void LoadCharacterData(SO_Character character)
    {
        if (character == null) return;

        int savedPoints = SaveData.LoadRelationshipPoints(character.CharacterName);
        if (savedPoints > 0)
        {
            var field = typeof(SO_Character).GetField("relationshipPoints",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(character, savedPoints);

            var updateMethod = typeof(SO_Character).GetMethod("UpdateRelationshipLevel",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            updateMethod?.Invoke(character, null);
        }

        var used = SaveData.LoadUsedDialogues(character.CharacterName);
        character.UsedDialogues.Clear();
        character.UsedDialogues.AddRange(character.DialogueData.FindAll(d => used.Contains(d.DialogName)));
    }

    /// <summary>
    /// Clear all cached character data (e.g., on new game).
    /// </summary>
    public static void ClearCache()
    {
        cache.Clear();
    }
}
