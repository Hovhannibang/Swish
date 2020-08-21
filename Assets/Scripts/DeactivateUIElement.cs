using UnityEngine;

public class DeactivateUIElement : MonoBehaviour
{
    public void setInteractableFalse()
    {
        GetComponent<CanvasGroup>().interactable = false;
    }

    public void deactivate()
    {
        gameObject.SetActive(false);
    }
}
