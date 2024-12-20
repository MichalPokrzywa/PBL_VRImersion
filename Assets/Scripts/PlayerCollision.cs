using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private bool gameEnded = false;
    public GameObject objectToActivate;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Rock") && !gameEnded)
        {
            Debug.Log("Gracz dotkn¹³ kamienia. Koniec gry.");
            Invoke("EndGame", 0.5f); 
            gameEnded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ShooterActivation"))
        {
            objectToActivate.SetActive(true);
        }
    }

    void EndGame()
    {
        ChangeScene.Instance.MenuScene();
    }
}
