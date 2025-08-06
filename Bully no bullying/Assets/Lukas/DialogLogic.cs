using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogLogic : MonoBehaviour
{
    public GameManager gameManager;

    public void EnterDialogue(Character character)
    {
        DialogueData randomDialog = character.GetRandomDialogue();
        while (character.UsedDialogues.Contains(randomDialog))
        {
            randomDialog = character.GetRandomDialogue();
        }
        character.UsedDialogues.Add(randomDialog);
        Debug.Log($"Starting dialogue with {character.CharacterName}: {randomDialog.DialogText}");
        
        foreach (var choice in randomDialog.Choices)
        {
            Debug.Log($"Choice: {choice.ChoiceText} (Reputation: {choice.ReputationPoint})");
            // Here we would normally display the choice in the UI
            Debug.Log($"Response: {choice.responseText}");
        }
    }
}