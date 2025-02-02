using UnityEngine;
using System.Collections;

public class DisappearingBeam : MonoBehaviour
{
    public float disappearDelay = 2f;
    private bool isPlayerOnBeam = false;
    private Collider beamCollider;
    private Renderer beamRenderer;

    private float blinkSpeed = 0.2f;  
    private Color blinkColor = Color.red;  
    private Material beamMaterial;  
    private Coroutine blinkCoroutine;  

    const string PLAYER_TAG = "Player";

    void Start()
    {
        beamCollider = GetComponent<Collider>();
        beamRenderer = GetComponent<Renderer>();
        if (beamRenderer != null)
        {
            beamMaterial = beamRenderer.material;  
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(PLAYER_TAG) && !isPlayerOnBeam)
        {
            isPlayerOnBeam = true;
            Invoke(nameof(DisableBeam), disappearDelay);
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);  
            }
            blinkCoroutine = StartCoroutine(BlinkBeam());
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
