using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdController : MonoBehaviour, IUnityAdsListener
{
    private string gameId = "3754351";
    private bool testMode = true;
    private string banner = "bannerAd";
    private string rewardedVideo = "rewardedVideo";
    private string rewardedVideoGems = "rewardedVideoGems";
    private string rewardedVideoSkin = "rewardedVideoSkin";
    public TextMeshProUGUI gemsText;
    public Button doubleButton;
    public UIController uiController;
    private bool doubleTaken;
    private int counter;
    public ShopController shopController;
    private int roundsSinceLastAd = 0;
    private int skinNumber;
    private bool ballSkin;


    // Start is called before the first frame update
    public void Start()
    {
        doubleButton.interactable = Advertisement.IsReady(rewardedVideo);
        if (doubleButton)
        {
            doubleButton.onClick.AddListener(ShowRewardedVideo);
        }
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
        if (PlayerPrefs.GetInt("adsRemoved") == 0)
        {
            StartCoroutine(ShowBannerWhenInitialized());
        }
        StartCoroutine(countTimeForAdCoroutine());
        StartCoroutine(LoadRewardedGemsBanner());
        StartCoroutine(LoadRewardedSkinBanner());
    }

    private IEnumerator countTimeForAdCoroutine()
    {
        while (true)
        {
            if (int.Parse(uiController.score.text) > 50 && !uiController.timeController.isGameOver() && !uiController.timeController.isGamePaused())
            {
                counter++;
            }
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    private IEnumerator ShowBannerWhenInitialized()
    {
        while (!Advertisement.isInitialized)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(banner);
    }

    public void removeBanner()
    {
        Advertisement.Banner.Hide();
    }

    public void ShowInterstitialAd()
    {
        if (PlayerPrefs.GetInt("adsRemoved") == 0)
        {
            StartCoroutine(ShowInterstitialAdCoroutine());
        }
    }
    private IEnumerator ShowInterstitialAdCoroutine()
    {
        if (Advertisement.IsReady() && counter >= 45 && roundsSinceLastAd > 1)
        {
            yield return new WaitForSeconds(0.15f);
            Advertisement.Show();
            counter = 0;
            roundsSinceLastAd = 0;
        }
        yield return null;
    }
    void ShowRewardedVideo()
    {
        Advertisement.Show(rewardedVideo);
    }

    public void ShowRewardedGemsVideo()
    {
        Advertisement.Show(rewardedVideoGems);
    }

    public void ShowRewardedSkinVideo(int skinNumber, bool ballSkin)
    {
        this.skinNumber = skinNumber;
        this.ballSkin = ballSkin;
        Advertisement.Show(rewardedVideoSkin);
    }

    public IEnumerator LoadRewardedBanner()
    {
        while (!Advertisement.IsReady(rewardedVideo))
        {
            yield return new WaitForSeconds(0.5f);
        }
        doubleButton.interactable = true;
    }
    public IEnumerator LoadRewardedGemsBanner()
    {
        while (!Advertisement.IsReady(rewardedVideoGems))
        {
            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator LoadRewardedSkinBanner()
    {
        while (!Advertisement.IsReady(rewardedVideoSkin))
        {
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == rewardedVideo && !doubleTaken)
        {
            doubleButton.interactable = true;
        }
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (showResult == ShowResult.Finished)
        {
            if (placementId == rewardedVideo)
            {
                uiController.unlockDoubleDownAchievement();
                doubleButton.interactable = false;
                doubleTaken = true;
                StartCoroutine(doubleGems());
            }
            else if (placementId == rewardedVideoGems)
            {
                StartCoroutine(increaseGemsBy25());
            }
            else if (placementId == rewardedVideoSkin)
            {
                if (ballSkin)
                {
                    PlayerPrefs.SetInt("adBall" + skinNumber, PlayerPrefs.GetInt("adBall" + skinNumber) + 1);
                    if (PlayerPrefs.GetInt("adBall" + skinNumber) >= 5)
                    {
                        PlayerPrefs.SetInt("ballSkin" + skinNumber, 1);
                    }
                }
                else
                {
                    PlayerPrefs.SetInt("adTrail" + skinNumber, PlayerPrefs.GetInt("adTrail" + skinNumber) + 1);
                    if (PlayerPrefs.GetInt("adTrail" + skinNumber) >= 5)
                    {
                        PlayerPrefs.SetInt("trailSkin" + skinNumber, 1);
                    }
                }
                shopController.updateShopButtons();
            }
        }
    }

    private IEnumerator increaseGemsBy25()
    {
        yield return new WaitForSeconds(0.25f);
        PlayerPrefs.SetInt("totalGems", PlayerPrefs.GetInt("totalGems") + 25);
        uiController.updateTotalGems();
        shopController.updateAmount();
        StartCoroutine(LoadRewardedGemsBanner());
        yield return null;
    }

    public void setDoubleTaken(bool doubleTaken)
    {
        this.doubleTaken = doubleTaken;
    }

    private IEnumerator doubleGems()
    {
        yield return new WaitForSeconds(0.25f);
        int gemsInt = int.Parse(gemsText.text);
        int gemsIntTemp = gemsInt;
        PlayerPrefs.SetInt("totalGems", PlayerPrefs.GetInt("totalGems") + gemsInt);
        shopController.updateAmount();
        for (int i = 0; i < gemsIntTemp; i++)
        {
            gemsInt++;
            gemsText.text = gemsInt.ToString();
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }

    public void OnUnityAdsDidError(string message) { }
    void IUnityAdsListener.OnUnityAdsDidStart(string placementId) { }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }

    public void ShowBanner()
    {
        Advertisement.Banner.Show();
    }

    public void incrementRoundsSinceLastAd()
    {
        roundsSinceLastAd++;
    }
}
