using UnityEngine;
using System.Collections;
using System.Linq;

public class DisappearingBeam : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] BridgeBehaviour bridgeBehaviour;
    [SerializeField] float disappearDelay = 2f;
    [SerializeField] Collider collisionCollider;
    [SerializeField] Collider triggerCollider;

    public Rigidbody beamRb => rb;

    bool isPlayerOnBeam = false;

    Renderer beamRenderer;
    Material beamMaterial;

    float blinkSpeed = 1f;
    Color blinkColor = Color.red;
    Coroutine blinkCoroutine;

    const string PLAYER_TAG = "Player";

    void Start()
    {
        triggerCollider = GetComponent<Collider>();
        collisionCollider = GetComponentsInChildren<Collider>().Where(go => go.gameObject != this.gameObject).First();
        beamRenderer = GetComponent<Renderer>();
        if (beamRenderer != null)
        {
            beamMaterial = beamRenderer.material;
        }
        if (bridgeBehaviour == null)
        {
            bridgeBehaviour = GetComponentInParent<BridgeBehaviour>();
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag(PLAYER_TAG) && !isPlayerOnBeam)
        {
            isPlayerOnBeam = true;
            bridgeBehaviour.IsPlayerOnBridge = true;
            Invoke(nameof(DisableBeam), disappearDelay);
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
            }
            blinkCoroutine = StartCoroutine(BlinkBeam());
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag(PLAYER_TAG))
        {
            isPlayerOnBeam = false;
            bridgeBehaviour.IsPlayerOnBridge = false;
        }
    }

    public void EnableBeam()
    {
        collisionCollider.enabled = true;
        triggerCollider.enabled = true;
        beamRenderer.enabled = true;
    }

    void DisableBeam()
    {
        collisionCollider.enabled = false;
        triggerCollider.enabled = false;
        beamRenderer.enabled = false;
    }

    IEnumerator BlinkBeam()
    {
        float elapsedTime = 0f;
        while (elapsedTime < disappearDelay)
        {
            beamMaterial.color = (beamMaterial.color == blinkColor) ? Color.white : blinkColor;
            elapsedTime += blinkSpeed;
            blinkSpeed = Mathf.Clamp(blinkSpeed - 0.01f, 0.05f, 0.2f);
            yield return new WaitForSeconds(blinkSpeed);
        }

        DisableBeam();
        beamMaterial.color = Color.white;
    }
}
