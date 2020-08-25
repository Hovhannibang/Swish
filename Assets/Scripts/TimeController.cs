using System.Collections;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public UIController uicontroller;
    public TouchController touchController;
    private Coroutine currentCoroutineAfter05s;
    private bool gameStarted;
    private bool gameOver;
    private bool gamePaused;
    private bool slowDown;
    private float slowdownFactor = 0.20f;
    float yVelocity = 0.0f;

    private void Start()
    {
        gameOver = false;
        gameStarted = false;
    }

    public void startSlowdown()
    {
        if (gameStarted)
        {
            slowDown = true;
            StartCoroutine(startSlowdownCoroutine());
        }
    }

    private IEnumerator startSlowdownCoroutine()
    {
        if (uicontroller.getDifficulty() == 2)
        {
            currentCoroutineAfter05s = StartCoroutine(endSlowdownAfter0p5s());
        }
        while (Time.timeScale > (slowdownFactor + 0.005f) && slowDown)
        {
            Time.timeScale = Mathf.SmoothDamp(Time.timeScale, slowdownFactor, ref yVelocity, 0.025f);
            Time.fixedDeltaTime = Time.timeScale * (1f / Application.targetFrameRate);
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = slowdownFactor;
        yield return null;
    }

    public void endSlowdown()
    {
        slowDown = false;
        StartCoroutine(endSlowdownCoroutine());
        if (currentCoroutineAfter05s != null)
        {
            StopCoroutine(currentCoroutineAfter05s);
        }
    }

    public IEnumerator endSlowdownAfter0p5s()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        touchController.setCollided();
        endSlowdown();
        yield return null;
    }

    public void stopSlowdown()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = (1f / Application.targetFrameRate);
    }
    private IEnumerator endSlowdownCoroutine()
    {
        while (Time.timeScale < 0.995f && !slowDown && !gamePaused)
        {
            Time.timeScale = Mathf.SmoothDamp(Time.timeScale, 1f, ref yVelocity, 0.025f);
            Time.fixedDeltaTime = Time.timeScale * (1f / Application.targetFrameRate);
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = 1f;
        yield return null;
    }

    public void setGameStarted(bool gameStarted)
    {
        this.gameStarted = gameStarted;
    }

    public bool isGameStarted()
    {
        return gameStarted;
    }

    public void setGameOver(bool gameOver)
    {
        this.gameOver = gameOver;
    }

    public bool isGameOver()
    {
        return gameOver;
    }

    public void setGamePaused(bool gamePaused)
    {
        this.gamePaused = gamePaused;
    }

    public bool isGamePaused()
    {
        return gamePaused;
    }
}
