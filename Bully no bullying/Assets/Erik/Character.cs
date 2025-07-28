using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;

[CreateAssetMenu(fileName = "New Character", menuName = "Character/NPC")]
public class Character : ScriptableObject, IInteractable
{
   //Basic character information 
   [SerializeField] private string CharacterName;
   [SerializeField, TextArea] private string description;
   [SerializeField] private Sprite icon;
   [SerializeField] private int morale;
   [SerializeField] private RelationshipLevel relationshipLevel;
   [SerializeField] private int relationshipPoints;
   [SerializeField] private bool isBully;
   
   public void Interact()
   {
       // Implement interaction logic here
       Debug.Log($"Interacting with {CharacterName}. Relationship Level: {relationshipLevel}");
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
       // Determine the relationship level based on relationship points
       if (relationshipPoints < 100)
       {
           relationshipLevel = RelationshipLevel.Unfriendly;
       }
       else if (relationshipPoints is > 100 and < 200)
       {
           relationshipLevel = RelationshipLevel.Classmate;
       }
       else if (relationshipPoints is >= 200 and < 300)
       {
           relationshipLevel = RelationshipLevel.Friendly;
       }
       else if (relationshipPoints is >= 300 and < 400)
       {
           relationshipLevel = RelationshipLevel.CloseFriend;
       }
       else if (relationshipPoints >= 400)
       {
           relationshipLevel = RelationshipLevel.BestFriend;
       }
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