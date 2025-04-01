using UnityEngine;
using System;

public class WinCondition : MonoBehaviour
{
    public static Action onFinishRoofTop;
    public static Action onDeath;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onFinishRoofTop.Invoke();
        }
    }
}
