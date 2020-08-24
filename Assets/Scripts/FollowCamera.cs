using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 posToMoveAt;
    private void Start()
    {
        mainCamera = Camera.main;
        posToMoveAt = transform.position;
    }
    private void LateUpdate()
    {
        if(mainCamera.transform.position.x > transform.position.x + 13f)
        {
            posToMoveAt.x = mainCamera.transform.position.x - 13f;
            transform.position = posToMoveAt;
        }
    }
}
