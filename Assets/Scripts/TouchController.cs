using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject wall;
    public GameObject walls;
    private GameObject tempWall;
    private Vector2 topPos = new Vector2(0f, 10f);
    private TimeController timeController;
    private bool collided;
    private Touch touch;
    private bool spawned;
    private LineRenderer lr;
    private GameObject temp;

    private readonly Queue<GameObject> wallPool = new Queue<GameObject>();

    private void Start()
    {
        timeController = mainCamera.GetComponent<TimeController>();

        for (int i = 0; i < 10; i++)
        {
            temp = Instantiate(wall);
            temp.transform.SetParent(walls.transform);
            temp.SetActive(false);
            wallPool.Enqueue(temp);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.touchCount > 0 && !timeController.isGameOver() && timeController.isGameStarted() && !timeController.isGamePaused())
        {
            touch = Input.GetTouch(0);
            Vector3 touchPosition = mainCamera.ScreenToWorldPoint(touch.position);
            if (touch.phase == TouchPhase.Began && touchPosition.y < 3.6 && !isTouchOnBall(touchPosition))
            {
                tempWall = wallPool.Dequeue();
                wallPool.Enqueue(tempWall);
                tempWall.GetComponent<EdgeCollider2D>().points = new Vector2[] { topPos, topPos };
                spawned = false;
                collided = false;

                lr = tempWall.GetComponent<LineRenderer>();
                if (touchPosition.y > 3.4f)
                {
                    lr.SetPosition(0, new Vector3(touchPosition.x, 3.4f, 0f));
                    lr.SetPosition(1, new Vector3(touchPosition.x, 3.4f, 0f));
                }
                else if (touchPosition.y < -3.4f)
                {
                    lr.SetPosition(0, new Vector3(touchPosition.x, -3.4f, 0f));
                    lr.SetPosition(1, new Vector3(touchPosition.x, -3.4f, 0f));
                }
                else
                {
                    lr.SetPosition(0, new Vector3(touchPosition.x, touchPosition.y, 0f));
                    lr.SetPosition(1, new Vector3(touchPosition.x, touchPosition.y, 0f));
                }
                tempWall.SetActive(true);
                timeController.startSlowdown();
            }
            if (tempWall != null && !collided && !spawned)
            {
                lr = tempWall.GetComponent<LineRenderer>();
                if (touchPosition.y > 3.4f)
                {
                    lr.SetPosition(1, new Vector3(touchPosition.x, 3.4f, 0f));
                }
                else if (touchPosition.y < -3.4f)
                {
                    lr.SetPosition(1, new Vector3(touchPosition.x, -3.4f, 0f));
                }
                else
                {
                    lr.SetPosition(1, new Vector3(touchPosition.x, touchPosition.y, 0f));
                }
                Vector3 startPos = lr.GetPosition(0);
                Vector3 endPos = lr.GetPosition(1);
                Vector3 dir = endPos - startPos;
                float dist = Mathf.Clamp(Vector3.Distance(startPos, endPos), 0, 2.5f);
                lr.SetPosition(1, startPos + (dir.normalized * dist));
                tempWall.GetComponent<EdgeCollider2D>().points = new Vector2[] { tempWall.GetComponent<LineRenderer>().GetPosition(0), tempWall.GetComponent<LineRenderer>().GetPosition(1) };
            }
            if (touch.phase == TouchPhase.Ended)
            {
                if (tempWall != null)
                {
                    lr = tempWall.GetComponent<LineRenderer>();
                    if ((lr.GetPosition(0) - lr.GetPosition(1)).sqrMagnitude < 0.05 && !collided)
                    {
                        tempWall.SetActive(false);
                    }
                }
                spawned = true;
                timeController.endSlowdown();
            }
        }
    }

    public void setCollided()
    {
        collided = true;
    }
    public bool setCollided(GameObject wall)
    {
        if (wall == tempWall)
        {
            collided = true;
            return true;
        }
        return false;
    }

    private bool isTouchOnBall(Vector3 touchPosition)
    {
        RaycastHit2D hitInformation = Physics2D.Raycast(touchPosition, Camera.main.transform.forward, 2, LayerMask.GetMask("DeadZone"));

        if (hitInformation.collider != null)
        {
            GameObject touchedObject = hitInformation.transform.gameObject;
            if (touchedObject.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
}
