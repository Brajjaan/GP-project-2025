using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class SaveData
{
    // Saves used dialogs
    public static void SaveUsedDialogues(string characterName, List<string> usedDialogueNames)
    {
        string saveKey = characterName + "_usedDialogues";
        string saveValue = string.Join(",", usedDialogueNames);
        Debug.Log($"Saving for {characterName}: {saveValue}");
        PlayerPrefs.SetString(saveKey, saveValue);
        PlayerPrefs.Save();
    }

    // Load used dialogs
    public static List<string> LoadUsedDialogues(string characterName)
    {
        string saveKey = characterName + "_usedDialogues";
        if (PlayerPrefs.HasKey(saveKey))
        {
            string saveValue = PlayerPrefs.GetString(saveKey);
            return saveValue.Split(',').Where(n => !string.IsNullOrEmpty(n)).ToList();
        }
        return new List<string>();
    }

    // clears saved data
    public static void ClearUsedDialogues(string characterName)
    {
        string saveKey = characterName + "_usedDialogues";
        PlayerPrefs.DeleteKey(saveKey);
    }
    public static void SaveRelationshipPoints(string characterName, int points)
    {
        string saveKey = characterName + "_relationshipPoints";
        PlayerPrefs.SetInt(saveKey, points);
        PlayerPrefs.Save();
    }

    public static int LoadRelationshipPoints(string characterName)
    {
        string saveKey = characterName + "_relationshipPoints";
        return PlayerPrefs.HasKey(saveKey) ? PlayerPrefs.GetInt(saveKey) : 0;
    }

    public static void ClearRelationshipPoints(string characterName)
    {
        string saveKey = characterName + "_relationshipPoints";
        PlayerPrefs.DeleteKey(saveKey);
    }

    
}