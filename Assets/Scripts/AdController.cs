using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class AdController : MonoBehaviour, IUnityAdsListener
{
    private string gameId = "3748085";
    private bool testMode = true;
    private string banner = "bannerAd";
    private string rewardedVideo = "rewardedVideo";
    public TextMeshProUGUI gemsText;
    public Button doubleButton;
    public UIController uiController;
    private bool doubleTaken;
    private int counter;
    public ShopController shopController;
    private int roundsSinceLastAd = 0;


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
        StartCoroutine(ShowBannerWhenInitialized());
        StartCoroutine(countTimeForAdCoroutine());
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

    public void ShowInterstitialAd()
    {
        StartCoroutine(ShowInterstitialAdCoroutine());
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

    public IEnumerator LoadRewardedBanner()
    {
        while (!Advertisement.IsReady(rewardedVideo))
        {
            yield return new WaitForSeconds(0.5f);
        }
        doubleButton.interactable = true;
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
        if (showResult == ShowResult.Finished && placementId == rewardedVideo)
        {
            uiController.unlockDoubleDownAchievement();
            doubleButton.interactable = false;
            doubleTaken = true;
            StartCoroutine(doubleGems());
        }
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
