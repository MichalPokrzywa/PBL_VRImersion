using UnityEngine;

public class DisappearingBeam : MonoBehaviour
{
    public float disappearDelay = 2f;
    private bool isPlayerOnBeam = false;
    private Collider beamCollider;
    private Renderer beamRenderer;

    const string PLAYER_TAG = "Player";

    void Start()
    {
        beamCollider = GetComponent<Collider>();
        beamRenderer = GetComponent<Renderer>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(PLAYER_TAG) && !isPlayerOnBeam)
        {
            isPlayerOnBeam = true;
            Invoke(nameof(DisableBeam), disappearDelay);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(PLAYER_TAG))
        {
            isPlayerOnBeam = false;
        }
    }

    public void EnableBeam()
    {
        beamCollider.enabled = true;
        beamRenderer.enabled = true;
    }

    void DisableBeam()
    {
        beamCollider.enabled = false;
        beamRenderer.enabled = false;
    }
}
