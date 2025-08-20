using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using Scenes;

    public class InterstitialAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [SerializeField] private string androidAdUnitId = "Interstitial_Android";
        [SerializeField] private string iOSAdUnitId = "Interstitial_iOS";

        private string adUnitId;
        private bool isAdLoaded = false;

        private void Awake()
        {
#if UNITY_IOS
            adUnitId = iOSAdUnitId;
#elif UNITY_ANDROID
            adUnitId = androidAdUnitId;
#else
            adUnitId = null;
#endif
        }

        public void LoadInterstitialAd()
        {
            if (string.IsNullOrEmpty(adUnitId))
            {
                Debug.LogError("Ad Unit ID is null or empty.");
                return;
            }

            Debug.Log("Loading Interstitial Ad...");
            Advertisement.Load(adUnitId, this);
        }

        public void ShowInterstitialAd()
        {
            if (isAdLoaded)
            {
                Debug.Log("Showing Interstitial Ad...");
                Advertisement.Show(adUnitId, this);
                isAdLoaded = false; // Reset until next load
            }
            else
            {
                Debug.Log("Interstitial ad not ready yet.");
            }
        }

        // --- Callbacks ---
        public void OnUnityAdsAdLoaded(string placementId)
        {
            if (placementId.Equals(adUnitId))
            {
                Debug.Log("Interstitial Ad Loaded.");
                isAdLoaded = true;
            }
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.LogWarning($"Failed to load Interstitial Ad: {error} - {message}");
            isAdLoaded = false;
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.LogWarning($"Interstitial Ad Show Failure: {error} - {message}");
        }

        public void OnUnityAdsShowStart(string placementId) { }
        public void OnUnityAdsShowClick(string placementId) { }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            Debug.Log("Interstitial Ad Completed.");
            LoadInterstitialAd(); // Preload next ad
        }

        // --- For Ricimi transition ---
        public IEnumerator ShowAdAndTransition(string sceneName, float duration, Color color)
        {
            ShowInterstitialAd();
            yield return new WaitUntil(() => !Advertisement.isShowing);
            Transition.LoadLevel(sceneName, duration, color);
        }
    }
