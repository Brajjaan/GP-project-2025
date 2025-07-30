using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character/NPC")]
public class Character : ScriptableObject
{
   //Basic character information 
   [SerializeField] private string characterName;
   [SerializeField, TextArea] private string description;
   [SerializeField] private Sprite icon;
   [SerializeField] private int morale;
   [SerializeField] private RelationshipLevel relationshipLevel;
   [SerializeField] private int relationshipPoints;
   [SerializeField] private bool isBully;
   [SerializeField] private List<int> AvailableDialogId = new List<int>();
   [SerializeField] private List<int> UsedDialogId = new List<int>();
   [SerializeField] private int DialogStartId;
   [SerializeField] private int DialogEndId;

   public void initializeDialogue(int startId, int endId)
   {
       DialogStartId = startId;
       DialogEndId = endId;
       AvailableDialogId.Clear();
       UsedDialogId.Clear();
       for (int i = startId; i <= endId; i++)
       {
           AvailableDialogId.Add(i);
       }
   }

   public int GetRandomDialogue()
   {
       if (AvailableDialogId.Count == 0)
       {
           Resetdialogs();
       }

       if (AvailableDialogId.Count == 0)
       {
           return-1;
       }
       
       int Randomindex = Random.Range(0, AvailableDialogId.Count);
       int selectedDialogId = AvailableDialogId[Randomindex];
       AvailableDialogId.RemoveAt(Randomindex);
       UsedDialogId.Add(selectedDialogId);
       
       return selectedDialogId;
   }

   private void Resetdialogs()
   {
       AvailableDialogId.Clear();
       for (int i = DialogStartId; i <= DialogEndId; i++)
       {
           AvailableDialogId.Add(i);
       }
       UsedDialogId.Clear();
   }

   public bool HasAvailableDialog()
   {
       return AvailableDialogId.Count > 0;
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
           // Determine the relationship level based on relationship points
           < 100 => RelationshipLevel.Unfriendly,
           > 100 and < 200 => RelationshipLevel.Classmate,
           >= 200 and < 300 => RelationshipLevel.Friendly,
           >= 300 and < 400 => RelationshipLevel.CloseFriend,
           >= 400 => RelationshipLevel.BestFriend,
           
           _ => RelationshipLevel.Classmate
       };
   }
}

enum RelationshipLevel
{
    Unfriendly,
    Classmate,
    Friendly,
    CloseFriend,
    BestFriend
}