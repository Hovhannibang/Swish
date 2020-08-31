using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ShopController : MonoBehaviour
{
    public GameObject uiBall;
    public GameObject playBall;
    public UIController uiController;
    public NotificationController notificationController;
    public AdController adController;
    public ScrollRect ballScrollView;
    public ScrollRect trailScrollView;
    public ScrollRect gemsScrollView;
    public GameObject ballSkinScrollViewContent;
    public GameObject trailSkinScrollViewContent;
    public GameObject gemsScrollViewContent;
    public GameObject buttonPrefab;
    private Coroutine currentUpdateGemsCoroutine;
    private GameObject selectedBall;
    private GameObject selectedTrail;
    private Button dailyButton;
    public TextMeshProUGUI amount;
    public GameObject skinConfirmationDialog;
    public GameObject customizeButtonAchievementInfo;
    public GameObject gemsButtonAchievementInfo;
    public GameObject moreButton;
    public GameObject noAdsButton;
    public GameObject moreButtonAdsRemoved;

    private readonly char[] splitColon = { ':' };
    private readonly char[] splitMinus = { '-' };
    private Vector2 textLeft = new Vector2(-77.5f, -4.2f);

    private readonly Dictionary<int, string[]> ballSkinDictionary = new Dictionary<int, string[]> { // TODO SKIN FRAGMENT COLORS
        {15, new string[]{ "x/5", "soccerball:21-43-58-72:#D0D0D0-#E3E3E3-#282828-#444444" } },
        {16, new string[]{ "x/5", "volleyball:14-35-46-61-72:#50BFD4-#EFEDEE-#FADC60-#F2C42C-#57D0E6" } },
        {17, new string[]{ "x/5", "beachball:3-15-17-33-41-43-44-59-65-72:#FFFFFF-#F26E91-#ED8600-#FFB5C5-#91EDFF-#F5DC1D-#EBF9FF-#7191F0-#FFE940-#FF9000" } },
        {18, new string[]{ "x/5", "baseball:31-62-67-72:#C1C2C4-#EDEEF0-#C8523B-#A75546" } },
        {19, new string[]{ "x/5", "basketball:9-19-46-72:#FBBD61-#FF4F4D-#FC985A-#FE7453" } },
        {23, new string[]{ "x/5", "bowlingball:1-35-38-72:#C70024-#FF0C38-#830018-#FF3F62" } },
        {24, new string[]{ "x/5", "tennisball:7-36-43-72:#FFFFFF-#9ADB05-#F3EEE9-#83B904" } },
        {26, new string[]{ "#'? ? ?'", "questionmark:26-43-56-63-66-72:#000000-#0C0C0C-#191919-#262626-#333333-#ADADAD" } },
        {20, new string[]{ "#'ADBLOCKER'", "adblocker:4-40-72:#FFFFFF-#FF0C38-#C70024" } },
        {0, new string[]{ "0", "#FFFFFF"} },
        {1, new string[]{ "100", "#0172BD" } },
        {2, new string[]{ "100", "#FF01FF" } },
        {3, new string[]{ "100", "#93268F" } },
        {4, new string[]{ "100", "#009346" } },
        {5, new string[]{ "100", "#00FF01" } },
        {6, new string[]{ "100", "#FFFF01" } },
        {7, new string[]{ "100", "#FACC01" } },
        {8, new string[]{ "100", "#FF7E01" } },
        {9, new string[]{ "100", "#FF0002" } },
        {10, new string[]{ "100", "#D60000" } },
        {27, new string[]{ "250", "Flags/albania:14-72:#000000-#D80027" } },
        {28, new string[]{ "250", "Flags/america:32-46-72:#F0F0F0-#0052B4-#D80027" } },
        {29, new string[]{ "250", "Flags/argentina:35-39-72:#F0F0F0-#FFDA44-#338AF3" } },
        {30, new string[]{ "250", "Flags/armenia:31-51-72:#0052B4-#FF9811-#D80027" } },
        {31, new string[]{ "250", "Flags/australia:11-63-72:#F0F0F0-#0052B4-#D80027" } },
        {32, new string[]{ "250", "Flags/austria:31-72:#F0F0F0-#D80027" } },
        {33, new string[]{ "250", "Flags/belgium:20-51-72:#000000-#FFDA44-#D80027" } },
        {34, new string[]{ "250", "Flags/brazil:6-20-69-72:#0052B4-#FFDA44-#6DA544-#F0F0F0" } },
        {35, new string[]{ "250", "Flags/britain:27-44-72:#F0F0F0-#0052B4-#D80027" } },
        {36, new string[]{ "250", "Flags/canada:34-72:#F0F0F0-#D80027" } },
        {37, new string[]{ "250", "Flags/china:6-72:#FFDA44-#D80027" } },
        {38, new string[]{ "250", "Flags/croatia:26-46-48-72:#F0F0F0-#0052B4-#338AF3-#D80027" } },
        {39, new string[]{ "250", "Flags/denmark:22-72:#F0F0F0-#D80027" } },
        {40, new string[]{ "250", "Flags/egypt:28-48-51-72:#F0F0F0-#000000-#FF9811-#D80027" } },
        {41, new string[]{ "250", "Flags/finland:50-72:#F0F0F0-#0052B4" } },
        {42, new string[]{ "250", "Flags/france:31-51-72:#F0F0F0-#0052B4-#D80027" } },
        {43, new string[]{ "250", "Flags/germany:20-40-72:#000000-#FFDA44-#D80027" } },
        {44, new string[]{ "250", "Flags/ghana:4-24-52-72:#000000-#496E2D-#FFDA44-#D80027" } },
        {45, new string[]{ "250", "Flags/greece:37-72:#338AF3-#F0F0F0" } },
        {46, new string[]{ "250", "Flags/india:31-48-55-72:#F0F0F0-#6DA544-#0052B4-#FF9811" } },
        {47, new string[]{ "250", "Flags/indonesia:36-72:#F0F0F0-#A2001D" } },
        {48, new string[]{ "250", "Flags/iran:15-52-72:#6DA544-#F0F0F0-#D80027" } },
        {49, new string[]{ "250", "Flags/italy:31-51-72:#F0F0F0-#6DA544-#D80027" } },
        {50, new string[]{ "250", "Flags/japan:58-72:#F0F0F0-#D80027" } },
        {51, new string[]{ "250", "Flags/netherlands:31-51-72:#F0F0F0-#0052B4-#A2001D" } },
        {52, new string[]{ "250", "Flags/norway:18-40-72:#F0F0F0-#0052B4-#D80027" } },
        {53, new string[]{ "250", "Flags/philippines:13-40-44-72:#F0F0F0-#0052B4-#FFDA44-#D80027" } },
        {54, new string[]{ "250", "Flags/portugal:16-21-72:#6DA544-#FFDA44-#D80027" } },
        {55, new string[]{ "250", "Flags/republicofpoland:36-72:#F0F0F0-#D80027" } },
        {56, new string[]{ "250", "Flags/russia:20-51-72:#F0F0F0-#0052B4-#D80027" } },
        {57, new string[]{ "250", "Flags/singapore:39-72:#F0F0F0-#D80027" } },
        {58, new string[]{ "250", "Flags/southafrica:17-28-32-50-54-72:#6DA544-#F0F0F0-#000000-#0052B4-#FFDA44-#D80027" } },
        {59, new string[]{ "250", "Flags/southkorea:56-63-67-72:#F0F0F0-#000000-#0052B4-#D80027" } },
        {60, new string[]{ "250", "Flags/spain:31-72:#FFDA44-#D80027" } },
        {61, new string[]{ "250", "Flags/sweden:50-72:#0052B4-#FFDA44" } },
        {62, new string[]{ "250", "Flags/switzerland:14-72:#F0F0F0-#D80027" } },
        {63, new string[]{ "250", "Flags/thailand:24-56-72:#F0F0F0-#0052B4-#D80027" } },
        {64, new string[]{ "250", "Flags/turkey:4-72:#F0F0F0-#D80027" } },
        {65, new string[]{ "250", "Flags/uganda:21-27-53-72:#000000-#F0F0F0-#FFDA44-#D80027" } },
        {66, new string[]{ "250", "Flags/unitedarabemirates:21-36-51-72:#F0F0F0-#000000-#6DA544-#A2001D" } },
    };

    private readonly Dictionary<int, string[]> trailSkinDictionary = new Dictionary<int, string[]> {
        {0, new string[]{ "0", "#FFFFFF"} },
        {1, new string[]{ "100", "#71ABDD"} },
        {2, new string[]{ "100", "#6BCADE"} },
        {3, new string[]{ "100", "#82CCB5"} },
        {4, new string[]{ "100", "#B6D884"} },
        {5, new string[]{ "100", "#FFF68F"} },
        {6, new string[]{ "100", "#FDCD79"} },
        {7, new string[]{ "100", "#F497AA"} },
        {8, new string[]{ "#'DOUBLE DOWN'", "gem:#00D272:#009C54" } },
        {9, new string[]{ "#'STARS'", "star:#FFEF68:#D8BB00" } },
    };

    public GameObject[] gemButtons;

    void Start()
    {
        amount.text = PlayerPrefs.GetInt("totalGems").ToString();
        if (!PlayerPrefs.HasKey("lastTimeDailyTaken"))
        {
            PlayerPrefs.SetString("lastTimeDailyTaken", "01010001010101");
        }
        PlayerPrefs.SetInt("ballSkin0", 1);
        PlayerPrefs.SetInt("trailSkin0", 1);
        initiateBallShop();
        initiateTrailShop();
        initiateGemsShop();
        setLastSelectedSkins();
        StartCoroutine(checkIfDailyAvailable());

        if (PlayerPrefs.GetInt("adsRemoved") == 1)
        {
            moreButton.SetActive(false);
            noAdsButton.SetActive(false);
            moreButtonAdsRemoved.SetActive(true);
        }
    }

    private void initiateBallShop()
    {
        for (int i = 0; i < ballSkinDictionary.Count; i++)
        {
            string[] currentElement = ballSkinDictionary.ElementAt(i).Value;
            int currentKey = ballSkinDictionary.ElementAt(i).Key;
            GameObject temp = Instantiate(buttonPrefab);
            if (PlayerPrefs.GetInt("ballSkin" + currentKey) == 1)
            {
                temp.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                TextMeshProUGUI textMesh = temp.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
                if (currentElement[0].EndsWith("/5"))
                {
                    temp.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
                    textMesh.gameObject.transform.localPosition = textLeft;
                    textMesh.color = Color.white;
                    textMesh.text = PlayerPrefs.GetInt("adBall" + currentKey).ToString() + "/5";
                    temp.transform.GetChild(4).gameObject.SetActive(true);
                    temp.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "WATCH ADS";
                }
                else if (currentElement[0].StartsWith("#"))
                {
                    temp.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
                    temp.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
                    textMesh.text = "ACHIEVE";
                    textMesh.gameObject.transform.localPosition = textLeft;
                    textMesh.fontSize = 22;
                    textMesh.fontStyle = FontStyles.Bold;
                    textMesh.color = Color.white;
                    temp.transform.GetChild(4).gameObject.SetActive(true);
                    temp.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = currentElement[0].Replace("#", "");
                }
                else
                {
                    temp.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = currentElement[0];
                }
            }

            if (currentElement[1].StartsWith("#"))
            {
                ColorUtility.TryParseHtmlString(currentElement[1], out Color color);
                temp.transform.GetChild(0).GetComponent<Image>().color = color;
            }
            else
            {
                temp.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(currentElement[1].Split(splitColon)[0]);
            }
            temp.GetComponent<Button>().onClick.AddListener(() => buyOrSelectBall(temp, currentKey));
            temp.transform.SetParent(ballSkinScrollViewContent.transform, false);
        }
        ballScrollView.verticalNormalizedPosition = 1;
    }

    private void initiateTrailShop()
    {
        Sprite trailSprite = Resources.Load<Sprite>("trail");
        Vector3 scale120perc = new Vector3(1.2f, 1.2f, 1.2f);
        for (int i = 0; i < trailSkinDictionary.Count; i++)
        {
            string[] currentElement = trailSkinDictionary.ElementAt(i).Value;
            int currentKey = trailSkinDictionary.ElementAt(i).Key;
            GameObject temp = Instantiate(buttonPrefab);
            Image previewImage = temp.transform.GetChild(0).GetComponent<Image>();
            previewImage.sprite = trailSprite;
            if (PlayerPrefs.GetInt("trailSkin" + currentKey) == 1)
            {
                temp.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                TextMeshProUGUI textMesh = temp.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
                if (currentElement[0].EndsWith("/5"))
                {
                    temp.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
                    textMesh.gameObject.transform.localPosition = textLeft;
                    textMesh.color = Color.white;
                    textMesh.text = PlayerPrefs.GetInt("adTrail" + currentKey).ToString() + "/5";
                    temp.transform.GetChild(4).gameObject.SetActive(true);
                    temp.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "WATCH ADS";
                }
                else if (currentElement[0].StartsWith("#"))
                {
                    temp.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
                    temp.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
                    textMesh.text = "ACHIEVE";
                    textMesh.gameObject.transform.localPosition = textLeft;
                    textMesh.fontSize = 22;
                    textMesh.fontStyle = FontStyles.Bold;
                    textMesh.color = Color.white;
                    temp.transform.GetChild(4).gameObject.SetActive(true);
                    temp.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = currentElement[0].Replace("#", "");
                }
                else
                {
                    temp.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = currentElement[0];
                }
            }
            if (currentElement[1].StartsWith("#"))
            {
                temp.transform.GetChild(0).localScale = scale120perc;
                ColorUtility.TryParseHtmlString(currentElement[1], out Color color);
                previewImage.color = color;
            }
            else
            {
                ColorUtility.TryParseHtmlString(currentElement[1].Split(splitColon)[1], out Color previewColor1);
                Color.RGBToHSV(previewColor1, out float pc1h, out float pc1s, out float pc1v);

                ColorUtility.TryParseHtmlString(currentElement[1].Split(splitColon)[2], out Color previewColor2);
                Color.RGBToHSV(previewColor2, out float pc2h, out float pc2s, out float pc2v);

                Transform particlePreview = temp.transform.GetChild(3);
                previewImage.gameObject.SetActive(false);
                particlePreview.gameObject.SetActive(true);

                Sprite sprite = Resources.Load<Sprite>(currentElement[1].Split(splitColon)[0]);
                for (int j = 0; j < particlePreview.childCount; j++)
                {
                    particlePreview.GetChild(j).GetComponent<Image>().sprite = sprite;
                    particlePreview.GetChild(j).GetComponent<Image>().color = Random.ColorHSV(pc1h, pc2h, pc1s, pc2s, pc1v, pc2v);
                }
            }
            temp.GetComponent<Button>().onClick.AddListener(() => buyOrSelectTrail(temp, currentKey));
            temp.transform.SetParent(trailSkinScrollViewContent.transform, false);
        }
        trailScrollView.verticalNormalizedPosition = 1;
    }

    private void initiateGemsShop()
    {
        for (int i = 0; i < gemButtons.Length; i++)
        {
            GameObject temp = Instantiate(gemButtons[i]);
            Button tempButton = temp.GetComponent<Button>();
            int gemAmount = int.Parse(temp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Replace(" Gems", ""));
            if (i > 1)
            {
                tempButton.onClick.AddListener(() => increaseGems(gemAmount));
            }
            else if (i == 0)
            {
                dailyButton = tempButton;
                tempButton.onClick.AddListener(() =>
                {
                    PlayerPrefs.SetString("lastTimeDailyTaken", DateTime.Now.ToString("ddMMyyyyHHmmss"));
                    tempButton.interactable = false;
                    increaseGems(gemAmount);
                    notificationController.setupDailyNotification();
                    customizeButtonAchievementInfo.SetActive(false);
                    gemsButtonAchievementInfo.SetActive(false);

                });
            }
            else if (i == 1)
            {
                tempButton.onClick.AddListener(() =>
                {
                    adController.ShowRewardedGemsVideo();
                });
            }

            temp.transform.SetParent(gemsScrollViewContent.transform, false);
        }
        gemsScrollView.verticalNormalizedPosition = 1;
    }

    public void increaseGems(int gems)
    {
        int currentAmount = PlayerPrefs.GetInt("totalGems");
        PlayerPrefs.SetInt("totalGems", currentAmount + gems);
        updateAmount();
        uiController.updateTotalGems();
    }

    private void buyOrSelectBall(GameObject button, int number)
    {
        ballSkinDictionary.TryGetValue(number, out string[] values);
        string colorOrSkin = values[1];
        if (button.transform.GetChild(2).gameObject.activeSelf)
        {
            string text = button.transform.GetChild(2).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
            if (text.EndsWith("/5"))
            {
                adController.ShowRewardedSkinVideo(number, true);
            }
            else if (!text.Equals("ACHIEVE"))
            {
                int price = int.Parse(text);
                int totalGems = PlayerPrefs.GetInt("totalGems");
                if (totalGems >= price)
                {
                    ballScrollView.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    ballScrollView.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0.25f;
                    skinConfirmationDialog.transform.GetChild(6).GetComponent<Image>().sprite = button.transform.GetChild(0).GetComponent<Image>().sprite;
                    skinConfirmationDialog.transform.GetChild(6).GetComponent<Image>().color = button.transform.GetChild(0).GetComponent<Image>().color;
                    skinConfirmationDialog.SetActive(true);
                    skinConfirmationDialog.transform.GetChild(skinConfirmationDialog.transform.childCount - 1).GetComponent<Button>().onClick.RemoveAllListeners();
                    skinConfirmationDialog.transform.GetChild(skinConfirmationDialog.transform.childCount - 1).GetComponent<Button>().onClick.AddListener(() =>
                    {
                        PlayerPrefs.SetInt("ballSkin" + number, 1);
                        totalGems -= price;
                        PlayerPrefs.SetInt("totalGems", totalGems);
                        uiController.updateTotalGems();
                        updateAmount();
                        button.transform.GetChild(2).gameObject.SetActive(false);
                        selectedBall.transform.GetChild(1).gameObject.SetActive(false);
                        selectedBall = button;
                        selectedBall.transform.GetChild(1).gameObject.SetActive(true);

                        setBallColor(colorOrSkin);
                        PlayerPrefs.SetInt("lastBallSkin", number);

                        if (PlayerPrefs.GetInt("ach6") == 0)
                        {
                            PlayerPrefs.SetInt("achtakeable6", 1);
                            uiController.activateAchievementInfo(6);
                        }
                    });
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
            selectedBall.transform.GetChild(1).gameObject.SetActive(false);
            selectedBall = button;
            selectedBall.transform.GetChild(1).gameObject.SetActive(true);
            setBallColor(colorOrSkin);
            PlayerPrefs.SetInt("lastBallSkin", number);
        }
    }

    private void setBallColor(string colorOrSkin)
    {
        if (colorOrSkin.StartsWith("#"))
        {
            ColorUtility.TryParseHtmlString(colorOrSkin, out Color color);
            uiBall.GetComponent<Image>().sprite = Resources.Load<Sprite>("standardBall");
            uiBall.GetComponent<Image>().color = color;
            playBall.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("standardBall");
            playBall.GetComponent<SpriteRenderer>().color = color;
            Material tempMat = new Material(playBall.GetComponent<SpriteRenderer>().material)
            {
                color = color
            };
            foreach (GameObject frag in playBall.GetComponent<Explodable>().fragments)
            {
                frag.GetComponent<MeshRenderer>().material = tempMat;
            }
        }
        else
        {
            string[] split = colorOrSkin.Split(splitColon);
            string skin = split[0];
            string[] colorAmounts = split[1].Split(splitMinus);
            string[] colors = split[2].Split(splitMinus);
            uiBall.GetComponent<Image>().sprite = Resources.Load<Sprite>(skin);
            uiBall.GetComponent<Image>().color = Color.white;
            playBall.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(skin);
            playBall.GetComponent<SpriteRenderer>().color = Color.white;
            GameObject[] fragments = playBall.GetComponent<Explodable>().fragments.OrderBy(x => Guid.NewGuid()).ToList().ToArray();
            int colorAmountStart = 0;
            int colorAmountEnd;
            for (int i = 0; i < colorAmounts.Length; i++)
            {
                ColorUtility.TryParseHtmlString(colors[i], out Color color);
                Material tempMat = new Material(playBall.GetComponent<SpriteRenderer>().material)
                {
                    color = color
                };
                colorAmountEnd = int.Parse(colorAmounts[i]);
                for (int j = colorAmountStart; j < colorAmountEnd; j++)
                {
                    fragments[j].GetComponent<MeshRenderer>().material = tempMat;
                }
                colorAmountStart = colorAmountEnd;
            }
        }
    }

    private void buyOrSelectTrail(GameObject button, int number)
    {
        trailSkinDictionary.TryGetValue(number, out string[] values);
        string colorOrSkin = values[1];
        if (button.transform.GetChild(2).gameObject.activeSelf)
        {
            string text = button.transform.GetChild(2).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
            if (text.EndsWith("/5"))
            {
                adController.ShowRewardedSkinVideo(number, false);
            }
            else if (!text.Equals("ACHIEVE"))
            {
                int price = int.Parse(button.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text);
                int totalGems = PlayerPrefs.GetInt("totalGems");
                if (totalGems >= price)
                {
                    ballScrollView.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    ballScrollView.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0.25f;
                    skinConfirmationDialog.transform.GetChild(6).GetComponent<Image>().sprite = button.transform.GetChild(0).GetComponent<Image>().sprite;
                    skinConfirmationDialog.transform.GetChild(6).GetComponent<Image>().color = button.transform.GetChild(0).GetComponent<Image>().color;
                    skinConfirmationDialog.SetActive(true);
                    skinConfirmationDialog.transform.GetChild(skinConfirmationDialog.transform.childCount - 1).GetComponent<Button>().onClick.RemoveAllListeners();
                    skinConfirmationDialog.transform.GetChild(skinConfirmationDialog.transform.childCount - 1).GetComponent<Button>().onClick.AddListener(() =>
                    {
                        PlayerPrefs.SetInt("trailSkin" + number, 1);
                        totalGems -= price;
                        PlayerPrefs.SetInt("totalGems", totalGems);
                        uiController.updateTotalGems();
                        updateAmount();
                        button.transform.GetChild(2).gameObject.SetActive(false);
                        selectedTrail.transform.GetChild(1).gameObject.SetActive(false);
                        selectedTrail = button;
                        selectedTrail.transform.GetChild(1).gameObject.SetActive(true);

                        setTrailColor(colorOrSkin);
                        PlayerPrefs.SetInt("lastTrailSkin", number);

                        if (PlayerPrefs.GetInt("ach6") == 0)
                        {
                            PlayerPrefs.SetInt("achtakeable6", 1);
                            uiController.activateAchievementInfo(6);
                        }
                    });
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
            selectedTrail.transform.GetChild(1).gameObject.SetActive(false);
            selectedTrail = button;
            selectedTrail.transform.GetChild(1).gameObject.SetActive(true);
            setTrailColor(colorOrSkin);
            PlayerPrefs.SetInt("lastTrailSkin", number);
        }
    }

    private void setTrailColor(string colorOrSkin)
    {
        ParticleSystem ps = playBall.GetComponent<ParticleSystem>();
        ParticleSystem.EmissionModule em = ps.emission;
        ParticleSystem.MainModule mm = ps.main;
        if (colorOrSkin.StartsWith("#"))
        {
            ColorUtility.TryParseHtmlString(colorOrSkin, out Color color);
            playBall.GetComponent<TrailRenderer>().startColor = color;
            playBall.GetComponent<TrailRenderer>().endColor = new Color(color.r, color.g, color.b, 0f);
            playBall.GetComponent<TrailRenderer>().enabled = true;
            em.enabled = false;
        }
        else
        {
            playBall.GetComponent<TrailRenderer>().enabled = false;
            string[] split = colorOrSkin.Split(splitColon);
            ColorUtility.TryParseHtmlString(split[1], out Color color1);
            ColorUtility.TryParseHtmlString(split[2], out Color color2);
            ParticleSystem.MinMaxGradient grad = new ParticleSystem.MinMaxGradient(color1, color2)
            {
                mode = ParticleSystemGradientMode.TwoColors
            };
            mm.startColor = grad;
            ps.textureSheetAnimation.SetSprite(0, Resources.Load<Sprite>(split[0]));
            em.enabled = true;
        }
    }

    public void updateShopButtons()
    {
        for (int i = 0; i < ballSkinScrollViewContent.transform.childCount; i++)
        {
            int currentKey = ballSkinDictionary.ElementAt(i).Key;
            Transform buttonTransform = ballSkinScrollViewContent.transform.GetChild(i).transform;
            if (buttonTransform.GetChild(2).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text.EndsWith("/5"))
            {
                buttonTransform.GetChild(2).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("adBall" + currentKey).ToString() + "/5";
            }
            if (PlayerPrefs.GetInt("ballSkin" + currentKey) == 1)
            {
                buttonTransform.GetChild(2).gameObject.SetActive(false);
                buttonTransform.GetChild(4).gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < trailSkinScrollViewContent.transform.childCount; i++)
        {
            int currentKey = trailSkinDictionary.ElementAt(i).Key;
            Transform buttonTransform = trailSkinScrollViewContent.transform.GetChild(i).transform;
            if (buttonTransform.GetChild(2).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text.EndsWith("/5"))
            {
                buttonTransform.GetChild(2).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetInt("adTrail" + currentKey).ToString() + "/5";
            }
            if (PlayerPrefs.GetInt("trailSkin" + currentKey) == 1)
            {
                buttonTransform.GetChild(2).gameObject.SetActive(false);
                buttonTransform.GetChild(4).gameObject.SetActive(false);
            }
        }
    }

    public void updateAmount()
    {
        if (currentUpdateGemsCoroutine != null)
        {
            StopCoroutine(currentUpdateGemsCoroutine);
        }
        currentUpdateGemsCoroutine = StartCoroutine(updateShopGemsCoroutine());
    }

    private IEnumerator updateShopGemsCoroutine()
    {
        int playerPrefsGems = PlayerPrefs.GetInt("totalGems");

        if (int.Parse(amount.text) < playerPrefsGems)
        {
            while (int.Parse(amount.text) < playerPrefsGems)
            {
                amount.text = (int.Parse(amount.text) + 5).ToString();
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (int.Parse(amount.text) > playerPrefsGems)
            {
                amount.text = (int.Parse(amount.text) - 5).ToString();
                yield return new WaitForEndOfFrame();
            }
        }
        switch (int.Parse(amount.text) - playerPrefsGems)
        {
            case 4:
                amount.text = (int.Parse(amount.text) - 4).ToString();
                break;
            case 3:
                amount.text = (int.Parse(amount.text) - 3).ToString();
                break;
            case 2:
                amount.text = (int.Parse(amount.text) - 2).ToString();
                break;
            case 1:
                amount.text = (int.Parse(amount.text) - 1).ToString();
                break;
            case -1:
                amount.text = (int.Parse(amount.text) + 1).ToString();
                break;
            case -2:
                amount.text = (int.Parse(amount.text) + 2).ToString();
                break;
            case -3:
                amount.text = (int.Parse(amount.text) + 3).ToString();
                break;
            case -4:
                amount.text = (int.Parse(amount.text) + 4).ToString();
                break;
            default:
                break;
        }
        yield return null;
    }

    private void setLastSelectedSkins()
    {
        int lastBallSkin = PlayerPrefs.GetInt("lastBallSkin");
        int lastTrailSkin = PlayerPrefs.GetInt("lastTrailSkin");

        selectedBall = ballSkinScrollViewContent.transform.GetChild(Array.IndexOf(ballSkinDictionary.Keys.ToArray(), lastBallSkin)).gameObject;
        selectedBall.transform.GetChild(1).gameObject.SetActive(true);
        ballSkinDictionary.TryGetValue(lastBallSkin, out string[] valuesBall);
        setBallColor(valuesBall[1]);

        selectedTrail = trailSkinScrollViewContent.transform.GetChild(Array.IndexOf(trailSkinDictionary.Keys.ToArray(), lastTrailSkin)).gameObject;
        selectedTrail.transform.GetChild(1).gameObject.SetActive(true);
        trailSkinDictionary.TryGetValue(lastTrailSkin, out string[] valuesTrail);
        setTrailColor(valuesTrail[1]);
    }

    private IEnumerator checkIfDailyAvailable()
    {
        while (true)
        {
            if (DateTime.Compare(DateTime.ParseExact(PlayerPrefs.GetString("lastTimeDailyTaken"), "ddMMyyyyHHmmss", CultureInfo.InvariantCulture).AddDays(1), DateTime.Now) <= 0)
            {
                dailyButton.interactable = true;
                dailyButton.transform.GetChild(3).gameObject.SetActive(true);
                customizeButtonAchievementInfo.SetActive(true);
                gemsButtonAchievementInfo.SetActive(true);
            }
            else
            {
                dailyButton.interactable = false;
            }
            yield return new WaitForSecondsRealtime(1);
        }
    }

    public void removeAds()
    {
        //TODO REMOVE ADS IAP
        PlayerPrefs.SetInt("adsRemoved", 1);
        adController.removeBanner();
        moreButton.SetActive(false);
        noAdsButton.SetActive(false);
        moreButtonAdsRemoved.SetActive(true);
        if (PlayerPrefs.GetInt("ach12") == 0)
        {
            PlayerPrefs.SetInt("achtakeable12", 1);
            PlayerPrefs.SetInt("ballSkin20", 1);
            uiController.activateAchievementInfo(12);
        }
    }
}
