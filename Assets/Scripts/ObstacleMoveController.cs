using UnityEngine;

public class ObstacleMoveController : MonoBehaviour
{
    public bool pingPongUp = false;
    public bool pingPongDown = false;
    private float delta = 2.262742f;

    void FixedUpdate()
    {
        if (pingPongDown)
        {
            float y = Mathf.PingPong(2 * Time.time, delta);
            Vector3 pos = new Vector3(transform.position.x, 1.697056f - y, transform.position.z);
            transform.position = pos;
        }
        else if (pingPongUp)
        {
            float y = Mathf.PingPong(2 * Time.time, delta);
            Vector3 pos = new Vector3(transform.position.x, -1.697056f + y, transform.position.z);
            transform.position = pos;
        }
    }
}
