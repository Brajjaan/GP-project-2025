using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UI
{
    public class ReputationJournalUI : MonoBehaviour
    {
        [SerializeField] private Transform characterListRoot;
        [SerializeField] private CharacterCardUI characterCardPrefab;

        // Optional: assign characters manually in the inspector
        [SerializeField] private List<SO_Character> characters;

        private readonly List<CharacterCardUI> spawnedCards = new();

        private void Start()
        {
            Populate();
        }

        public void Populate()
        {
            // Clear existing entries
            foreach (Transform t in characterListRoot)
                Destroy(t.gameObject);
            spawnedCards.Clear();

            // Load all SO_Characters (from inspector or Resources)
            var listToUse = (characters != null && characters.Count > 0)
                ? characters
                : Resources.LoadAll<SO_Character>("").ToList();

            foreach (var so in listToUse)
            {
                // Get or create runtime data instance from the cache
                var runtimeCharacter = CharacterDataCache.GetOrCreate(so);

                // Create card and initialize it with cached instance
                var card = Instantiate(characterCardPrefab, characterListRoot);
                card.Initialize(runtimeCharacter);
                spawnedCards.Add(card);
            }
        }

        /// <summary>
        /// Forces all cards to refresh their display.
        /// Call this if you suspect data changed without event triggers.
        /// </summary>
        public void RefreshAll()
        {
            foreach (var card in spawnedCards)
                card.ForceUpdate();
        }
    }
}