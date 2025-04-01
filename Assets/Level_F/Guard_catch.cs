using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Guard_catch : MonoBehaviour
{

    public static Action onDeath;

    private void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Player"))
       {
            SceneManager.LoadScene("Level_F");
       }
    }
}