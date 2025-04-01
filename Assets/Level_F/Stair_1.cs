using UnityEngine;

public class Stair1 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject stair1;
    public GameObject box;
    Box_of_items box_Of_Items;
    private bool rotated;
    void Start()
    {
        box_Of_Items = box.GetComponent<Box_of_items>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rotated == false && box_Of_Items.items_in_box == 1)
        {
            rotated = true;
            transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }
}
