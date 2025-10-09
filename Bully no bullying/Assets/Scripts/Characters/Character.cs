using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UI;
public class Character : MonoBehaviour, IInteractable
{
    [SerializeField] GameManager gameManager;
    [SerializeField] SO_Character characterData;
    [SerializeField] Canvas dialogueCanvas;
    [SerializeField] Image characterIcon;
    [SerializeField] TMP_Text characterMainText;
    [SerializeField] TMP_Text choice1Text;
    [SerializeField] TMP_Text choice2Text;
    [SerializeField] TMP_Text choice3Text;
    [SerializeField] Button choice1Button;
    [SerializeField] Button choice2Button;
    [SerializeField] Button choice3Button;
    
    [SerializeField] private CharacterUI characterUI;

    void Start()
    {
        dialogueCanvas.gameObject.SetActive(false);
        if (characterData != null)
        {
            characterData = CharacterDataCache.GetOrCreate(characterData);

            // Load used dialogues
            List<string> loadedNames = SaveData.LoadUsedDialogues(characterData.CharacterName);
            characterData.UsedDialogues.Clear();
            characterData.UsedDialogues.AddRange(
                characterData.DialogueData.Where(d => loadedNames.Contains(d.DialogName))
            );

            // Load relationship points (reputation)
            int savedPoints = SaveData.LoadRelationshipPoints(characterData.CharacterName);
            if (savedPoints > 0)
            {
                typeof(SO_Character)
                    .GetField("relationshipPoints", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                    .SetValue(characterData, savedPoints);

                // Update relationship level after loading
                var updateMethod = typeof(SO_Character).GetMethod("UpdateRelationshipLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                updateMethod.Invoke(characterData, null);
            }

            return;
        }
    }

    public void EnterDialogue()
    {
        gameManager.isInteracting = true;
        dialogueCanvas.gameObject.SetActive(true);
        
        if (characterUI != null)
            characterUI.SetCharacter(characterData);

        DialogueData randomDialog = characterData.GetRandomDialogue();

        while (characterData.UsedDialogues.Contains(randomDialog))
        {
            int i = 0;
            randomDialog = characterData.GetRandomDialogue();
            i++;
            if (i > 100)
            {
                return;
            }
        }
        characterData.UsedDialogues.Add(randomDialog);
        SaveData.SaveUsedDialogues(
            characterData.CharacterName,
            characterData.UsedDialogues.Select(d => d.DialogName).ToList()
        );
        characterMainText.text = characterData.CharacterName + " : " + randomDialog.DialogText;
        choice1Text.text = randomDialog.Choices[0].ChoiceText;
        choice2Text.text = randomDialog.Choices[1].ChoiceText;
        choice3Text.text = randomDialog.Choices[2].ChoiceText;

        choice1Button.onClick.RemoveAllListeners();
        choice2Button.onClick.RemoveAllListeners();
        choice3Button.onClick.RemoveAllListeners();

        choice1Button.onClick.AddListener(() => OnChoiceSelected(randomDialog.Choices[0]));
        choice2Button.onClick.AddListener(() => OnChoiceSelected(randomDialog.Choices[1]));
        choice3Button.onClick.AddListener(() => OnChoiceSelected(randomDialog.Choices[2]));

        characterIcon.sprite = characterData.Icon;
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        Debug.Log($"[DIALOGUE] {characterData.CharacterName} — Player chose: \"{choice.ChoiceText}\" " +
                  $"(Reputation change: {choice.ReputationPoint})");

        // Apply the reputation change
        characterData.ApplyDialogChoice(choice);

        // Now log the updated relationship points and level
        Debug.Log($"[REPUTATION] {characterData.CharacterName} — New Relationship Points: {characterData.CurrentRelationshipPoints}, " +
                  $"Current Level: {characterData.CurrentRelationshipLevel}");

        StartCoroutine(HideCanvasAfterDelay());
    }


    private IEnumerator HideCanvasAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        dialogueCanvas.gameObject.SetActive(false);
        gameManager.isInteracting = false;
    }

    public void Interact()
    {
        if (gameManager.isInteracting)
        {
            return;
        }
        EnterDialogue();
    }
}