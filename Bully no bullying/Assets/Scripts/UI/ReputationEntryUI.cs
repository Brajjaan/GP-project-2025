using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class ReputationEntryUI : MonoBehaviour
    {
        [SerializeField] private Image characterIcon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text pointsText;

        private SO_Character linkedCharacter;

        public void Initialize(SO_Character character)
        {
            linkedCharacter = character;
            linkedCharacter.OnRelationshipChanged += UpdateUI;
            UpdateUI();
        }

        private void OnDestroy()
        {
            if (linkedCharacter != null)
                linkedCharacter.OnRelationshipChanged -= UpdateUI;
        }

        private void UpdateUI()
        {
            if (linkedCharacter == null) return;

            if (characterIcon) characterIcon.sprite = linkedCharacter.Icon;
            if (nameText) nameText.text = linkedCharacter.CharacterName;
            if (levelText) levelText.text = linkedCharacter.CurrentRelationshipLevel.ToString();
            if (pointsText) pointsText.text = linkedCharacter.CurrentRelationshipPoints.ToString();
        }
    }
}