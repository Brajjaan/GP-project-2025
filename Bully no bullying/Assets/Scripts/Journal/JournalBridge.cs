using System.Collections;
using UnityEngine;

public class JournalBridge : MonoBehaviour
{
    public static JournalBridge Instance { get; private set; }

    [Header("Main UI objects to disable while Journal is open")]
    [Tooltip("Drag GameObjects (e.g. the parent that contains your journal button and other related buttons)")]
    [SerializeField] private GameObject[] objectsToDisable;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnJournalOpened()
    {
        foreach (var obj in objectsToDisable)
            if (obj != null)
                obj.SetActive(false);
    }

    public void OnJournalClosed()
    {
        StartCoroutine(ReenableAfterFrame());
    }

    private IEnumerator ReenableAfterFrame()
    {
        // Wait one frame so any teardown completes first
        yield return null;
        foreach (var obj in objectsToDisable)
            if (obj != null)
                obj.SetActive(true);
    }
}