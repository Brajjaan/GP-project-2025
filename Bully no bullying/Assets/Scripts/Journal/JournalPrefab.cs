using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JournalPrefab : MonoBehaviour
{
    [Header("Optional: reference to a close button inside the prefab")]
    [SerializeField] private Button closeButton;

    [Header("Optional: prefab animator/audio before close")]
    [SerializeField] private Animator animator;
    [SerializeField] private string closeTrigger = "Close"; // animator trigger name
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float waitForAnimationSeconds = 0.25f;

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseButtonPressed);
    }

    private void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseButtonPressed);
    }

    public void CloseButtonPressed()
    {
        if (animator != null && !string.IsNullOrEmpty(closeTrigger))
        {
            animator.SetTrigger(closeTrigger);
        }
        if (audioSource != null)
        {
            audioSource.Play();
        }

        StartCoroutine(CloseAfterDelay());
    }

    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSecondsRealtime(waitForAnimationSeconds);

        // If parent or spawner needs to be notified, try to find it
        // Prefer the JournalSpawner approach: it will destroy this prefab by calling CloseJournalFromPrefab
        // But if none found, just destroy the prefab and notify bridge
        var spawner = FindObjectOfType<JournalSpawner>();
        if (spawner != null)
        {
            spawner.CloseJournalFromPrefab();
        }
        else
        {
            // fallback: simply destroy self and notify bridge
            Destroy(gameObject);
            JournalBridge.Instance?.OnJournalClosed();
            Time.timeScale = 1f;
        }
    }
}
