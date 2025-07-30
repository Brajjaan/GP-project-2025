using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialog", menuName = "Character/Dialog")]
public class DialogData : ScriptableObject
{
    [SerializeField] private string dialogName;
    [SerializeField] private string dialogText;
    [SerializeField] private DialogChoice[] choices = new DialogChoice[3];
}


[System.Serializable]
public class DialogChoice
{
    [SerializeField] private string choiceText;
    [SerializeField] private int moralChange;
    public string ChoiceText => choiceText;
    public int MoralChange => moralChange;
}