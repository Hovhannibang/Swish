using UnityEngine;

public class DestroyUIBall : MonoBehaviour
{
    public GameObject[] panels;
    public UIController uiController;
    public void deactivateMe()
    {
        gameObject.SetActive(false);
        uiController.shootBall();
        foreach (GameObject obj in panels)
        {
            obj.SetActive(false);
        }
    }
}
