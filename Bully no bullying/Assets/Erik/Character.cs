using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using TMPro;
using UnityEngine.UI;
using System.Linq;

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

    void Start()
    {
        dialogueCanvas.gameObject.SetActive(false);
        if (characterData != null)
        {
            characterData = Instantiate(characterData);
            
            
            // Loads used dialogs from save
            List<string> loadedNames = SaveData.LoadUsedDialogues(characterData.CharacterName);
            characterData.UsedDialogues.Clear();
            characterData.UsedDialogues.AddRange(
                characterData.DialogueData.Where(d => loadedNames.Contains(d.DialogName))
            ); 

            return;
        }
    }

    public void EnterDialogue()
    {
        gameManager.isInteracting = true;
        dialogueCanvas.gameObject.SetActive(true);
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
        characterData.ApplyDialogChoice(choice);
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