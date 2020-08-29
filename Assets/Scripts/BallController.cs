using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    private GameObject ball;
    private Camera mainCamera;
    private TimeController timeController;
    private FollowBall followBall;
    public TouchController touchController;
    public AudioController audioManager;
    public GameObject gameOverPanel;
    public AdController adManager;
    public TextMeshProUGUI score;
    public TextMeshProUGUI gameOverScore;
    public UIController uiController;
    public Button pauseButton;
    public ShopController shopController;

    private void Start()
    {
        mainCamera = Camera.main;
        timeController = mainCamera.GetComponent<TimeController>();
        followBall = mainCamera.GetComponent<FollowBall>();
    }

    internal TimeController getTimeController()
    {
        return timeController;
    }

    internal TouchController getTouchController()
    {
        return touchController;
    }

    internal GameObject getBall()
    {
        return ball;
    }

    public void setBall(GameObject ball)
    {
        this.ball = ball;
    }

    internal FollowBall getFollowball()
    {
        return followBall;
    }

    internal GameObject getGameOverPanel()
    {
        return gameOverPanel;
    }

    internal AdController getAdController()
    {
        return adManager;
    }

    internal TextMeshProUGUI getScore()
    {
        return score;
    }

    internal Button getPauseButton()
    {
        return pauseButton;
    }

    internal TextMeshProUGUI getGameOverScore()
    {
        return gameOverScore;
    }

    internal UIController getUIController()
    {
        return uiController;
    }

    internal AudioController getAudioController()
    {
        return audioManager;
    }

    internal ShopController getShopController()
    {
        return shopController;
    }
}
