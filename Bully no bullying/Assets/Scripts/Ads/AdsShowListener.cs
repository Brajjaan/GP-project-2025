using UnityEngine.Advertisements;
using System;

public class AdsShowListener : IUnityAdsShowListener
{
    private readonly Action onStart;
    private readonly Action<UnityAdsShowError, string> onError;
    private readonly Action onComplete;

    public AdsShowListener(Action onStart, Action<UnityAdsShowError, string> onError, Action onComplete)
    {
        this.onStart = onStart;
        this.onError = onError;
        this.onComplete = onComplete;
    }

    public void OnUnityAdsShowStart(string placementId) => onStart?.Invoke();
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) 
        => onError?.Invoke(error, message);
    public void OnUnityAdsShowClick(string placementId) { }
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState) 
        => onComplete?.Invoke();
}