using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialog", menuName = "Character/Dialog")]
public class DialogueData : ScriptableObject
{
    [SerializeField] private string dialogName;
    [SerializeField] private string dialogText;
    [SerializeField] private DialogueChoice[] choices = new DialogueChoice[3];
    
    public string DialogName => dialogName;
    public string DialogText => dialogText;
    public DialogueChoice[] Choices => choices;
}


[System.Serializable]
public class DialogueChoice
{
    [SerializeField] public string choiceText;
    [SerializeField] public int reputationPoint;
    public string ChoiceText => choiceText;
    public int ReputationPoint => reputationPoint;
}