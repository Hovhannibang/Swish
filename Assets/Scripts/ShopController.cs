using System;
using System.Collections;
using System.Collections.Generic;
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
    public TextMeshProUGUI amount;
    public GameObject skinConfirmationDialog;

    private char[] splitColon = { ':' };
    private char[] splitMinus = { '-' };
    private Vector2 textLeft = new Vector2(-77.5f, -4.2f);

    private Dictionary<int, string[]> ballSkinDictionary = new Dictionary<int, string[]> {
        {15, new string[]{ "x/5", "ballGer:24-48-72:#000000-#FF2014-#FFD800" } },
        {0, new string[]{ "0", "#FFFFFF"} },
        {1, new string[]{ "100", "#71ABDD"} },
        {2, new string[]{ "100", "#6BCADE"} },
        {3, new string[]{ "100", "#82CCB5"} },
        {4, new string[]{ "100", "#B6D884"} },
        {5, new string[]{ "100", "#FFF68F"} },
        {6, new string[]{ "100", "#FDCD79"} },
        {7, new string[]{ "100", "#F497AA"} },
        {8, new string[]{ "100", "#DD86B9"} },
        {9, new string[]{ "100", "#9977B4"} },
        {10, new string[]{ "200", "ballGer:24-48-72:#000000-#FF2014-#FFD800" } },
    };

    private Dictionary<int, string[]> trailSkinDictionary = new Dictionary<int, string[]> {
        {0, new string[]{ "0", "#FFFFFF"} },
        {1, new string[]{ "100", "#71ABDD"} },
        {2, new string[]{ "100", "#6BCADE"} },
        {3, new string[]{ "100", "#82CCB5"} },
        {4, new string[]{ "100", "#B6D884"} },
        {5, new string[]{ "100", "#FFF68F"} },
        {6, new string[]{ "100", "#FDCD79"} },
        {7, new string[]{ "100", "#F497AA"} },
        {8, new string[]{ "100", "#DD86B9"} },
        {9, new string[]{ "100", "#9977B4"} },
        {10, new string[]{ "#'DOUBLE DOWN'", "gem:#00D272:#009C54" } },
    };

    public GameObject[] gemButtons;

    void Start()
    {
        amount.text = PlayerPrefs.GetInt("totalGems").ToString();
        if (!PlayerPrefs.HasKey("ballSkin0"))
        {
            PlayerPrefs.SetInt("ballSkin0", 1);
            PlayerPrefs.SetInt("trailSkin0", 1);
            PlayerPrefs.SetInt("lastBallSkin", 0);
            PlayerPrefs.SetInt("lastTrailSkin", 0);
        }
        initiateBallShop();
        initiateTrailShop();
        initiateGemsShop();
        setLastSelectedSkins();
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
                if (currentElement[0].StartsWith("x"))
                {
                    temp.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
                    textMesh.gameObject.transform.localPosition = textLeft;
                    textMesh.color = Color.white;
                    textMesh.text = currentElement[0];
                    temp.transform.GetChild(4).gameObject.SetActive(true);
                    temp.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "WATCH ADS";
                }
                else if (currentElement[0].StartsWith("#"))
                {
                    temp.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
                    temp.transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
                    textMesh.text = "ACHIEVE";
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
                Color color;
                ColorUtility.TryParseHtmlString(currentElement[1], out color);
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
                if (currentElement[0].StartsWith("x"))
                {
                    temp.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
                    textMesh.gameObject.transform.localPosition = textLeft;
                    textMesh.color = Color.white;
                    textMesh.text = currentElement[0];
                    temp.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "WATCH ADS";
                }
                else if (currentElement[0].StartsWith("#"))
                {
                    temp.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
                    temp.transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
                    textMesh.text = "ACHIEVE";
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
                Color color;
                ColorUtility.TryParseHtmlString(currentElement[1], out color);
                previewImage.color = color;
            }
            else
            {
                Color previewColor1;
                ColorUtility.TryParseHtmlString(currentElement[1].Split(splitColon)[1], out previewColor1);
                float pc1h, pc1s, pc1v;
                Color.RGBToHSV(previewColor1, out pc1h, out pc1s, out pc1v);

                Color previewColor2;
                ColorUtility.TryParseHtmlString(currentElement[1].Split(splitColon)[2], out previewColor2);
                float pc2h, pc2s, pc2v;
                Color.RGBToHSV(previewColor2, out pc2h, out pc2s, out pc2v);

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
            int gemAmount = int.Parse(temp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Replace(" Gems", ""));
            temp.GetComponent<Button>().onClick.AddListener(() => increaseGems(gemAmount));
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
        string[] values;
        ballSkinDictionary.TryGetValue(number, out values);
        string colorOrSkin = values[1];
        if (button.transform.GetChild(2).gameObject.activeSelf)
        {
            string text = button.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text;
            if (text.StartsWith("A"))
            {

            }
            else
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
                    Debug.Log("Not enough moneten");
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
            Color color;
            if (ColorUtility.TryParseHtmlString(colorOrSkin, out color))
            {
                uiBall.GetComponent<Image>().sprite = Resources.Load<Sprite>("standardBall");
                uiBall.GetComponent<Image>().color = color;
                playBall.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("standardBall");
                playBall.GetComponent<SpriteRenderer>().color = color;
                Material tempMat = new Material(playBall.GetComponent<SpriteRenderer>().material);
                tempMat.color = color;
                foreach (GameObject frag in playBall.GetComponent<Explodable>().fragments)
                {
                    frag.GetComponent<MeshRenderer>().material = tempMat;
                }
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

            Debug.Log(colors.Length);
            GameObject[] fragments = playBall.GetComponent<Explodable>().fragments.OrderBy(x => Guid.NewGuid()).ToList().ToArray();
            Debug.Log(fragments.Length);

            int colorAmountStart = 0;
            int colorAmountEnd;
            for (int i = 0; i < colorAmounts.Length; i++)
            {
                Color color;
                ColorUtility.TryParseHtmlString(colors[i], out color);
                Debug.Log("color: " + i + " " + color);
                Material tempMat = new Material(playBall.GetComponent<SpriteRenderer>().material);
                tempMat.color = color;
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
        string[] values;
        trailSkinDictionary.TryGetValue(number, out values);
        string colorOrSkin = values[1];
        if (button.transform.GetChild(2).gameObject.activeSelf)
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
                Debug.Log("Not enough moneten");
                return;
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
            Color color;
            if (ColorUtility.TryParseHtmlString(colorOrSkin, out color))
            {
                playBall.GetComponent<TrailRenderer>().startColor = color;
                playBall.GetComponent<TrailRenderer>().endColor = new Color(color.r, color.g, color.b, 0f);
                playBall.GetComponent<TrailRenderer>().enabled = true;
                em.enabled = false;
            }
        }
        else
        {
            playBall.GetComponent<TrailRenderer>().enabled = false;
            string[] split = colorOrSkin.Split(splitColon);
            Color color1;
            Color color2;
            ColorUtility.TryParseHtmlString(split[1], out color1);
            ColorUtility.TryParseHtmlString(split[2], out color2);
            ParticleSystem.MinMaxGradient grad = new ParticleSystem.MinMaxGradient(color1, color2)
            {
                mode = ParticleSystemGradientMode.TwoColors
            };
            mm.startColor = grad;
            ps.textureSheetAnimation.SetSprite(0, Resources.Load<Sprite>(split[0]));
            em.enabled = true;
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
        string[] valuesBall;
        ballSkinDictionary.TryGetValue(lastBallSkin, out valuesBall);
        setBallColor(valuesBall[1]);

        selectedTrail = trailSkinScrollViewContent.transform.GetChild(Array.IndexOf(trailSkinDictionary.Keys.ToArray(), lastTrailSkin)).gameObject;
        selectedTrail.transform.GetChild(1).gameObject.SetActive(true);
        string[] valuesTrail;
        trailSkinDictionary.TryGetValue(lastTrailSkin, out valuesTrail);
        setTrailColor(valuesTrail[1]);
    }
}
