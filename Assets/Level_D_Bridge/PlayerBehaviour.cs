using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] Vector3 startPosition;
    [SerializeField] XROrigin xrOrigin;
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource audioSource;

    public UnityAction<int> onCollisionWithCheckpoint;
    public UnityAction onCollisionWithWater;
    public bool VRMode = true;

    Rigidbody rb;

    const string WATER_TAG = "Water";
    const string CHECKPOINT_TAG = "Rock";
    const float timeToRestart = 1.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        RestartPosition();
    }

    void FixedUpdate()
    {
        if (rb != null && VRMode)
        {
            Vector3 newPosition = xrOrigin.transform.position;
            Quaternion newRotation = xrOrigin.transform.rotation;
            rb.Move(newPosition, newRotation);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if (collision.collider.CompareTag(CHECKPOINT_TAG))
        {
            int rockID = collision.collider.gameObject.GetInstanceID();
            onCollisionWithCheckpoint?.Invoke(rockID);
            Checkpoint checkpointScript = collision.collider.GetComponent<Checkpoint>();
            if (checkpointScript != null)
            {
                checkpointScript.DeactivateObject();
            }
        }
        else if (collision.collider.CompareTag(WATER_TAG))
        {
            audioSource.Play();
            onCollisionWithWater?.Invoke();
            Invoke(nameof(RestartPosition), timeToRestart);
        }
    }

    public void RestartPosition()
    {
        if (VRMode)
            xrOrigin.transform.position = startPosition;
        else
            transform.position = startPosition;
    }
}
