using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingBeam : MonoBehaviour
{
    public float disappearDelay = 2f; // Czas po jakim belka znika
    private bool isPlayerOnBeam = false; // Flaga sprawdzaj�ca czy gracz jest na belce
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
        // Sprawd�, czy obiekt koliduj�cy to gracz (zak�adamy tag "Player")
        if (collision.gameObject.CompareTag("Player") && !isPlayerOnBeam)
        {
            isPlayerOnBeam = true;
            Invoke(nameof(DisableBeam), disappearDelay);
        }
    }

    private void DisableBeam()
    {
        // Wy��cz mo�liwo�� chodzenia po belce
        beamCollider.enabled = false;
        beamRenderer.enabled = false; // Opcjonalnie - ukryj belk�
    }

    private void OnCollisionExit(Collision collision)
    {
        // Je�li gracz opuszcza belk�, resetuj flag� (je�li potrzebne)
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnBeam = false;
        }
    }
}
