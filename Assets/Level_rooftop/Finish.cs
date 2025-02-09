using System;
using UnityEngine;

public class Finish : MonoBehaviour
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
