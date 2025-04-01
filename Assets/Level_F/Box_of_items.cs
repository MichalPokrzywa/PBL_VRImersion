using UnityEngine;

public class Box_of_items : MonoBehaviour
{
    public int items_in_box = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Relict"))
        {
            collider.gameObject.SetActive(false);
            items_in_box++;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
