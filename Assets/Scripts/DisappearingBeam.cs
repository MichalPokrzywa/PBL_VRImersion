using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingBeam : MonoBehaviour
{
    public float disappearDelay = 2f; // Czas po jakim belka znika
    private bool isPlayerOnBeam = false; // Flaga sprawdzaj¹ca czy gracz jest na belce
    private Collider beamCollider;
    private Renderer beamRenderer;

    void Start()
    {
        // Pobierz komponenty belki
        beamCollider = GetComponent<Collider>();
        beamRenderer = GetComponent<Renderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // SprawdŸ, czy obiekt koliduj¹cy to gracz (zak³adamy tag "Player")
        if (collision.gameObject.CompareTag("Player") && !isPlayerOnBeam)
        {
            isPlayerOnBeam = true;
            Invoke(nameof(DisableBeam), disappearDelay);
        }
    }

    private void DisableBeam()
    {
        // Wy³¹cz mo¿liwoœæ chodzenia po belce
        beamCollider.enabled = false;
        beamRenderer.enabled = false; // Opcjonalnie - ukryj belkê
    }

    private void OnCollisionExit(Collision collision)
    {
        // Jeœli gracz opuszcza belkê, resetuj flagê (jeœli potrzebne)
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnBeam = false;
        }
    }
}
