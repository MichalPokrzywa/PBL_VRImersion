using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class WinCondition : MonoBehaviour
{
    public static Action onFinishRoofTop;
    public static Action onDeath;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Level_F");
        }
    }
}
