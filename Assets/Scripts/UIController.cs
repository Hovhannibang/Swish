using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private string[] badComments = { "Yeah, not good..", "Oof...", "You can do better..", "Really? Embarrassing...", "My grandma got farther!", "Did you even try?", "No comment.", "Poor performance.", "Didn't even see the ball moving.", "You might beat me in 1000 years.", "Unacceptable", "Keep dreaming.", "Come on buddy..", "Is that all you got?" };
    private string[] mediumComments = { "Not bad. But not good.", "Mediocre.", "Almost good...", "We're getting better.", "Now you're trying!", "Meh..", "Try again", "Practice makes perfect.", "Concentrate!", "I think the ball moved a little bit!" };
    private string[] goodComments = { "Pretty good!", "Nice.", "I think you beat my highscore!", "Wonderful!", "Impressive!", "Looking good!", "Practice did make you perfect!", "HOMERUN!", "In German you'd say: 'Stabil!'" };
    private string[] insanceComments = { "NANI?!", "IT'S OVER 9000!...almost", "Holy moly!", "Is this on easy mode?", "I can't even count that high!", "I think it's time for a break...", "Screenshot that!", "Become a Swish-Streamer!" };

    private char[] splitMinus = { '-' };
    private char[] splitStar = { '*' };
    private char[] splitColon = { ':' };

    /* 
     * If you want to add a new achievement, just add an entry in the dictionary with a new key. Keep the keys in perfect ascending order. 
     * You can add new entries anywhere in the dictionary, not just append at the end.
     */
    private Dictionary<int, string> achievementDictionary = new Dictionary<int, string> {
        {0, "Tutorial Master-Finish the tutorial.-25"},
        {1, "Basic Skills-Reach a score of 200 in normal mode.-50"},
        {2, "Extreme-Reach a score of 1200 in normal mode.-120"},
        {3, "Grand Master-Reach a score of 2000 in normal or extreme mode.-200"},
        {4, "Hacker-Reach a score of 5000 in extreme mode.-500"},
        {5, "Beat Paul-Reach a score of 10000 in extreme mode.-1000"},
        {6, "Customizer-Change your ball or trail skin.-50"},
        {7, "Stars-Rate the game on the app store.-trail*Stars"},
        {8, "Double Down-Double your earned gems in the game over panel.-trail*Gems"},
        {9, "Reverse-Touch the back wall.-25"},
        {10, "???-???-skin*???"},
        {11, "Zero velocity-Stop the ball.-100"},
    };

    private Vector2[,] ballPreviewLinePositions = new Vector2[5, 4]
    {
        {new Vector2(0,150), new Vector2(0,-150), Vector2.zero, Vector2.zero },
        {new Vector2(-150,0), new Vector2(150,0), Vector2.zero, Vector2.zero },
        {new Vector2(0,150), new Vector2(150,-150), new Vector2(-150,-150), Vector2.zero },
        {new Vector2(-100,100), new Vector2(100,100), new Vector2(100,-100), new Vector2(-100,-100) },
        {new Vector2(0,150), new Vector2(150,0), new Vector2(0,-150), new Vector2(-150,0) }
    };

    private int[,] ballPreviewLineRotations = new int[5, 4]
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
    private Vector2 screenBounds;
    private Color gray;
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
    public TextMeshProUGUI gameOverPanelComment;
    public TextMeshProUGUI highScore;
    public TextMeshProUGUI score;
    public TextMeshProUGUI gameOverScore;
    public TextMeshProUGUI totalGems;
    public TextMeshProUGUI earnedGems;
    public TextMeshProUGUI extremeButtonInfo;
    public TextMeshProUGUI extremeButtonInfoLocked;
    public TextMeshProUGUI extremeButtonIngameInfo;
    public TextMeshProUGUI extremeButtonIngameInfoLocked;
    public Button extremeButton;
    public Button extremeButtonIngame;
    public BallController ballController;
    public AdController adController;
    public TimeController timeController;
    public ShopController shopController;
    public AudioSource backgroundSource;
    public GameObject achievementButtonPrefab;
    public ScrollRect achievementScrollView;
    public GameObject achievementScrollViewContent;
    public GameObject backToHomeAchievementInfo;
    public GameObject moreAchievementInfo;
    public GameObject achievementsAchievementInfo;
    public GameObject highScoreSign;
    public GameObject[] ballPreviewLines;
    public AnimationClip[] ballPreviewAnimations;

    private void Start()
    {
        previewNr = Random.Range(0, 5);
        gray = new Color(0.362f, 0.362f, 0.362f);
        Application.targetFrameRate = 60;
        Time.fixedDeltaTime = (1f / Application.targetFrameRate);
        Time.maximumDeltaTime = (1.5f / Application.targetFrameRate);
        mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, transform.position.z));
        difficulty = 1;
        ballController.setBall(ball);
        followBall = mainCamera.GetComponent<FollowBall>();
        ballTransform = ball.transform;
        ballRb = ball.GetComponent<Rigidbody2D>();

        if (!PlayerPrefs.HasKey("totalGems"))
        {
            PlayerPrefs.SetInt("totalGems", 0);
        }
        if (!PlayerPrefs.HasKey("highScore"))
        {
            PlayerPrefs.SetInt("highScore", 0);
            PlayerPrefs.SetFloat("highScorePosX", 0f);
        }
        if (PlayerPrefs.GetInt("highScore") >= 1200)
        {
            extremeButton.interactable = true;
            extremeButtonInfo.gameObject.SetActive(true);
            extremeButtonInfoLocked.gameObject.SetActive(false);
            extremeButtonIngame.interactable = true;
            extremeButtonIngameInfo.gameObject.SetActive(true);
            extremeButtonIngameInfoLocked.gameObject.SetActive(false);
        }
        highScoreSign.transform.position = new Vector2(PlayerPrefs.GetFloat("highScorePosX"), -3.3f);
        intitializeAchievements();
        setRandomBallPreview();
        highScore.text = PlayerPrefs.GetInt("highScore").ToString();
        totalGems.text = PlayerPrefs.GetInt("totalGems").ToString();
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
            }
            if (difficulty == 1 && ballRb.velocity.magnitude <= 7.5f && !timeController.isGamePaused())
            {
                ballRb.AddForce(ballRb.velocity.normalized * 0.05f);
            }
            if (ball.activeSelf && ballRb.velocity.magnitude < 3f && ball.transform.position.x >= -4.9f) // >=-4.9f ??????
            {
                resetObstacle.SetActive(true);
                resetObstacle.tag = "destroyBall";
                ball.GetComponent<BallCollisionDetection>().OnTriggerEnter2D(resetObstacle.GetComponent<Collider2D>());
                resetObstacle.tag = "reset";
                gameOverPanelComment.text = "Why would you want the ball to get stuck?";
                resetObstacle.SetActive(false);
                if (PlayerPrefs.GetInt("ach11") == 0)
                {
                    PlayerPrefs.SetInt("achtakeable11", 1);
                    activateAchievementInfo(11);
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
        pauseButtonAnimator.gameObject.SetActive(true);
        scoreAnimator.SetBool("fadeIn", true);
        pauseButtonAnimator.SetBool("fadeIn", true);
        ballAnimator.SetBool("moveToStart", true);
        titlePanelAnimator.SetBool("fadeIn", false);
        titlePanelAnimator.SetBool("fade", true);
        middlePanelAnimator.SetBool("fadeIn", false);
        middlePanelAnimator.SetBool("fade", true);
        difficultySelectAnimator.SetBool("fadeIn", false);
        difficultySelectAnimator.SetBool("fade", true);

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
    }

    public void updateTotalGems()
    {
        StartCoroutine(updateTotalGemsCoroutine());
    }

    private IEnumerator updateTotalGemsCoroutine()
    {
        int playerPrefsGems = PlayerPrefs.GetInt("totalGems");
        if (int.Parse(totalGems.text) < playerPrefsGems)
        {
            while (int.Parse(totalGems.text) < playerPrefsGems)
            {
                totalGems.text = (int.Parse(totalGems.text) + 5).ToString();
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (int.Parse(totalGems.text) > playerPrefsGems)
            {
                totalGems.text = (int.Parse(totalGems.text) - 5).ToString();
                yield return new WaitForEndOfFrame();
            }
        }
        switch (int.Parse(totalGems.text) - playerPrefsGems)
        {
            case 4:
                totalGems.text = (int.Parse(totalGems.text) - 4).ToString();
                break;
            case 3:
                totalGems.text = (int.Parse(totalGems.text) - 3).ToString();
                break;
            case 2:
                totalGems.text = (int.Parse(totalGems.text) - 2).ToString();
                break;
            case 1:
                totalGems.text = (int.Parse(totalGems.text) - 1).ToString();
                break;
            case -1:
                totalGems.text = (int.Parse(totalGems.text) + 1).ToString();
                break;
            case -2:
                totalGems.text = (int.Parse(totalGems.text) + 2).ToString();
                break;
            case -3:
                totalGems.text = (int.Parse(totalGems.text) + 3).ToString();
                break;
            case -4:
                totalGems.text = (int.Parse(totalGems.text) + 4).ToString();
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

    public void deactivateWallsAndObstacles()
    {
        foreach (GameObject wall in GameObject.FindGameObjectsWithTag("bounceOff"))
        {
            wall.SetActive(false);
        }

        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            obstacle.SetActive(false);
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
        deactivateWallsAndObstacles();
        wallBack.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        ball.GetComponent<TrailRenderer>().enabled = true;
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
        timeController.setGamePaused(true);
        backgroundSource.Pause();
        Time.timeScale = 0f;
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
        deactivateWallsAndObstacles();
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
        this.difficulty = difficulty;
    }

    public void setGameOverPanelComment(int score)
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

        if (ball.transform.position.x < -8f)
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

        if (randomText.Equals(gameOverPanelComment.text) && score < 9000 && ball.transform.position.x >= -8f)
        {
            setGameOverPanelComment(score);
        }
        else
        {
            gameOverPanelComment.text = randomText;
        }
    }

    public void UpdateExtremeLock()
    {
        if (PlayerPrefs.GetInt("highScore") >= 1200)
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
        return int.Parse(totalGems.text);
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
        if (PlayerPrefs.GetInt("ach7") == 0)
        {
            PlayerPrefs.SetInt("achtakeable7", 1);
            activateAchievementInfo(7);
        }
    }

    private void intitializeAchievements()
    {
        if (!PlayerPrefs.HasKey("ach0"))
        {
            for (int i = 0; i < achievementDictionary.Count; i++)
            {
                PlayerPrefs.SetInt("ach" + i, 0);
                PlayerPrefs.SetInt("achtakeable" + i, 0);
            }
        }
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
                        Debug.Log("unlocked " + reward.Split(splitMinus)[0] + " Skin");
                    }
                    else
                    {
                        PlayerPrefs.SetInt("totalGems", PlayerPrefs.GetInt("totalGems") + int.Parse(reward.Replace(" Gems", "")));
                        updateTotalGems();
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
        moreAchievementInfo.SetActive(true); ;
        achievementsAchievementInfo.SetActive(true);
        achievementScrollViewContent.transform.GetChild(achnr).transform.GetChild(3).gameObject.SetActive(true);
        achievementScrollViewContent.transform.GetChild(achnr).transform.GetChild(4).gameObject.SetActive(true);
    }

    public void unlockDoubleDownAchievement()
    {
        if (PlayerPrefs.GetInt("ach8") == 0)
        {
            PlayerPrefs.SetInt("achtakeable8", 1);
            activateAchievementInfo(8);
        }
    }

    public void unlockQuestionMarkAchievement()
    {
        if (PlayerPrefs.GetInt("ach10") == 0)
        {
            PlayerPrefs.SetInt("achtakeable10", 1);
            activateAchievementInfo(10);
        }
    }

    public int getHighScore()
    {
        return int.Parse(highScore.text);
    }

    public void activateHighScoreSign()
    {
        if (PlayerPrefs.GetInt("highScorePosX") > screenBounds.x + 1f)
        {
            highScoreSign.SetActive(true);
        }
    }

    public void setHighscoreSign(float ballXpos)
    {
        highScore.text = score.text;
        PlayerPrefs.SetFloat("highScorePosX", ballXpos);
        highScoreSign.transform.position = new Vector2(ballXpos, -3.3f);
        if (ballXpos > screenBounds.x + 1f)
        {
            highScoreSign.SetActive(true);
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
