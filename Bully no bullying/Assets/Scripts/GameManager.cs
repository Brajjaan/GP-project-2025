using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DialogLogic dialogLogic;
    [SerializeField] private Character[] characterPrefabs;
    public List<Character> characters;

    private void Start()
    {
        characters = new List<Character>();
        foreach (var character in characterPrefabs)
        {
            Character newCharacter = Instantiate(character);
            if (newCharacter.autoLoadDialogs)
            {
                newCharacter.LoadDialogsFromFolder();
            }
            characters.Add(newCharacter);
        }
        
        dialogLogic.EnterDialogue(characters[0]); // Start dialogue with the first character for demonstration
    }
}