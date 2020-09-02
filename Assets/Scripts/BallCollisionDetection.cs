using TMPro;
using UnityEngine;

public class BallCollisionDetection : MonoBehaviour
{
    public BallController ballController;
    private TouchController touchController;
    private AudioController audioController;
    private ShopController shopController;
    private TimeController timeController;
    private UIController uiController;
    private GameObject gameOverPanel;
    private TextMeshProUGUI score;
    private GameObject ball;
    private FollowBall fb;
    private bool isColliding;

    void Start()
    {
        ball = ballController.getBall();
        fb = ballController.getFollowball();
        uiController = ballController.getUIController();
        timeController = ballController.getTimeController();
        touchController = ballController.getTouchController();
        audioController = ballController.getAudioController();
        gameOverPanel = ballController.getGameOverPanel();
        shopController = ballController.getShopController();
        score = ballController.getScore();
    }

    private void Update()
    {
        isColliding = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (isColliding) return;
        isColliding = true;
        if (collision.gameObject.CompareTag("destroyBall") || collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("ObstacleWallBack"))
        {
            int intScore = int.Parse(score.text);
            if (collision.gameObject.CompareTag("ObstacleWallBack"))
            {
                if (PlayerPrefs.GetInt("ach9") == 0)
                {
                    PlayerPrefs.SetInt("achtakeable9", 1);
                    uiController.activateAchievementInfo(9);
                }
            }
            if (intScore >= 200 && uiController.getDifficulty() > 0 && PlayerPrefs.GetInt("ach1") == 0)
            {
                PlayerPrefs.SetInt("achtakeable1", 1);
                uiController.activateAchievementInfo(1);
            }
            if (intScore >= 1200 && uiController.getDifficulty() > 0 && PlayerPrefs.GetInt("ach2") == 0)
            {
                PlayerPrefs.SetInt("achtakeable2", 1);
                uiController.activateAchievementInfo(2);
            }
            if (intScore >= 2000 && uiController.getDifficulty() > 0 && PlayerPrefs.GetInt("ach3") == 0)
            {
                PlayerPrefs.SetInt("achtakeable3", 1);
                uiController.activateAchievementInfo(3);
            }
            if (intScore >= 5000 && uiController.getDifficulty() == 2 && PlayerPrefs.GetInt("ach4") == 0)
            {
                PlayerPrefs.SetInt("achtakeable4", 1);
                uiController.activateAchievementInfo(4);
            }
            if (intScore >= 10000 && uiController.getDifficulty() == 2 && PlayerPrefs.GetInt("ach5") == 0)
            {
                PlayerPrefs.SetInt("achtakeable5", 1);
                uiController.activateAchievementInfo(5);
            }
            ballController.getAdController().ShowInterstitialAd();
            if (collision.gameObject.CompareTag("ObstacleWallBack"))
            {
                uiController.setGameOverPanelComment(int.Parse(score.text), true);
            }
            else
            {
                uiController.setGameOverPanelComment(int.Parse(score.text), false);
            }

            gameOverPanel.SetActive(true);
            ballController.getPauseButton().gameObject.SetActive(false);
            timeController.setGameOver(true);
            timeController.endSlowdown();
            fb.StartCoroutine(fb.Shake(1f, 0.05f));
            fb.setFollowActive(false);
            audioController.playExplosion();
            Vector2 lastVelocity = ballController.getBall().GetComponent<Rigidbody2D>().velocity;
            foreach (GameObject frag in ball.GetComponent<Explodable>().fragments)
            {
                frag.transform.parent = null;
                frag.SetActive(true);
                Rigidbody2D fragRb = frag.GetComponent<Rigidbody2D>();
                fragRb.MovePosition(new Vector2(fragRb.position.x + Random.Range(-.2f, .2f), fragRb.position.y + Random.Range(-.2f, .2f)));
                fragRb.AddForce(new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f)) + lastVelocity * 0.5f, ForceMode2D.Impulse);
                fragRb.AddTorque(Random.Range(-2f, 2f) * 2f);
            }
            ball.SetActive(false);
            int currentScore = int.Parse(score.text);
            switch (uiController.getDifficulty())
            {
                case 0:
                    if (PlayerPrefs.GetInt("highScoreE") < currentScore)
                    {
                        PlayerPrefs.SetInt("highScoreE", currentScore);
                        uiController.setHighscoreSign(ball.transform.position.x);
                    }
                    break;
                case 1:
                    if (PlayerPrefs.GetInt("highScoreN") < currentScore)
                    {
                        PlayerPrefs.SetInt("highScoreN", currentScore);
                        uiController.setHighscoreSign(ball.transform.position.x);
                    }
                    break;
                case 2:
                    if (PlayerPrefs.GetInt("highScoreX") < currentScore)
                    {
                        PlayerPrefs.SetInt("highScoreX", currentScore);
                        uiController.setHighscoreSign(ball.transform.position.x);
                    }
                    break;
            }
            calculateAndSetGems();
            shopController.updateAmount();
        }
        else if (collision.gameObject.CompareTag("reset"))
        {
            fb.setFollowActive(false);
            audioController.playExplosion();
            Vector2 lastVelocity = ballController.getBall().GetComponent<Rigidbody2D>().velocity;
            foreach (GameObject frag in ball.GetComponent<Explodable>().fragments)
            {
                frag.transform.parent = null;
                frag.SetActive(true);
                Rigidbody2D fragRb = frag.GetComponent<Rigidbody2D>();
                fragRb.MovePosition(new Vector2(fragRb.position.x + Random.Range(-.2f, .2f), fragRb.position.y + Random.Range(-.2f, .2f)));
                fragRb.AddForce(new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f)) + lastVelocity * 0.5f, ForceMode2D.Impulse);
                fragRb.AddTorque(Random.Range(-2f, 2f) * 2f);
            }
            ball.SetActive(false);
            uiController.StartCoroutine(uiController.waitAndRetry());
        }
        else if (collision.gameObject.CompareTag("collectibleGem"))
        {
            collision.gameObject.SetActive(false);
            PlayerPrefs.SetInt("totalGems", uiController.getTotalGems() + 1);
            uiController.getTotalGemsText().text = PlayerPrefs.GetInt("totalGems").ToString();
            shopController.updateAmount();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ballController.getAudioController().playBounce();
        if (touchController.setCollided(collision.gameObject))
        {
            timeController.endSlowdown();
        }
    }

    private void calculateAndSetGems()
    {
        int scoreInt = int.Parse(score.text);
        int gemAmount;
        if (scoreInt < 100)
        {
            return;
        }
        else
        {
            gemAmount = scoreInt / 100;
            if (uiController.getDifficulty() == 2)
            {
                gemAmount *= 2;
            }
            else if (uiController.getDifficulty() == 0)
            {
                gemAmount /= 2;
            }
            PlayerPrefs.SetInt("totalGems",uiController.getTotalGems() + gemAmount);
            uiController.setEarnedGems(gemAmount);
            shopController.updateAmount();
        }

    }
}
