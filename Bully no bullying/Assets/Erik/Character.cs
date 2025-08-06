using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character/NPC")]
public class Character : ScriptableObject
{
    // Basic character informations
    [SerializeField] private string characterName;
    [SerializeField, TextArea] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private RelationshipLevel relationshipLevel;
    [SerializeField] private int relationshipPoints;
    [SerializeField] private bool isBully;
    [SerializeField] private List<DialogueData> dialogueData;
    [SerializeField] private List<DialogueData> usedDialogues;

    [Header("Dialog Settings")]
    [SerializeField] private CharacterFolder characterFolder = CharacterFolder.Character1;
    [SerializeField] public bool autoLoadDialogs = true;

    // Public properties
    public string CharacterName => characterName;
    public string Description => description;
    public Sprite Icon => icon;
    public RelationshipLevel CurrentRelationshipLevel => relationshipLevel;
    public int CurrentRelationshipPoints => relationshipPoints;
    public bool IsBully => isBully;
    public List<DialogueData> DialogueData => dialogueData;
    public List<DialogueData> UsedDialogues => usedDialogues;
    public CharacterFolder SelectedCharacterFolder => characterFolder;

    [ContextMenu("Load Dialogs from Folder")]
    public void LoadDialogsFromFolder()
    {
        dialogueData.Clear();
        int folderNumber = (int)characterFolder;

#if UNITY_EDITOR
        string folderPath = $"Assets/Characters/Character {folderNumber} Dialogue";
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:DialogueData", new[] { folderPath });
        
        DialogueData[] loadedDialogs = new DialogueData[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
            loadedDialogs[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<DialogueData>(assetPath);
        }
#else
        DialogueData[] loadedDialogs = new DialogueData[0];
#endif

        if (loadedDialogs.Length > 0)
        {
            dialogueData.AddRange(loadedDialogs);
            //Debug.Log($"Loaded {loadedDialogs.Length} dialogs from Characters/Character {folderNumber} Dialogue");

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
        else
        {
            //Debug.LogWarning($"No dialogs found in folder: Characters/Character {folderNumber} Dialogue");
        }
    }

    private void OnValidate()
    {
        if (autoLoadDialogs)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += LoadDialogsFromFolder;
#endif
        }
    }

    public void ApplyDialogChoice(DialogueChoice choice)
    {
        if (choice.ReputationPoint > 0)
        {
            AddRelationshipPoints(choice.ReputationPoint);
        }
        else if (choice.ReputationPoint < 0)
        {
            RemoveRelationshipPoints(-choice.ReputationPoint);
        }
    }

    public void AddRelationshipPoints(int points)
    {
        relationshipPoints += points;
        UpdateRelationshipLevel();
    }

    public void RemoveRelationshipPoints(int points)
    {
        relationshipPoints -= points;
        if (relationshipPoints < 0) relationshipPoints = 0;
        UpdateRelationshipLevel();
    }

    private void UpdateRelationshipLevel()
    {
        relationshipLevel = relationshipPoints switch
        {
            < 100 => RelationshipLevel.Unfriendly,
            > 100 and < 200 => RelationshipLevel.Classmate,
            >= 200 and < 300 => RelationshipLevel.Friendly,
            >= 300 and < 400 => RelationshipLevel.CloseFriend,
            >= 400 => RelationshipLevel.BestFriend,
            _ => RelationshipLevel.Classmate
        };
    }
    
    public DialogueData GetRandomDialogue()
    {
        if (dialogueData == null || dialogueData.Count == 0)
        {
            Debug.LogWarning($"No dialogues available for character: {characterName}");
            return null;
        }

        int randomIndex = Random.Range(0, dialogueData.Count);
        return dialogueData[randomIndex];
    }
}

public enum CharacterFolder
{
    Character1 = 1,
    Character2 = 2,
    Character3 = 3,
    Character4 = 4,
    Character5 = 5
}

public enum RelationshipLevel
{
    Unfriendly,
    Classmate,
    Friendly,
    CloseFriend,
    BestFriend
}