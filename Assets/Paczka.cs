using UnityEngine;

public class Paczka : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Paczka paczka;
        paczka = collision.collider.GetComponent<Paczka>();
        Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
