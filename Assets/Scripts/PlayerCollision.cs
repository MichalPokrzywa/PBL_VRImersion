using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] int rocksTouchedCount = 0;
    [SerializeField] int requiredTouches = 3;
    [SerializeField] Vector3 startPosition;
    [SerializeField] GameObject parentXR;

    HashSet<int> touchedRocks = new HashSet<int>();
    bool gameEnded = false;

    Rigidbody rb;

    const string WATER_TAG = "Water";
    const string ROCK_TAG = "Rock";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Restart();
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            Vector3 newPosition = parentXR.transform.position;
            Quaternion newRotation = parentXR.transform.rotation;
            rb.Move(newPosition, newRotation);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(ROCK_TAG) && !gameEnded)
        {
            int rockID = collision.collider.gameObject.GetInstanceID();

            if (!touchedRocks.Contains(rockID))
            {
                touchedRocks.Add(rockID);
                rocksTouchedCount++;
                Debug.Log($"Dotkniêto nowego kamienia! Dotkniêcia: {rocksTouchedCount}/{requiredTouches}");

                // Win condition
                if (rocksTouchedCount >= requiredTouches)
                {
                    gameEnded = true;
                }
            }
        }

        // Lose condition
        else if (collision.collider.CompareTag(WATER_TAG) && !gameEnded)
        {
            Restart();
        }
    }

    void Restart()
    {
        parentXR.transform.position = startPosition;
        rocksTouchedCount = 0;
        touchedRocks.Clear();
        gameEnded = false;
    }
}
