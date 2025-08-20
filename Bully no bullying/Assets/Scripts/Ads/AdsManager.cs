using UnityEngine;
using UnityEngine.Advertisements;
using System;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener
{
    [Header("Game IDs")]
    [SerializeField] private string androidGameId = "YOUR_ANDROID_GAME_ID";
    [SerializeField] private string iOSGameId = "YOUR_IOS_GAME_ID";
    [SerializeField] private bool testMode = true;

    [Header("Ad Unit IDs")]
    [SerializeField] private string interstitialAndroidId = "Interstitial_Android";
    [SerializeField] private string interstitialIOSId = "Interstitial_iOS";
    [SerializeField] private string rewardedAndroidId = "Rewarded_Android";
    [SerializeField] private string rewardedIOSId = "Rewarded_iOS";
    [SerializeField] private string bannerAndroidId = "Banner_Android";
    [SerializeField] private string bannerIOSId = "Banner_iOS";

    private string gameId;
    private string interstitialId;
    private string rewardedId;
    private string bannerId;

    public static AdsManager Instance { get; private set; }

    private Action onRewardedComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

#if UNITY_IOS
        gameId = iOSGameId;
        interstitialId = interstitialIOSId;
        rewardedId = rewardedIOSId;
        bannerId = bannerIOSId;
#elif UNITY_ANDROID
        gameId = androidGameId;
        interstitialId = interstitialAndroidId;
        rewardedId = rewardedAndroidId;
        bannerId = bannerAndroidId;
#else
        gameId = null;
#endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Debug.Log("Initializing Unity Ads...");
            Advertisement.Initialize(gameId, testMode, this);
        }
    }
    
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        LoadInterstitial();
        LoadRewarded();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.LogError($"Unity Ads Initialization Failed: {error} - {message}");
    }
    
    public void LoadInterstitial()
    {
        Advertisement.Load(interstitialId, new AdsLoadListener(
            () => Debug.Log("Interstitial loaded"),
            (err, msg) => Debug.LogError($"Interstitial failed: {err} - {msg}")
        ));
    }

    public void ShowInterstitial()
    {
        Advertisement.Show(interstitialId, new AdsShowListener(
            () => Debug.Log("Interstitial shown"),
            (err, msg) => Debug.LogError($"Interstitial show failed: {err} - {msg}"),
            () =>
            {
                Debug.Log("Interstitial closed");
                LoadInterstitial();
            }
        ));
    }


    public void LoadRewarded()
    {
        Advertisement.Load(rewardedId, new AdsLoadListener(
            () => Debug.Log("Rewarded loaded"),
            (err, msg) => Debug.LogError($"Rewarded failed: {err} - {msg}")
        ));
    }

    public void ShowRewarded(Action onComplete)
    {
        if (Advertisement.isInitialized)
        {
            onRewardedComplete = onComplete;
            Advertisement.Show(rewardedId, new AdsShowListener(
                () => Debug.Log("Rewarded shown"),
                (err, msg) => Debug.LogError($"Rewarded show failed: {err} - {msg}"),
                () =>
                {
                    Debug.Log("Rewarded closed");
                    onRewardedComplete?.Invoke();
                    LoadRewarded();
                }
            ));
        }
        else
        {
            Debug.LogWarning("Rewarded ad not ready yet.");
        }
    }
    
    public void ShowBanner()
    {
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = () => Debug.Log("Banner loaded"),
            errorCallback = (msg) => Debug.LogError($"Banner load failed: {msg}")
        };

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Load(bannerId, options);

        BannerOptions showOptions = new BannerOptions
        {
            showCallback = () => Debug.Log("Banner shown"),
            hideCallback = () => Debug.Log("Banner hidden"),
            clickCallback = () => Debug.Log("Banner clicked")
        };

        Advertisement.Banner.Show(bannerId, showOptions);
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }
}
