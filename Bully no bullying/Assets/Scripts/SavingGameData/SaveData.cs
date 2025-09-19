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
}