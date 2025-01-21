using UnityEngine;
using UnityEngine.Events;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] Vector3 startPosition;
    [SerializeField] GameObject parentXR;

    public UnityAction<int> onCollisionWithCheckpoint;
    public UnityAction onCollisionWithWater;
    public bool VRMode = true;

    Rigidbody rb;

    const string WATER_TAG = "Water";
    const string CHECKPOINT_TAG = "Rock";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Restart();
    }

    void FixedUpdate()
    {
        if (rb != null && VRMode)
        {
            Vector3 newPosition = parentXR.transform.position;
            Quaternion newRotation = parentXR.transform.rotation;
            rb.Move(newPosition, newRotation);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(CHECKPOINT_TAG))
        {
            int rockID = collision.collider.gameObject.GetInstanceID();
            onCollisionWithCheckpoint?.Invoke(rockID);
        }

        else if (collision.collider.CompareTag(WATER_TAG))
        {
            onCollisionWithWater?.Invoke();
            Restart();
        }
    }

    public void Restart()
    {
        if (VRMode)
            parentXR.transform.position = startPosition;
        else
            transform.position = startPosition;
    }
}
