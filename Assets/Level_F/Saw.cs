using System;
using Unity.VisualScripting;
using UnityEngine;

public class Saw : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    [SerializeField] float movementspeed;
    public GameObject saw;
    private bool direction=true;
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
        if (saw.transform.position.z <= 463.66 + -2.5 && direction==true)
        {
            movementspeed *= -1;
            direction = false;
        }
        if(saw.transform.position.z >= 463.66 && direction==false)
        {
            movementspeed *= -1;
            direction = true;
        }
        saw.transform.position = new Vector3(saw.transform.position.x, saw.transform.position.y,saw.transform.position.z + movementspeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(180 * Time.time, 0, 90);
    }
}
