using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class out_of_bounds : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public static Action onDeath;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Level_F");
        }
        if (other.CompareTag("Relict"))
        {
            SceneManager.LoadScene("Level_F");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
