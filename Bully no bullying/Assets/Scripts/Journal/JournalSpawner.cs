using UnityEngine;

public class JournalSpawner : MonoBehaviour
{
    [Header("Assign the Journal Prefab (Canvas root)")]
    [SerializeField] private GameObject journalPrefab;

    [Header("Parent canvas (optional). If empty, prefab will be instantiated at root)")]
    [SerializeField] private Transform uiParent;

    [Header("Pause game while journal open?")]
    [SerializeField] private bool pauseGame = true;

    private GameObject currentInstance;
    private bool isOpen = false;

    // Call from your main button OnClick
    public void ToggleJournal()
    {
        if (isOpen) CloseJournal();
        else OpenJournal();
    }

    public void OpenJournal()
    {
        if (isOpen || journalPrefab == null) return;

        // Instantiate under given UI parent (so it uses the same canvas if desired)
        if (uiParent != null)
            currentInstance = Instantiate(journalPrefab, uiParent, false);
        else
            currentInstance = Instantiate(journalPrefab);

        // Notify bridge to disable main UI
        JournalBridge.Instance?.OnJournalOpened();

        // Pause game optionally
        if (pauseGame) Time.timeScale = 0f;

        isOpen = true;
    }

    public void CloseJournal()
    {
        if (!isOpen) return;

        if (currentInstance != null)
            Destroy(currentInstance);

        // Re-enable main UI
        JournalBridge.Instance?.OnJournalClosed();

        if (pauseGame) Time.timeScale = 1f;

        isOpen = false;
    }

    // Optional: allow JournalPrefab to call this when it wants to close itself
    public void CloseJournalFromPrefab()
    {
        CloseJournal();
    }
}