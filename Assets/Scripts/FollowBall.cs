using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FollowBall : MonoBehaviour
{
    public GameObject ball;
    public UIController uiController;
    private bool followActive;
    private bool firstSpawn = true;
    private float previousSpawnX;
    private float previousSpawnY;
    private Vector2 screenBounds;
    private Vector3 tempPos;
    public GameObject obstacle;
    public GameObject obstacles;
    private GameObject temp;
    private TimeController timeController;
    public TouchController touchController;
    private readonly float topYpos = 2.828428f;
    private readonly float deltaYpos = 1.131371f;
    private readonly Queue<GameObject> obstaclePool = new Queue<GameObject>();

    private void Start()
    {
        followActive = true;
        screenBounds = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, transform.position.z));
        previousSpawnX = screenBounds.x - 3f;
        timeController = GetComponent<TimeController>();

        for (int i = 0; i < 10; i++)
        {
            temp = Instantiate(obstacle);
            temp.transform.SetParent(obstacles.transform);
            temp.SetActive(false);
            obstaclePool.Enqueue(temp);
        }
    }

    private void LateUpdate()
    {
        if (ball != null && followActive)
        {
            tempPos = transform.position;
            tempPos.x = ball.transform.position.x + 5f;
            transform.position = tempPos;
        }
    }

    private void FixedUpdate()
    {
        spawnObstacles();
    }

    public void setPreviousSpawn()
    {
        previousSpawnX = screenBounds.x - 3f;
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            magnitude -= 0.05f * (Time.unscaledDeltaTime / duration);
            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }

    public void setBall(GameObject ball)
    {
        this.ball = ball;
    }

    public void setFollowActive(bool followActive)
    {
        this.followActive = followActive;
    }

    public void setFirstSpawn()
    {
        firstSpawn = true;
        previousSpawnX = screenBounds.x - 3f;
    }

    private void spawnObstacles()
    {
        if (timeController.isGameStarted() && transform.position.x + screenBounds.x > previousSpawnX + 4f)
        {
            float yPosToSpawn = topYpos;
            bool spawn = true;
            float randomValue = Random.value;
            if (randomValue >= 0f && randomValue < 0.075f)
            {
                yPosToSpawn = topYpos - deltaYpos;
            }
            else if (randomValue >= 0.075f && randomValue < 0.125f)
            {
                yPosToSpawn = topYpos - deltaYpos * 2;
            }
            else if (randomValue >= 0.125f && randomValue < 0.3f)
            {
                yPosToSpawn = topYpos - deltaYpos * 3;
            }
            else if (randomValue >= 0.3f && randomValue < 0.6f)
            {
                yPosToSpawn = topYpos - deltaYpos * 4;
            }
            else if (randomValue >= 0.06f && randomValue < 0.725f)
            {
                yPosToSpawn = topYpos - deltaYpos * 5;
            }

            if (previousSpawnY == yPosToSpawn)
            {
                spawn = false;
            }
            if (spawn)
            {
                GameObject tempObstacle = obstaclePool.Dequeue();
                tempObstacle.SetActive(true);
                tempObstacle.GetComponent<LineRenderer>().enabled = false;
                tempObstacle.GetComponent<ObstacleMoveController>().pingPongDown = false;
                tempObstacle.GetComponent<ObstacleMoveController>().pingPongUp = false;
                obstaclePool.Enqueue(tempObstacle);
                if (!firstSpawn)
                {
                    tempObstacle.transform.position = new Vector3(previousSpawnX + 4.5f, yPosToSpawn, 0f);
                    if (yPosToSpawn == topYpos - deltaYpos)
                    {
                        float spawnProb = Random.value;
                        float probThresh = -1f;
                        int currentScore = uiController.getCurrentScore();
                        if (currentScore >= 250 && currentScore < 1000)
                        {
                            probThresh = 0.25f;
                        }
                        else if (currentScore >= 1000 && currentScore < 1500)
                        {
                            probThresh = 0.35f;
                        }
                        else if (currentScore >= 1500 && currentScore < 2000)
                        {
                            probThresh = 0.45f;
                        }
                        else if (currentScore >= 2000 && currentScore < 2500)
                        {
                            probThresh = 0.55f;
                        }
                        else if (currentScore >= 2500)
                        {
                            probThresh = 0.65f;
                        }
                        if (spawnProb >= 1f - probThresh)
                        {
                            LineRenderer lr = tempObstacle.GetComponent<LineRenderer>();
                            lr.SetPosition(0, tempObstacle.transform.position);
                            lr.SetPosition(1, new Vector2(tempObstacle.transform.position.x, -0.565685f));
                            lr.enabled = true;
                            tempObstacle.GetComponent<ObstacleMoveController>().pingPongDown = true;
                        }
                    }
                    else if (yPosToSpawn == topYpos - deltaYpos * 4)
                    {
                        float spawnProb = Random.value;
                        float probThresh = -1f;
                        int currentScore = uiController.getCurrentScore();
                        if (currentScore >= 250 && currentScore < 1000)
                        {
                            probThresh = 0.25f;
                        }
                        else if (currentScore >= 1000 && currentScore < 1500)
                        {
                            probThresh = 0.35f;
                        }
                        else if (currentScore >= 1500 && currentScore < 2000)
                        {
                            probThresh = 0.45f;
                        }
                        else if (currentScore >= 2000 && currentScore < 2500)
                        {
                            probThresh = 0.55f;
                        }
                        else if (currentScore >= 3000)
                        {
                            probThresh = 0.65f;
                        }
                        if (uiController.getDifficulty() == 2)
                        {
                            probThresh += 0.1f;
                        }
                        if (spawnProb >= 1f - probThresh)
                        {
                            LineRenderer lr = tempObstacle.GetComponent<LineRenderer>();
                            lr.SetPosition(0, tempObstacle.transform.position);
                            lr.SetPosition(1, new Vector2(tempObstacle.transform.position.x, 0.565685f));
                            lr.enabled = true;
                            tempObstacle.GetComponent<ObstacleMoveController>().pingPongUp = true;
                        }
                    }
                }
                else
                {
                    int rnd = Random.Range(2, 4);
                    tempObstacle.transform.position = new Vector3(previousSpawnX + 4.5f, topYpos - (rnd * deltaYpos), 0f);
                    firstSpawn = false;
                }
                previousSpawnX = tempObstacle.transform.position.x;
                previousSpawnY = tempObstacle.transform.position.y;
            }
        }
    }
}
