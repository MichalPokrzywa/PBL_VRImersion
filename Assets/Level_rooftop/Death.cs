using System;
using UnityEngine;

public class Death : MonoBehaviour
{
    public static Action onDeath;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onDeath.Invoke();
        }
    }
}
