using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public InitializeAds InitializeAds;
    public InterstitialAds InterstitialAds;
    public RewardedAds RewardedAds;
    
    public static  AdsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null  && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
            
        InterstitialAds.LoadInterstitialAd();
        RewardedAds.LoadRewardedAd();
    }
    
}
