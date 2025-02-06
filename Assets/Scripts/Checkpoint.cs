using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    const string tag = "Shield";

    public void DeactivateObject()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag(tag))
            {
                child.gameObject.SetActive(false);
                break;
            }
        }
    }

    public void ActivateObject()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag(tag))
            {
                child.gameObject.SetActive(true);
                break;
            }
        }
    }
}
