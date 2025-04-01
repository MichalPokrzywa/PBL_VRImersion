using System;
using UnityEngine;

public class Axe : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public GameObject axe;
    private float rotation=180;
    [SerializeField] float speed;
    public static Action onDeath;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onDeath.Invoke();
        }
    }
    // Update is called once per frame
    void Update()
    {
        float target = Mathf.PingPong(Time.time*speed, rotation);
        transform.rotation = Quaternion.Euler(target-90,0,0);
    }
}
