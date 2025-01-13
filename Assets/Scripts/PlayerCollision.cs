using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private bool gameEnded = false;
    [SerializeField] private HashSet<int> touchedRocks = new HashSet<int>(); // Zestaw przechowuj�cy ID dotkni�tych kamieni
    [SerializeField] private int rocksTouchedCount = 0; // Licznik dotkni�tych unikalnych kamieni
    public int requiredTouches = 3; // Liczba wymaganych dotkni�� r�nych kamieni

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Rock") && !gameEnded)
        {
            // Pobierz unikalny identyfikator kamienia 
            int rockID = collision.collider.gameObject.GetInstanceID();

            // Sprawd�, czy kamie� zosta� ju� dotkni�ty
            if (!touchedRocks.Contains(rockID))
            {
                touchedRocks.Add(rockID); // Dodaj ID kamienia do zestawu
                rocksTouchedCount++; // Zwi�ksz licznik dotkni��
                Debug.Log($"Dotkni�to nowego kamienia! Dotkni�cia: {rocksTouchedCount}/{requiredTouches}");

                // Sprawd�, czy gracz dotkn�� wystarczaj�cej liczby kamieni
                if (rocksTouchedCount >= requiredTouches)
                {
                    Debug.Log("Gracz dotkn�� wystarczaj�cej liczby kamieni. Koniec gry.");
                    Invoke("EndGame", 0.5f);
                    gameEnded = true;
                }
            }
            else
            {
                Debug.Log("Ten kamie� zosta� ju� dotkni�ty!");
            }
        }
        if (collision.collider.CompareTag("Water") && !gameEnded)
        {
            ChangeScene.Instance.MenuSecondScene();
        }
    }

    void EndGame()
    {
        ChangeScene.Instance.MenuScene(); // Twoja metoda zmiany sceny
    }
}
