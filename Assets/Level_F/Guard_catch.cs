using System;
using UnityEngine;

public class Guard_catch : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    //
    public static Action onDeath;


    private void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Player"))
       {
                onDeath.Invoke();
       }
    }
    void Update()
    {
        
    }
}
