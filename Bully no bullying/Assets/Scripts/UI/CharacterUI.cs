using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class CharacterUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text relationshipPointsText;
        [SerializeField] private TMP_Text relationshipLevelText;
        [SerializeField] private Image characterIcon;

        private SO_Character currentCharacter;

        public void SetCharacter(SO_Character character)
        {
            if (currentCharacter != null)
                currentCharacter.OnRelationshipChanged -= UpdateUI;
            
            currentCharacter = character;
            if (currentCharacter != null)
                currentCharacter.OnRelationshipChanged += UpdateUI;

            UpdateUI(); 
        }

        public void UpdateUI()
        {
            if (currentCharacter == null) return;

            if (relationshipPointsText != null)
                relationshipPointsText.text = $"Points: {currentCharacter.CurrentRelationshipPoints}";

            if (relationshipLevelText != null)
                relationshipLevelText.text = $"Level: {currentCharacter.CurrentRelationshipLevel}";

            if (characterIcon != null)
                characterIcon.sprite = currentCharacter.Icon;
        }

        private void OnDestroy()
        {
            if (currentCharacter != null)
                currentCharacter.OnRelationshipChanged -= UpdateUI;
        }
    }
}