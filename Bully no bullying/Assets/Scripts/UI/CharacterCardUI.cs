using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class CharacterCardUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text pointsText;
        
        [SerializeField] private TMP_FontAsset customFont;

        private SO_Character linkedCharacter;

        public void Initialize(SO_Character baseCharacter)
        {
            // Get the cached runtime instance for the given character
            linkedCharacter = CharacterDataCache.GetOrCreate(baseCharacter);

            if (linkedCharacter == null)
            {
                Debug.LogWarning($"CharacterCardUI: Tried to initialize with null character.");
                return;
            }

            // Subscribe to updates so UI refreshes automatically
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

            if (icon) icon.sprite = linkedCharacter.Icon;
            if (nameText) nameText.text = linkedCharacter.CharacterName;
            if (levelText) levelText.text = $"Level: {linkedCharacter.CurrentRelationshipLevel}";
            if (pointsText) pointsText.text = $"Points: {linkedCharacter.CurrentRelationshipPoints}";
            
            if (customFont != null)
            {
                nameText.font = customFont;
                levelText.font = customFont;
                pointsText.font = customFont;
            }
            
        }

        /// <summary>
        /// Manually triggers an update (useful if forced refresh is called from journal UI).
        /// </summary>
        public void ForceUpdate()
        {
            UpdateUI();
        }
    }
}