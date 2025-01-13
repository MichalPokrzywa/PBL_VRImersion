using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private bool gameEnded = false;
    [SerializeField] private HashSet<int> touchedRocks = new HashSet<int>(); // Zestaw przechowuj¹cy ID dotkniêtych kamieni
    [SerializeField] private int rocksTouchedCount = 0; // Licznik dotkniêtych unikalnych kamieni
    public int requiredTouches = 3; // Liczba wymaganych dotkniêæ ró¿nych kamieni

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Rock") && !gameEnded)
        {
            // Pobierz unikalny identyfikator kamienia 
            int rockID = collision.collider.gameObject.GetInstanceID();

            // SprawdŸ, czy kamieñ zosta³ ju¿ dotkniêty
            if (!touchedRocks.Contains(rockID))
            {
                touchedRocks.Add(rockID); // Dodaj ID kamienia do zestawu
                rocksTouchedCount++; // Zwiêksz licznik dotkniêæ
                Debug.Log($"Dotkniêto nowego kamienia! Dotkniêcia: {rocksTouchedCount}/{requiredTouches}");

                // SprawdŸ, czy gracz dotkn¹³ wystarczaj¹cej liczby kamieni
                if (rocksTouchedCount >= requiredTouches)
                {
                    Debug.Log("Gracz dotkn¹³ wystarczaj¹cej liczby kamieni. Koniec gry.");
                    Invoke("EndGame", 0.5f);
                    gameEnded = true;
                }
            }
            else
            {
                Debug.Log("Ten kamieñ zosta³ ju¿ dotkniêty!");
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
