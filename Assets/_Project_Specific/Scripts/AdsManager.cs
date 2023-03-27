using UnityEngine;
using System;
using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdsManager : MonoBehaviour
{
    public static AdsManager inst;

    [Header("Android")] private string AndroidAppID = "ca-app-pub-1107552888714987~6336148998";

    private string AndroidBannerID,
        AndroidRewardVideoID,
        AndroidIntertitialID;

    [Header("IOS")] private string IOSAppID = "ca-app-pub-3940256099942544~1458002511";
    private string IOSBannerID = "ca-app-pub-3940256099942544/2934735716";

    private string IOSRewardVideoID = "ca-app-pub-3940256099942544/1712485313",
        IOSIntertitialID = "ca-app-pub-3940256099942544/4411468910";

    [Header("Splesh Screen Intrstitial ID")]
    private string SplashAndroidInterstitialID = "ca-app-pub-2981087280704608/3729013679";

    private string SplashIOSIntertitialID = "ca-app-pub-2981087280704608/1182221241";

    string AppID = "", BannerID = "", RewardVideoID = "", IntertitialID = "", Splash_InterstitialID = "";

    internal BannerView bannerView;
    internal InterstitialAd interstitial;
    internal RewardedAd rewardBasedVideo;

    internal string rewardType, interstitalType;

    public Root myDeserializedClass;
    private string AdURL = "https://ashafashion.store/vivek/omsaiinfo/godaddy/data.php";
    //private string AdURL = "http://android-admin-api.maheshpatel.me/v1/adSettingId?applicationMasterId=63f46a23f8445dcd0ed54e6c";


    public Button m_HideDaddy;
    public Button m_Simulator;

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
    }
    public void HomeScreen()
    {
        SceneManager.LoadScene(0);
        DOVirtual.DelayedCall(.1f, () =>
        {
            m_HideDaddy = GameObject.Find("Hide_Daddy").GetComponent<Button>();
            m_Simulator = GameObject.Find("Simulator").GetComponent<Button>();

            m_HideDaddy.onClick.AddListener(HIdeGame);
            m_Simulator.onClick.AddListener(Simulatorgame);
        });
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

        Application.targetFrameRate = 500;
        Input.multiTouchEnabled = false;

        MobileAds.SetiOSAppPauseOnBackground(true);
        StartCoroutine(getdata());
    }
    public void Btn()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            int Randoma = UnityEngine.Random.Range(1, 1000);
            if (Randoma % 2 == 0)
            {
                Debug.Log("even");
                RequestAndLoadInterstitialAd();
            }
            else
            {
                Debug.Log("odd");
                RequestAndLoadRewardedAd();
            }
        }
        else
        {
            //GameManager.instance.mInternetPenal.SetActive(true);
        }
    }
    public void requestBaner()
    {
        LoadBannerView();
    }
    public void destorybaner()
    {
        DestroyBanner();
    }

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder().Build();
    }

    //----------Banner Ad----------
    private bool isBannerLoded = false;

    internal void LoadBannerView()
    {
        isBannerLoded = false;

        // Clean up interstitial before using it
        bannerView?.Destroy();

        bannerView = new BannerView(BannerID,
            AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), AdPosition.Top);
        this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
        this.bannerView.OnAdFailedToLoad += this.BanerAdsFaild;
        this.bannerView.LoadAd(CreateAdRequest());
    }

    private void BanerAdsFaild(object sender, AdFailedToLoadEventArgs e)
    {
        if (AdXadUnitIdAndroidBanner != "")
        {
            AndroidBannerID = AdXadUnitIdAndroidBanner;
            requestBaner();
        }
    }

    private void HandleOnAdLoaded(object sender, EventArgs args)
    {
        isBannerLoded = true;
    }

    private void DestroyBanner()
    {
        bannerView?.Destroy();
    }
    //----------Interstitial Ad----------
    public void RequestAndLoadInterstitialAd(bool isSplashScreen = false)
    {
        if (interstitial != null && interstitial.IsLoaded())
            return;

        interstitial = new InterstitialAd(isSplashScreen ? Splash_InterstitialID : IntertitialID);
        // Add Event Handlers
        interstitial.OnAdLoaded += Interstitial_OnAdLoaded;
        interstitial.OnAdClosed += Interstitial_OnAdClosed;
        interstitial.OnAdFailedToLoad += Interstitial_OnAdFailToLoad;
        //  interstitial.OnAdFailedToShow += Interstitial_OnAdFailToShow;

        Debug.Log("Lodad interstial");
        // Load an interstitial ad
        interstitial.LoadAd(CreateAdRequest());
    }
    public bool ShowInterstitial(string interstitialType)
    {
        interstitalType = interstitialType;
        if (interstitial == null || !interstitial.IsLoaded()) return false;
        interstitial.Show();
        return true;
    }
    private void DestroyIntertitial()
    {
        interstitial?.Destroy();
    }
    private void Interstitial_OnAdClosed(object sender, System.EventArgs e)
    {
        Invoke(nameof(Interstitial_Close), 0.8f);
    }
    private void Interstitial_OnAdFailToLoad(object sender, System.EventArgs e)
    {
        if (AdXadUnitIdAndroidInterstitial != "")
        {
            IntertitialID = AdXadUnitIdAndroidInterstitial;
            SplashAndroidInterstitialID = AdXadUnitIdAndroidInterstitial;
            Btn();
        }

        Debug.Log("fail to load");
    }
    private void Interstitial_OnAdFailToShow(object sender, System.EventArgs e)
    {
        Debug.Log("fail to show");
    }
    private void Interstitial_OnAdLoaded(object sender, System.EventArgs e)
    {
        Debug.Log("Intertial Ready to load");
        //  ShowInterstitial("");
        // if (ShowInterstitial(""))
        // {
        //
        // }
    }

    public void Interstitial_Close()
    {
        switch (interstitalType)
        {
            case "":
                Debug.Log("interstioal close call back");
                //  AdsLoading.SetActive(false);
                break;
        }
    }
    //----------Reward Video----------
    private bool isSendOnlyRequest = false;

    //Call when you want to show reworded video
    public void LoadAndShow_RewardVideo(string RewardType)
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            RequestAndLoadRewardedAd();
            this.rewardType = RewardType;
        }
        else
        {
            Debug.LogError("Check Network Connection!");
        }
    }

    private void RequestAndLoadRewardedAd(bool sendOnlyRequest = false)
    {
        isSendOnlyRequest = sendOnlyRequest;
        if (rewardBasedVideo != null && rewardBasedVideo.IsLoaded())
        {
            if (!sendOnlyRequest)
                ShowRewardVideo();
            return;
        }


        rewardBasedVideo = new RewardedAd(RewardVideoID);

        rewardBasedVideo.OnUserEarnedReward += HandleUserEarnedReward;
        rewardBasedVideo.OnAdClosed += HandleRewardedAdClosed;
        rewardBasedVideo.OnAdLoaded += HandleRewardedAdLoded;
        rewardBasedVideo.OnAdFailedToLoad += HandleRewardAdFaildToLoad;

        rewardBasedVideo.LoadAd(CreateAdRequest());
    }

    private bool ShowRewardVideo()
    {
        if (rewardBasedVideo == null || !rewardBasedVideo.IsLoaded()) return false;
        rewardBasedVideo.Show();
        return true;

    }

    private void HandleRewardedAdLoded(object sender, EventArgs e)
    {
        //if (GameManager.Inst.adLoader.activeSelf)
        ShowRewardVideo();
    }

    private void HandleUserEarnedReward(object sender, Reward e)
    {
        Invoke(nameof(Give_Reward), 0.08f);
    }

    private void HandleRewardAdFaildToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        if (AdXadUnitIdAndroidRewardVideo != "")
        {
            AndroidRewardVideoID = AdXadUnitIdAndroidRewardVideo;
            RequestAndLoadRewardedAd();
        }
        /*if (!isSendOnlyRequest && GameManager.Inst.adLoader.activeSelf)
        {
            GameManager.Inst.Make_Toast("Ad Not Available!");
        }

        GameManager.Inst.Set_Ad_Loader_panel(false);*/
    }

    private void HandleRewardedAdClosed(object sender, EventArgs e)
    {
        //GameManager.Inst.Set_Ad_Loader_panel(false);
    }
    private void Give_Reward()
    {
        switch (rewardType)
        {
            case "GameOver":
                //FindObjectOfType<GameOverPopUpController>()?.Give_Reward();
                break;
        }
    }
    internal void RemoveAdsApply()
    {
        DestroyBanner();
        DestroyIntertitial();
    }
    private string AdXadUnitIdAndroidBanner;
    private string AdXadUnitIdAndroidInterstitial;
    private string AdXadUnitIdAndroidRewardVideo;

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

            // AdXadUnitIdAndroidRewardVideo = myDeserializedClass.getProfile[0].adxOne.rewarded;
            // AdXadUnitIdAndroidInterstitial = myDeserializedClass.getProfile[0].adxOne.interstitial;
            // AdXadUnitIdAndroidBanner = myDeserializedClass.getProfile[0].adxOne.banner;

#if UNITY_ANDROID
            AppID = AndroidAppID;
            BannerID = AndroidBannerID;
            IntertitialID = AndroidIntertitialID;
            RewardVideoID = AndroidRewardVideoID;


            /*  Debug.Log("BannerId::" + BannerID);
              Debug.Log("IntertitialID::" + IntertitialID);
              Debug.Log("RewardVideoID::" + RewardVideoID);*/

            if (BannerID == "")
            {
                BannerID = "ca-app-pub-3940256099942544/6300978111";
                Debug.Log("BannerId::" + BannerID);
            }
            if (IntertitialID == "")
            {
                IntertitialID = "ca-app-pub-3940256099942544/1033173712";
                Debug.Log("Inter::" + IntertitialID);
            }
            if (RewardVideoID == "")
            {
                RewardVideoID = "ca-app-pub-3940256099942544/5224354917";
                Debug.Log("rewordvideo:" + RewardVideoID);
            }

#elif UNITY_IPHONE
            AppID = IOSAppID;
            BannerID = IOSBannerID;
            RewardVideoID = IOSRewardVideoID;
            IntertitialID = IOSIntertitialID;
            
            Splash_InterstitialID = SplashIOSIntertitialID;
#else
        AppID = "unexpected_platform";
        BannerID = "unexpected_platform";
        RewardVideoID = "unexpected_platform";
        IntertitialID = "unexpected_platform";
#endif

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(initStatus => { });

            if (BannerID != "")
            {
                requestBaner();
                Debug.Log("Request Banner");
            }
            else
            {
                //BannerID = "ca-app-pub-3940256099942544/6300978111";
                //requestBaner();
            }
        }
        else
        {
            Debug.Log("Wrong Something!!!!!!!");
        }
    }
    private void OnApplicationQuit()
    {
        //rewardBasedVideo?.Destroy();
        RemoveAdsApply();
    }
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