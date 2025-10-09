using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private bool isJournalOpen = false;

    // Name of the Journal scene — make sure it matches your scene name exactly
    [SerializeField] private string journalSceneName = "UI_Reputation";

    // 🧭 Called from your UI button to open the Journal
    public void OpenJournal()
    {
        if (isJournalOpen) return;

        SceneManager.LoadSceneAsync(journalSceneName, LoadSceneMode.Additive);
        Time.timeScale = 0f; // Pause the game while journal is open
        isJournalOpen = true;
        Debug.Log($"[SceneLoader] Opened journal scene: {journalSceneName}");
    }

    // 🧭 Called from the Journal UI to close itself
    public void CloseJournal()
    {
        if (!isJournalOpen) return;

        SceneManager.UnloadSceneAsync(journalSceneName);
        Time.timeScale = 1f; // Resume game time
        isJournalOpen = false;
        Debug.Log($"[SceneLoader] Closed journal scene: {journalSceneName}");
    }
}