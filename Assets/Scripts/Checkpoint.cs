using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public void DeactivateObject()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Shield"))
            {
                child.gameObject.SetActive(false);
                break; 
            }
        }
    }
}
