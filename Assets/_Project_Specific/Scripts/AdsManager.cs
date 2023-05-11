using UnityEngine;
using System;
using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GoogleMobileAds.Common;

public class AdsManager : MonoBehaviour
{
    public static AdsManager inst;
    [Header("Android")] private string AndroidAppID = "ca-app-pub-1107552888714987~6336148998";
    private string AndroidBannerID,
        AndroidRewardVideoID,
        AndroidIntertitialID;

    private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;
    //private AppOpenAd appOpenAd;
    public bool isTest;


    internal string rewardType, interstitalType;
    public Root myDeserializedClass;
    private string AdURL = "https://ashafashion.store/vivek/omsaiinfo/godaddy/data.php";
    //private string AdURL = "http://android-admin-api.maheshpatel.me/v1/adSettingId?applicationMasterId=63f46a23f8445dcd0ed54e6c";

    public Button m_HideDaddy;
    public Button m_Simulator;
    private int AdsShowCount = 0;

    public GameObject m_NoInternet;

    public void SetRefs()
    {
        Debug.Log("Reset");
    }
    private void Start()
    {
        // m_HideDaddy = GameObject.Find("Hide_Daddy").GetComponent<Button>();
        // m_Simulator = GameObject.Find("Simulator").GetComponent<Button>();
        m_HideDaddy.onClick.AddListener(HIdeGame);
        m_Simulator.onClick.AddListener(Simulatorgame);
        CheckInternet();
    }

    public void CheckInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            m_NoInternet.SetActive(true);
            Debug.Log("Error. Check internet connection!");
        }
        else
        {
            StartCoroutine(getdata());
            m_NoInternet.SetActive(false);
        }
    }
    public void HomeScreen()
    {

        Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.LoadScene(0);
        DOVirtual.DelayedCall(.1f, () =>
        {
            m_HideDaddy = GameObject.Find("Hide_Daddy").GetComponent<Button>();
            m_Simulator = GameObject.Find("Simulator").GetComponent<Button>();

            m_HideDaddy.onClick.AddListener(HIdeGame);
            m_Simulator.onClick.AddListener(Simulatorgame);
        });
        Screen.orientation = ScreenOrientation.Portrait;
    }
    public void HIdeGame()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.LoadScene(1);
    }
    public void Simulatorgame()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        SceneManager.LoadScene(2);
    }
    [Obsolete]
    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (inst != this)
            Destroy(gameObject);
        CheckInternet();
        IsStart = true;
        StartCoroutine(getdata());
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;

    }
    #region APPOPENADS
    private AppOpenAd appOpenAd;
    private string appOpenId = "ca-app-pub-3940256099942544/3419835294";
    private bool IsStart = false;
    public void LoadAppOpenAd()
    {
        if (appOpenId == "")
        {
            return;
        }
        else
        {
            Debug.Log("appOpenId showing ads");
        }

        AdRequest request = new AdRequest.Builder().Build();

        // Load an app open ad for portrait orientation
        AppOpenAd.LoadAd(appOpenId, ScreenOrientation.Portrait, request, ((appOpenAd2, error) =>
        {
            if (error != null)
            {
                // Handle the error.
                //LoadAppOpenAd();
                Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
                return;
            }

            // App open ad is loaded.
            appOpenAd = appOpenAd2;
            if (IsStart)
            {
                IsStart = false;
                ShowAdIfAvailable();
            }
        }));
    }
    private bool IsAppOpenAdAvailable
    {
        get
        {
            return appOpenAd != null;
        }
    }
    bool isFirstTime = true;
    private void OnAppStateChanged(AppState state)
    {
        Debug.Log("App State changed to : " + state);

        // if the app is Foregrounded and the ad is available, show it.
        if (state == AppState.Foreground)
        {
            ShowAdIfAvailable();
        }
    }

    public bool isShowingAd = false;

    public void ShowAdIfAvailable()
    {
        if (!IsAppOpenAdAvailable || isShowingAd)
        {
            return;
        }
        appOpenAd.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
        appOpenAd.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
        appOpenAd.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
        appOpenAd.OnAdDidRecordImpression += HandleAdDidRecordImpression;
        appOpenAd.OnPaidEvent += HandlePaidEvent;
        appOpenAd.Show();

    }

    private void OpenAdsFirstTime()
    {
        Debug.Log("App Open AAds SHOW----------------");
        appOpenAd.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
        appOpenAd.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
        appOpenAd.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
        appOpenAd.OnAdDidRecordImpression += HandleAdDidRecordImpression;
        appOpenAd.OnPaidEvent += HandlePaidEvent;
        appOpenAd.Show();
    }
    private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args)
    {
        Debug.Log("Closed app open ad");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        appOpenAd = null;
        isShowingAd = false;
        Invoke(nameof(LoadAppOpenAd), 1);
    }

    private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args)
    {
        Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        appOpenAd = null;
        Invoke(nameof(LoadAppOpenAd), 1);
    }

    private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args)
    {
        Debug.Log("Displayed app open ad");
        isShowingAd = true;
    }

    private void HandleAdDidRecordImpression(object sender, EventArgs args)
    {
        Debug.Log("Recorded ad impression");
    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args)
    {
        Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
                args.AdValue.CurrencyCode, args.AdValue.Value);
    }
    #endregion
    [Obsolete]
    public IEnumerator getdata()
    {
        WWW _www = new WWW(AdURL);
        yield return _www;
        if (_www.error == null)
        {
            Debug.Log(_www.text);
            // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
            /// myDeserializedClass = JsonConvert.DeserializeObject<Root>(_www.text);
            myDeserializedClass = JsonUtility.FromJson<Root>(_www.text);
            if (myDeserializedClass.getProfile[0].googleAdmob.banner != "")
            {
                AndroidBannerID = myDeserializedClass.getProfile[0].googleAdmob.banner;
            }
            if (myDeserializedClass.getProfile[0].googleAdmob.interstitial != "")
            {
                AndroidIntertitialID = myDeserializedClass.getProfile[0].googleAdmob.interstitial;
            }
            if (myDeserializedClass.getProfile[0].googleAdmob.rewarded != "")
            {
                AndroidRewardVideoID = myDeserializedClass.getProfile[0].googleAdmob.rewarded;
            }
            appOpenId = myDeserializedClass.getProfile[0].googleAdmob.appOpen;
            if (AndroidBannerID == "")
            {
                AndroidBannerID = "ca-app-pub-3940256099942544/6300978111";
            }
            if (AndroidIntertitialID == "")
            {
                AndroidIntertitialID = "ca-app-pub-3940256099942544/1033173712";
            }
            if (AndroidRewardVideoID == "")
            {
                AndroidRewardVideoID = "ca-app-pub-3940256099942544/5224354917";
            }

            if (myDeserializedClass.getProfile[0].adsCount.interstitial != "")
            {
                AdsShowCount = int.Parse(myDeserializedClass.getProfile[0].adsCount.interstitial);
            }
            else
            {
                AdsShowCount = 4;
            }
            IsStart = true;
            MobileAds.Initialize(initStatus => { });
            LoadAppOpenAd();
            RequestBanner();
            RequestInterstitial();
            LoadRewardVideo();
        }
        else
        {
            Debug.Log("Wrong Something!!!!!!!");
        }
    }
    private void OnDestroy()
    {
        // Always unlisten to events when complete.
        AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
    }
    private int sdf = 0;
    public void ShowADs()
    {
        sdf++;
        if (sdf == AdsShowCount)
        {
            ShowRewarVideo();
            sdf = 0;
        }
        else
        {
            ShowInterstitial();
        }
    }
    #region Banner
    public void RequestBanner()
    {

#if UNITY_ANDROID
        string adUnitId = AndroidBannerID;
#elif UNITY_IPHONE
                string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            string adUnitId = "unexpected_platform";
#endif
        if (isTest)
        {
            adUnitId = "ca-app-pub-3940256099942544/6300978111";
        }
        if (adUnitId == "")
        {
            return;
        }
        AdSize adSize = AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        this.bannerView = new BannerView(adUnitId, adSize, AdPosition.Top);

        // Called when an ad request has successfully loaded.
        this.bannerView.OnAdLoaded += this.BannerAdLoad;
        // Called when an ad request failed to load.
        this.bannerView.OnAdFailedToLoad += this.BannerFailLoad;
        // Called when an ad is clicked.
        this.bannerView.OnAdOpening += this.BannerOpen;
        // Called when the user returned from the app after an ad click.
        this.bannerView.OnAdClosed += this.BannerAdClose;
        // Called when the ad click caused the user to leave the application.
        //this.bannerView.OnAdLeavingApplication += this.HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }

    public void HideBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
    }

    public void BannerAdLoad(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void BannerFailLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //Invoke("RequestBanner", 2);
        StartCoroutine(LoadBannerCo("Unity"));
    }

    public IEnumerator LoadBannerCo(string s)
    {
        yield return new WaitForSeconds(4);
        Invoke(nameof(RequestBanner), 0.2f);
    }

    public void BannerOpen(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void BannerAdClose(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }
    #endregion Banner

    #region Interstitial
    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = AndroidIntertitialID;
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif
        if (isTest)
        {
            adUnitId = "ca-app-pub-3940256099942544/1033173712";
        }
        if (adUnitId == "")
        {
            return;
        }
        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += InterstitialAdLoad;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += InterstitialAdFailToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += InterstitialOpening;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += InterstitialClose;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    public void InterstitialAdLoad(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void InterstitialAdFailToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        RequestInterstitial();
    }
    public void InterstitialOpening(object sender, EventArgs args)
    {
        Time.timeScale = 1;
        MonoBehaviour.print("HandleAdOpening event received");
    }

    public void InterstitialClose(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
        Time.timeScale = 1;
        RequestInterstitial();
    }

    public void ShowInterstitial()
    {
        if (interstitial.IsLoaded())
        {
            //isShowingAd = true;
            interstitial.Show();
        }
        else
        {
            RequestInterstitial();
        }
    }
    #endregion Interstitial

    #region Reward 
    public void LoadRewardVideo()
    {
        string adUnitId;
#if UNITY_ANDROID
        adUnitId = AndroidRewardVideoID;
#elif UNITY_IPHONE
                adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
        adUnitId = "unexpected_platform";
#endif
        if (isTest)
        {
            adUnitId = "ca-app-pub-3940256099942544/5224354917";
        }
        if (adUnitId == "")
        {
            return;
        }

        this.rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += RewardLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += RewardFailToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += RewardOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += RewardFailToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += RewardEarn;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += RewardCloseClick;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    public void RewardLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
    }

    public void RewardFailToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Invoke(nameof(LoadRewardVideo), 0.1f);
    }
    public void RewardOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }
    public void RewardFailToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdFailedToShow event received with message: " + args.Message);
    }

    public void RewardCloseClick(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdClosed event received");
    }

    public void RewardEarn(object sender, Reward args)
    {
        /*  MainController.instance.AddHint(5);
          FindObjectOfType<RewardDialog>().Close();*/
        Invoke(nameof(LoadRewardVideo), 0.2f);
    }
    public void ShowRewarVideo()
    {
        if (rewardedAd.IsLoaded())
        {
            //isShowingAd = true;
            rewardedAd.Show();
        }
        else
        {
            Invoke(nameof(LoadRewardVideo), 0.2f);
        }
    }
    #endregion Reward
}
[System.Serializable]

public class AdsCount
{
    public bool adsFix;
    public string interstitial;
    public string rewarded;
}
[System.Serializable]

public class AdxFive
{
    public string appOpen;
    public string banner;
    public string interstitial;
    public string rewarded;
    public string native;
}
[System.Serializable]

public class AdxFour
{
    public string appOpen;
    public string banner;
    public string interstitial;
    public string rewarded;
    public string native;
}
[System.Serializable]

public class AdxOne
{
    public string appOpen;
    public string banner;
    public string interstitial;
    public string rewarded;
    public string native;
}
[System.Serializable]

public class AdxThree
{
    public string appOpen;
    public string native;
    public string banner;
    public string interstitial;
    public string rewarded;
}
[System.Serializable]

public class AdxTwo
{
    public string appOpen;
    public string banner;
    public string interstitial;
    public string rewarded;
    public string native;
}
[System.Serializable]

public class CustomAds
{
    public string customImageBig;
    public string customImageSmall;
    public string appNameAds;
    public string descriptionOneAds;
    public string descriptionTwoAds;
    public string buttonTxt;
    public string playStoreUrl;
}
[System.Serializable]

public class FirstScreenSetting
{
    public string intersitialAppOpen;
}
[System.Serializable]

public class GetProfile
{
    public GoogleAdmob googleAdmob;
    public AdxOne adxOne;
    public AdxTwo adxTwo;
    public AdxThree adxThree;
    public AdxFour adxFour;
    public AdxFive adxFive;
    public AdsCount adsCount;
    public FirstScreenSetting firstScreenSetting;
    public CustomAds customAds;
    public string _id;
    public List<OtherInputField> otherInputFields;
    public string applicationMasterId;
    public DateTime date;
    public int __v;
}
[System.Serializable]

public class GoogleAdmob
{
    public string appOpen;
    public string native;
    public string banner;
    public string interstitial;
    public string rewarded;
}
[System.Serializable]
public class OtherInputField
{
    public string fieldId;
    public string fieldName;
    public object value;
    public bool isGlobal;
    public string _id;
}
[System.Serializable]
public class Root
{
    public List<GetProfile> getProfile;
    public bool isSuccess;
}