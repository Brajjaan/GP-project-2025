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