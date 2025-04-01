using UnityEngine;

public class Stair3 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject stair3;
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
        if (rotated == false && box_Of_Items.items_in_box == 3)
        {
            rotated = true;
            stair3.transform.Rotate(90,0,0);
        }
    }
}
