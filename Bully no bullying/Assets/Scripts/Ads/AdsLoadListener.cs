using UnityEngine.Advertisements;
using System;

public class AdsLoadListener : IUnityAdsLoadListener
{
    private readonly Action onLoaded;
    private readonly Action<UnityAdsLoadError, string> onFailed;

    public AdsLoadListener(Action onLoaded, Action<UnityAdsLoadError, string> onFailed)
    {
        this.onLoaded = onLoaded;
        this.onFailed = onFailed;
    }

    public void OnUnityAdsAdLoaded(string placementId) => onLoaded?.Invoke();
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) 
        => onFailed?.Invoke(error, message);
}