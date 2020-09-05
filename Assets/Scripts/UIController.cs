using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private readonly string[] badComments = { "Yeah, not good..", "Oof...", "You can do better..", "Really? Embarrassing...", "My grandma got farther!", "Did you even try?", "No comment.", "Poor performance.", "Didn't even see the ball moving.", "You might beat me in 1000 years.", "Unacceptable", "Keep dreaming.", "Come on buddy..", "Is that all you got?" };
    private readonly string[] mediumComments = { "Not bad. But not good.", "Mediocre.", "Almost good...", "We're getting better.", "Now you're trying!", "Meh..", "Try again", "Practice makes perfect.", "Concentrate!", "I think the ball moved a little bit!" };
    private readonly string[] goodComments = { "Pretty good!", "Nice.", "I think you beat my highscore!", "Wonderful!", "Impressive!", "Looking good!", "Practice did make you perfect!", "HOMERUN!", "In German you'd say: 'Stabil!'" };
    private readonly string[] insanceComments = { "NANI?!", "IT'S OVER 9000!...almost", "Holy moly!", "Is this on easy mode?", "I can't even count that high!", "I think it's time for a break...", "Screenshot that!", "Become a Swish-Streamer!" };

    private readonly char[] splitMinus = { '-' };
    private readonly char[] splitStar = { '*' };
    private readonly char[] splitColon = { ':' };

    /* 
     * If you want to add a new achievement, just add an entry in the dictionary with a new key. Keep the keys in perfect ascending order. 
     * You can add new entries anywhere in the dictionary, not just append at the end.
     */
    private readonly Dictionary<int, string> achievementDictionary = new Dictionary<int, string> {
        {0, "Basic Skills-Reach a score of 200 in normal mode.-50"},
        {1, "Extreme-Reach a score of 1200 in normal mode.-120"},
        {2, "Grand Master-Reach a score of 2000 in normal or extreme mode.-200"},
        {3, "Hacker-Reach a score of 5000 in extreme mode.-500"},
        {4, "Paul-Reach a score of 10000 in extreme mode.-1000"},
        {5, "Customizer-Change your ball or trail skin.-50"},
        {6, "Stars-Rate the game on the app store.-trail*Stars"},
        {7, "Double Down-Double your earned gems in the game over panel.-trail*Gems"},
        {8, "Reverse-Touch the back wall.-25"},
        {9, "???-???-skin*???"},
        {10, "Zero velocity-Stop the ball.-100"},
        {11, "Adblocker-Remove ads. Thank you :)-skin*Adblocker"},
    };

    private readonly Vector2[,] ballPreviewLinePositions = new Vector2[5, 4]
    {
        {new Vector2(0,150), new Vector2(0,-150), Vector2.zero, Vector2.zero },
        {new Vector2(-150,0), new Vector2(150,0), Vector2.zero, Vector2.zero },
        {new Vector2(0,150), new Vector2(150,-150), new Vector2(-150,-150), Vector2.zero },
        {new Vector2(-100,100), new Vector2(100,100), new Vector2(100,-100), new Vector2(-100,-100) },
        {new Vector2(0,150), new Vector2(150,0), new Vector2(0,-150), new Vector2(-150,0) }
    };

    private readonly int[,] ballPreviewLineRotations = new int[5, 4]
    {
        {90,90,0,0},
        {0,0,0,0},
        {90,315,45,0},
        {315,45,315,45},
        {90,0,90,0}
    };

    private int difficulty;
    private int previewNr;
    private bool random3Flip;
    private float scoreFloat = 0f;
    private Camera mainCamera;
    private Rigidbody2D ballRb;
    private Transform ballTransform;
    private FollowBall followBall;
    private Vector3 ballStartPos = new Vector3(-5, 0, 0);
    private Vector3 ballStartPos2 = new Vector3(-25, 0, 0);
    private Vector3 cameraStartPos1 = new Vector3(0, 0, -10);
    private Vector3 cameraStartPos2 = new Vector3(-20, 0, -10);
    private Vector2 middlePanelUp = new Vector2(0, -35);
    private Vector2 middlePanelDown = new Vector2(0, 35);
    private Color gray = new Color(0.362f, 0.362f, 0.362f);
    private Color red = new Color(0.9882353f, 0.05490196f, 0.3411765f);
    private Color green = new Color(0f, 0.8235294f, 0.4485295f);
    private Color blue = new Color(0.02352941f, 0.5607843f, 0.7372549f);
    private Coroutine currentUpdateGemsCoroutine;
    public Animator ballAnimator;
    public Animator scoreAnimator;
    public Animator titlePanelAnimator;
    public Animator pauseButtonAnimator;
    public Animator ballPreviewAnimator;
    public Animator middlePanelAnimator;
    public Animator startSelectPanelAnimator;
    public Animator difficultySelectAnimator;
    public Animator difficultySelectIngameAnimator;
    public GameObject uiBall;
    public GameObject ball;
    public GameObject wallBack;
    public GameObject resetObstacle;
    public GameObject gameOverPanel;
    public GameObject middlePanel;
    public GameObject backWall;
    public GameObject pauseButton;
    public GameObject pausePanel;
    public TextMeshProUGUI gameOverPanelComment;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI highScore;
    public TextMeshProUGUI highScoreDiff;
    public TextMeshProUGUI score;
    public TextMeshProUGUI gameOverScore;
    public TextMeshProUGUI totalGemsText;
    public TextMeshProUGUI earnedGems;
    public TextMeshProUGUI extremeButtonInfo;
    public TextMeshProUGUI extremeButtonInfoLocked;
    public TextMeshProUGUI extremeButtonIngameInfo;
    public TextMeshProUGUI extremeButtonIngameInfoLocked;
    public Button customizeButton;
    public Button playButton;
    public Button moreButton;
    public Button moreButtonAdsRemoved;
    public Button noAdsButton;
    public Button extremeButton;
    public Button extremeButtonIngame;
    public Button questionMarkButton;
    public Button pausePanelHomeButton;
    public Button pausePanelRestartButton;
    public BallController ballController;
    public AdController adController;
    public TimeController timeController;
    public ShopController shopController;
    public TouchController touchController;
    public AudioSource backgroundSource;
    public GameObject achievementButtonPrefab;
    public ScrollRect achievementScrollView;
    public GameObject achievementScrollViewContent;
    public GameObject backToHomeAchievementInfo;
    public GameObject moreAchievementInfo;
    public GameObject moreAdsRemovedAchievementInfo;
    public GameObject achievementsAchievementInfo;
    public GameObject highScoreSignE;
    public GameObject highScoreSignN;
    public GameObject highScoreSignX;
    public GameObject[] helper;
    public GameObject[] helperLines;
    public GameObject[] ballPreviewLines;
    public AnimationClip[] ballPreviewAnimations;

    private void Start()
    {
        previewNr = Random.Range(0, 5);
        Application.targetFrameRate = 60;
        Time.fixedDeltaTime = (1f / Application.targetFrameRate);
        Time.maximumDeltaTime = (1.5f / Application.targetFrameRate);
        mainCamera = Camera.main;
        difficulty = 1;
        ballController.setBall(ball);
        followBall = mainCamera.GetComponent<FollowBall>();
        ballTransform = ball.transform;
        ballRb = ball.GetComponent<Rigidbody2D>();

        if (PlayerPrefs.GetInt("highScoreN") >= 1200)
        {
            extremeButton.interactable = true;
            extremeButtonInfo.gameObject.SetActive(true);
            extremeButtonInfoLocked.gameObject.SetActive(false);
            extremeButtonIngame.interactable = true;
            extremeButtonIngameInfo.gameObject.SetActive(true);
            extremeButtonIngameInfoLocked.gameObject.SetActive(false);
        }

        if (!PlayerPrefs.HasKey("lastDifficulty"))
        {
            PlayerPrefs.SetInt("lastDifficulty", 1);
        }
        highScoreSignE.transform.position = new Vector2(PlayerPrefs.GetFloat("highScoreEPosX"), -3.3f);
        highScoreSignN.transform.position = new Vector2(PlayerPrefs.GetFloat("highScoreNPosX"), -3.3f);
        highScoreSignX.transform.position = new Vector2(PlayerPrefs.GetFloat("highScoreXPosX"), -3.3f);

        switch (PlayerPrefs.GetInt("lastDifficulty"))
        {
            case 0:
                highScore.text = PlayerPrefs.GetInt("highScoreE").ToString();
                score.color = highScoreDiff.color = highScoreText.color = highScore.color = blue;
                highScoreDiff.text = "EASY";
                break;
            case 1:
                highScore.text = PlayerPrefs.GetInt("highScoreN").ToString();
                score.color = highScoreDiff.color = highScoreText.color = highScore.color = green;
                highScoreDiff.text = "NORMAL";
                break;
            case 2:
                highScore.text = PlayerPrefs.GetInt("highScoreX").ToString();
                score.color = highScoreDiff.color = highScoreText.color = highScore.color = red;
                highScoreDiff.text = "EXTREME";
                break;
        }
        if (PlayerPrefs.GetInt("achtakeable9") == 1 || PlayerPrefs.GetInt("ach9") == 1)
        {
            questionMarkButton.interactable = false;
        }
        intitializeAchievements();
        setRandomBallPreview();
        totalGemsText.text = PlayerPrefs.GetInt("totalGems").ToString();
    }

    void FixedUpdate()
    {
        if (timeController.isGameStarted())
        {
            scoreFloat = (ballTransform.position.x + 5) * 5;
            int intScore = int.Parse(score.text);
            if (intScore < scoreFloat)
            {
                score.text = ((int)scoreFloat).ToString();
                gameOverScore.text = score.text;
            }
            if (difficulty == 1 && ballRb.velocity.magnitude <= 7.5f && !timeController.isGamePaused())
            {
                ballRb.AddForce(ballRb.velocity.normalized * 0.05f);
            }
            if (ball.activeSelf && ballRb.velocity.magnitude < 3f && ball.transform.position.x != -5f && ball.transform.position.x > -11f)
            {
                resetObstacle.SetActive(true);
                resetObstacle.tag = "destroyBall";
                ball.GetComponent<BallCollisionDetection>().OnTriggerEnter2D(resetObstacle.GetComponent<Collider2D>());
                resetObstacle.tag = "reset";
                gameOverPanelComment.text = "Why would you want the ball to get stuck?";
                resetObstacle.SetActive(false);
                if (PlayerPrefs.GetInt("ach10") == 0)
                {
                    PlayerPrefs.SetInt("achtakeable10", 1);
                    activateAchievementInfo(10);
                }
            }
        }
    }

    public void startGame()
    {
        backWall.transform.position = new Vector3(-13f, 0f, 0f);
        mainCamera.transform.position = new Vector3(0f, 0f, -10f);
        score.text = "0";
        earnedGems.text = "0";
        ball.transform.position = ballStartPos;
        score.gameObject.SetActive(true);
        scoreAnimator.SetBool("fadeIn", true);
        difficultySelectAnimator.SetBool("fadeIn", false);
        difficultySelectAnimator.SetBool("fade", true);
        middlePanelAnimator.SetBool("fadeIn", false);
        middlePanelAnimator.SetBool("fade", true);
        titlePanelAnimator.SetBool("fadeIn", false);
        titlePanelAnimator.SetBool("fade", true);
        pauseButtonAnimator.gameObject.SetActive(true);
        pauseButtonAnimator.SetBool("fadeIn", true);
        ballAnimator.SetBool("moveToStart", true);

        backgroundSource.Play();
        timeController.setGameOver(false);
        timeController.setGameStarted(true);
        followBall.setFollowActive(true);
        followBall.setFirstSpawn();


        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("PlayerFragment"))
        {
            obj.transform.parent = ball.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.SetActive(false);
        }
    }

    public void shootBall()
    {
        ball.SetActive(true);
        if (difficulty == 2)
        {
            ball.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 0) * 450 * ballRb.mass);
        }
        else
        {
            ball.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 0) * 200 * ballRb.mass);
        }
    }

    public void stopGame()
    {
        backWall.transform.position = new Vector3(-13f, 0f, 0f);
        timeController.endSlowdown();
        timeController.setGamePaused(false);
        followBall.setFollowActive(false);
        ball.SetActive(false);
        backToHomeScreen();
        adController.setDoubleTaken(false);
        StartCoroutine(adController.LoadRewardedBanner());
    }

    public void backToHomeScreen()
    {
        backWall.transform.position = new Vector3(-13f, 0f, 0f);
        setRandomBallPreview();
        updateTotalGems();
        deactivateFragments();
        StartCoroutine(cameraToHomeScreenCoroutine());
        timeController.setGameStarted(false);
        mainCamera.GetComponent<AudioSource>().Stop();
        score.gameObject.SetActive(false);
        pauseButtonAnimator.gameObject.SetActive(false);
        difficultySelectAnimator.enabled = false;
        difficultySelectAnimator.gameObject.GetComponent<CanvasGroup>().alpha = 1;
        middlePanelAnimator.SetBool("fadeIn", true);
        titlePanelAnimator.SetBool("fadeIn", true);
        ballPreviewAnimator.SetBool("fadeIn", true);
        startSelectPanelAnimator.SetBool("fadeIn", true);
        highScoreSignE.SetActive(false);
        highScoreSignN.SetActive(false);
        highScoreSignX.SetActive(false);
    }

    public void updateTotalGems()
    {
        if (currentUpdateGemsCoroutine != null)
        {
            StopCoroutine(currentUpdateGemsCoroutine);
        }
        currentUpdateGemsCoroutine = StartCoroutine(updateTotalGemsCoroutine());
    }

    private IEnumerator updateTotalGemsCoroutine()
    {
        int playerPrefsGems = PlayerPrefs.GetInt("totalGems");
        if (int.Parse(totalGemsText.text) < playerPrefsGems)
        {
            while (int.Parse(totalGemsText.text) < playerPrefsGems)
            {
                totalGemsText.text = (int.Parse(totalGemsText.text) + 5).ToString();
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (int.Parse(totalGemsText.text) > playerPrefsGems)
            {
                totalGemsText.text = (int.Parse(totalGemsText.text) - 5).ToString();
                yield return new WaitForEndOfFrame();
            }
        }
        switch (int.Parse(totalGemsText.text) - playerPrefsGems)
        {
            case 4:
                totalGemsText.text = (int.Parse(totalGemsText.text) - 4).ToString();
                break;
            case 3:
                totalGemsText.text = (int.Parse(totalGemsText.text) - 3).ToString();
                break;
            case 2:
                totalGemsText.text = (int.Parse(totalGemsText.text) - 2).ToString();
                break;
            case 1:
                totalGemsText.text = (int.Parse(totalGemsText.text) - 1).ToString();
                break;
            case -1:
                totalGemsText.text = (int.Parse(totalGemsText.text) + 1).ToString();
                break;
            case -2:
                totalGemsText.text = (int.Parse(totalGemsText.text) + 2).ToString();
                break;
            case -3:
                totalGemsText.text = (int.Parse(totalGemsText.text) + 3).ToString();
                break;
            case -4:
                totalGemsText.text = (int.Parse(totalGemsText.text) + 4).ToString();
                break;
            default:
                break;
        }
        shopController.updateAmount();
        yield return null;
    }

    public void deactivateFragments()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("PlayerFragment"))
        {
            obj.transform.parent = ball.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.SetActive(false);
        }
    }

    public void deactivateWallsAndObstaclesAndGems()
    {
        foreach (GameObject wall in GameObject.FindGameObjectsWithTag("bounceOff"))
        {
            wall.SetActive(false);
        }

        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            obstacle.SetActive(false);
        }

        foreach (GameObject gem in GameObject.FindGameObjectsWithTag("collectibleGem"))
        {
            gem.SetActive(false);
        }
    }
    public void retry()
    {
        score.text = "0";
        earnedGems.text = "0";
        shopController.updateAmount();
        updateTotalGems();
        deactivateFragments();
        ball.transform.position = ballStartPos2;
        ball.SetActive(true);
        ball.GetComponent<TrailRenderer>().enabled = false;
        if (difficultySelectIngameAnimator.gameObject.activeSelf)
        {
            difficultySelectIngameAnimator.SetBool("fade", true);
        }
        StartCoroutine(cameraToStartCoroutine(false));
        adController.setDoubleTaken(false);
        StartCoroutine(adController.LoadRewardedBanner());
    }
    public IEnumerator waitAndRetry()
    {
        yield return new WaitForSeconds(0.5f);
        retry();
        timeController.setGamePaused(false);
        yield return null;
    }

    private IEnumerator cameraToStartCoroutine(bool stopgame)
    {
        wallBack.SetActive(false);
        while (mainCamera.transform.position.x > -19.98f)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraStartPos2, Time.deltaTime * 15f);
            yield return new WaitForFixedUpdate();
        }
        mainCamera.transform.position = cameraStartPos1;
        ball.transform.position = ballStartPos2;
        ball.GetComponent<TrailRenderer>().SetPositions(new Vector3[] { ball.transform.position });
        if (!stopgame)
        {
            startGame();
            shootBall();
        }
        deactivateWallsAndObstaclesAndGems();
        wallBack.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        if (!ball.GetComponent<ParticleSystem>().emission.enabled)
        {
            ball.GetComponent<TrailRenderer>().enabled = true;
        }
        yield return null;
    }

    public void resetGame()
    {
        timeController.stopSlowdown();
        resetObstacle.SetActive(true);
        ball.GetComponent<BallCollisionDetection>().OnTriggerEnter2D(resetObstacle.GetComponent<Collider2D>());
        resetObstacle.SetActive(false);
    }

    public void pauseGame()
    {
        if (!touchController.isLineDrawing())
        {
            timeController.setGamePaused(true);
            backgroundSource.Pause();
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            pauseButton.SetActive(false);
        }
    }

    public void resumeGame()
    {
        timeController.setGamePaused(false);
        backgroundSource.Play();
        Time.timeScale = 1f;
    }

    private IEnumerator cameraToHomeScreenCoroutine()
    {
        wallBack.SetActive(false);
        while (mainCamera.transform.position.x > -19.98f)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraStartPos2, Time.deltaTime * 15f);
            yield return new WaitForFixedUpdate();
        }
        mainCamera.transform.position = cameraStartPos1;
        deactivateWallsAndObstaclesAndGems();
        wallBack.SetActive(true);
        yield return null;
    }

    public int getCurrentScore()
    {
        return int.Parse(score.text);
    }

    public int getDifficulty()
    {
        return difficulty;
    }

    public void setDifficulty(int difficulty)
    {
        PlayerPrefs.SetInt("lastDifficulty", difficulty);
        this.difficulty = difficulty;
    }

    public void setGameOverPanelComment(int score, bool wallBack)
    {
        string randomText;
        if (score < 200)
        {
            randomText = badComments[Random.Range(0, badComments.Length)];
        }
        else if (score < 1200)
        {
            randomText = mediumComments[Random.Range(0, mediumComments.Length)];
        }
        else if (score < 5000)
        {
            randomText = goodComments[Random.Range(0, goodComments.Length)];
        }
        else if (score < 9000)
        {
            randomText = insanceComments[Random.Range(0, insanceComments.Length)];
        }
        else
        {
            if (gameOverPanelComment.text.Equals("IT'S OVER 9000!!") || gameOverPanelComment.text.Equals("IT'S OVER 9000!! Wait.. Again?"))
            {
                randomText = "IT'S OVER 9000!! Wait.. Again?";
            }
            else
            {
                randomText = "IT'S OVER 9000!!";
            }
        }

        if (wallBack)
        {
            if (!gameOverPanelComment.text.Equals("That's the wrong direction...") && !gameOverPanelComment.text.Equals("Still the wrong direction."))
            {
                randomText = "That's the wrong direction...";
            }
            else
            {
                randomText = "Still the wrong direction.";
            }
        }

        if (randomText.Equals(gameOverPanelComment.text) && score < 9000 && !wallBack)
        {
            setGameOverPanelComment(score, false);
        }
        else
        {
            gameOverPanelComment.text = randomText;
        }
    }

    public void UpdateExtremeLock()
    {
        if (PlayerPrefs.GetInt("highScoreN") >= 1200)
        {
            extremeButton.interactable = true;
            extremeButtonInfo.gameObject.SetActive(true);
            extremeButtonInfoLocked.gameObject.SetActive(false);
            extremeButtonIngame.interactable = true;
            extremeButtonIngameInfo.gameObject.SetActive(true);
            extremeButtonIngameInfoLocked.gameObject.SetActive(false);
        }
    }

    public int getTotalGems()
    {
        return int.Parse(totalGemsText.text);
    }

    public TextMeshProUGUI getTotalGemsText()
    {
        return totalGemsText;
    }

    public void setEarnedGems(int amount)
    {
        earnedGems.text = amount.ToString();
    }

    public void exitGame()
    {
        Application.Quit();
    }

    public void openPrivacyPolicy()
    {
        Application.OpenURL("https://unity3d.com/legal/privacy-policy");
    }

    public void rateGame()
    {
        StartCoroutine(rateGameCoroutine());
    }

    private IEnumerator rateGameCoroutine()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=com.HovGames.Swish");
#elif UNITY_IPHONE
            Application.OpenURL("itms-apps://itunes.apple.com/app/idcom.HovGames.Swish");
#endif
        yield return new WaitForSeconds(2f);
        if (PlayerPrefs.GetInt("ach6") == 0)
        {
            PlayerPrefs.SetInt("achtakeable6", 1);
            PlayerPrefs.SetInt("trailSkin9", 1);
            activateAchievementInfo(6);
        }
    }

    private void intitializeAchievements()
    {
        for (int i = 0; i < achievementDictionary.Count; i++)
        {
            if (PlayerPrefs.GetInt("achtakeable" + i) == 1)
            {
                moreAchievementInfo.SetActive(true); ;
                achievementsAchievementInfo.SetActive(true);
            }
        }

        foreach (string achievementText in achievementDictionary.Values)
        {
            string[] components = achievementText.Split(splitMinus);
            string rewardString;
            if (components[2].StartsWith("skin"))
            {
                rewardString = "'" + components[2].Split(splitStar)[1] + "'-Skin";
            }
            else if (components[2].StartsWith("trail"))
            {
                rewardString = "'" + components[2].Split(splitStar)[1] + "'-Trail";
            }
            else
            {
                rewardString = components[2] + " Gems";
            }
            GameObject achievementButton = Instantiate(achievementButtonPrefab);
            Transform achtransform = achievementButton.transform;
            int achKey = achievementDictionary.FirstOrDefault(x => x.Value.Equals(achievementText)).Key;
            achtransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = components[0];
            achtransform.GetChild(1).GetComponent<TextMeshProUGUI>().text = components[1];
            achtransform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Reward: " + rewardString;

            if (PlayerPrefs.GetInt("ach" + achievementDictionary.FirstOrDefault(x => x.Value.Equals(achievementText)).Key) == 1)
            {
                achtransform.GetChild(3).gameObject.SetActive(true);
                achtransform.GetChild(2).GetComponent<TextMeshProUGUI>().color = gray;
            }
            if (PlayerPrefs.GetInt("achtakeable" + achKey) == 1)
            {
                achtransform.GetChild(4).gameObject.SetActive(true);
            }

            achievementButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (achtransform.GetChild(4).gameObject.activeSelf)
                {
                    PlayerPrefs.SetInt("ach" + achKey, 1);
                    PlayerPrefs.SetInt("achtakeable" + achKey, 0);
                    achtransform.GetChild(3).gameObject.SetActive(true);
                    achtransform.GetChild(4).gameObject.SetActive(false);
                    string reward = achtransform.GetChild(2).GetComponent<TextMeshProUGUI>().text.Split(splitColon)[1].Trim();
                    achtransform.GetChild(2).GetComponent<TextMeshProUGUI>().color = gray;
                    if (reward.EndsWith("Skin") || reward.EndsWith("Trail"))
                    {
                        shopController.updateShopButtons();
                    }
                    else
                    {
                        PlayerPrefs.SetInt("totalGems", PlayerPrefs.GetInt("totalGems") + int.Parse(reward.Replace(" Gems", "")));
                        updateTotalGems();
                        shopController.updateAmount();
                    }
                }
            });
            achievementButton.transform.SetParent(achievementScrollViewContent.transform, false);
        }
        GameObject moreToCome = Instantiate(achievementButtonPrefab);
        moreToCome.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "More to come...";
        moreToCome.transform.SetParent(achievementScrollViewContent.transform, false);
        achievementScrollView.verticalNormalizedPosition = 1;
    }

    public void activateAchievementInfo(int achnr)
    {
        backToHomeAchievementInfo.SetActive(true);
        moreAchievementInfo.SetActive(true);
        moreAdsRemovedAchievementInfo.SetActive(true);
        achievementsAchievementInfo.SetActive(true);
        achievementScrollViewContent.transform.GetChild(achnr).transform.GetChild(3).gameObject.SetActive(true);
        achievementScrollViewContent.transform.GetChild(achnr).transform.GetChild(4).gameObject.SetActive(true);
    }

    public void unlockDoubleDownAchievement()
    {
        if (PlayerPrefs.GetInt("ach7") == 0)
        {
            PlayerPrefs.SetInt("achtakeable7", 1);
            PlayerPrefs.SetInt("trailSkin8", 1);
            activateAchievementInfo(7);
        }
    }

    public void unlockQuestionMarkAchievement()
    {
        if (PlayerPrefs.GetInt("ach9") == 0)
        {
            PlayerPrefs.SetInt("achtakeable9", 1);
            PlayerPrefs.SetInt("ballSkin26", 1);
            activateAchievementInfo(9);
        }
    }

    public int getHighScore()
    {
        return int.Parse(highScore.text);
    }

    public void activateHighScoreSign()
    {
        switch (difficulty)
        {
            case 0:
                if (PlayerPrefs.GetInt("highScoreE") > 0)
                {
                    highScoreSignE.SetActive(true);
                }
                highScoreSignN.SetActive(false);
                highScoreSignX.SetActive(false);
                score.color = highScoreDiff.color = highScoreText.color = highScore.color = blue;
                highScoreDiff.text = "EASY";
                break;
            case 1:
                if (PlayerPrefs.GetInt("highScoreN") > 0)
                {
                    highScoreSignN.SetActive(true);
                }
                highScoreSignX.SetActive(false);
                highScoreSignE.SetActive(false);
                score.color = highScoreDiff.color = highScoreText.color = highScore.color = green;
                highScoreDiff.text = "NORMAL";
                break;
            case 2:
                if (PlayerPrefs.GetInt("highScoreX") > 0)
                {
                    highScoreSignX.SetActive(true);
                }
                highScoreSignE.SetActive(false);
                highScoreSignN.SetActive(false);
                score.color = highScoreDiff.color = highScoreText.color = highScore.color = red;
                highScoreDiff.text = "EXTREME";
                break;
        }
    }

    public void CycleHighScores(TextMeshProUGUI diff)
    {
        switch (diff.text)
        {
            case "EASY":
                highScore.text = PlayerPrefs.GetInt("highScoreN").ToString();
                highScoreDiff.color = highScoreText.color = highScore.color = green;
                highScoreDiff.text = "NORMAL";
                break;
            case "NORMAL":
                highScore.text = PlayerPrefs.GetInt("highScoreX").ToString();
                highScoreDiff.color = highScoreText.color = highScore.color = red;
                highScoreDiff.text = "EXTREME";
                break;
            case "EXTREME":
                highScore.text = PlayerPrefs.GetInt("highScoreE").ToString();
                highScoreDiff.color = highScoreText.color = highScore.color = blue;
                highScoreDiff.text = "EASY";
                break;
        }
    }

    private void CycleHighScoresInt(int diff)
    {
        switch (diff)
        {
            case 0:
                highScore.text = PlayerPrefs.GetInt("highScoreN").ToString();
                highScoreDiff.color = highScoreText.color = highScore.color = green;
                highScoreDiff.text = "NORMAL";
                break;
            case 1:
                highScore.text = PlayerPrefs.GetInt("highScoreX").ToString();
                highScoreDiff.color = highScoreText.color = highScore.color = red;
                highScoreDiff.text = "EXTREME";
                break;
            case 2:
                highScore.text = PlayerPrefs.GetInt("highScoreE").ToString();
                highScoreDiff.color = highScoreText.color = highScore.color = blue;
                highScoreDiff.text = "EASY";
                break;
        }
    }

    public void setHighscoreSign(float ballXpos)
    {
        switch (difficulty)
        {
            case 0:
                PlayerPrefs.SetFloat("highScoreEPosX", ballXpos);
                highScore.text = score.text;
                highScoreSignE.transform.position = new Vector2(ballXpos, -3.3f);
                highScoreSignE.SetActive(true);
                break;
            case 1:
                PlayerPrefs.SetFloat("highScoreNPosX", ballXpos);
                highScore.text = score.text;
                highScoreSignN.transform.position = new Vector2(ballXpos, -3.3f);
                highScoreSignN.SetActive(true);
                break;
            case 2:
                PlayerPrefs.SetFloat("highScoreXPosX", ballXpos);
                highScore.text = score.text;
                highScoreSignX.transform.position = new Vector2(ballXpos, -3.3f);
                highScoreSignX.SetActive(true);
                break;
        }
    }

    public void setRandomBallPreview()
    {
        if (previewNr == 4)
        {
            previewNr = 0;
        }
        else
        {
            previewNr++;
        }

        switch (previewNr)
        {
            case 0:
                uiBall.GetComponent<Animator>().SetInteger("previewNr", 1);
                break;
            case 1:
                uiBall.GetComponent<Animator>().SetInteger("previewNr", 2);
                break;
            case 2:
                random3Flip = !random3Flip;
                if (random3Flip)
                {
                    uiBall.GetComponent<Animator>().SetInteger("previewNr", 3);
                    middlePanel.transform.localPosition = middlePanelDown;
                    middlePanel.transform.localRotation = Quaternion.identity;
                }
                else
                {
                    uiBall.GetComponent<Animator>().SetInteger("previewNr", 4);
                    middlePanel.transform.localPosition = middlePanelUp;
                    middlePanel.transform.localRotation = Quaternion.Euler(0, 0, 180);
                }
                break;
            case 3:
                uiBall.GetComponent<Animator>().SetInteger("previewNr", 5);
                break;
            case 4:
                uiBall.GetComponent<Animator>().SetInteger("previewNr", 6);
                break;
            default:
                break;
        }

        if (previewNr != 2)
        {
            middlePanel.transform.localPosition = Vector2.zero;
            middlePanel.transform.localRotation = Quaternion.identity;
        }

        for (int i = 0; i < ballPreviewLines.Length; i++)
        {
            if (ballPreviewLinePositions[previewNr, i] == Vector2.zero)
            {
                ballPreviewLines[i].SetActive(false);
            }
            else
            {
                ballPreviewLines[i].SetActive(true);
                ballPreviewLines[i].transform.localPosition = ballPreviewLinePositions[previewNr, i];
                ballPreviewLines[i].transform.localRotation = Quaternion.Euler(0, 0, ballPreviewLineRotations[previewNr, i]);
            }
        }
    }
}
