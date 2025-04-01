using System;
using UnityEngine;

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
            onDeath.Invoke();
        }
        if (other.CompareTag("Relict"))
        {
            other.gameObject.transform.position = new Vector3(530,100,437);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
