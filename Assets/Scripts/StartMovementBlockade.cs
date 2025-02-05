using UnityEngine;

public class StartMovementBlockade : MonoBehaviour
{
    [SerializeField] GameObject[] colliders;

    public void BlockMovement()
    {
        foreach (var collider in colliders)
        {
            collider.SetActive(true);
        }
    }

    public void UnblockMovement()
    {
        foreach (var collider in colliders)
        {
            collider.SetActive(false);
        }
    }
}
