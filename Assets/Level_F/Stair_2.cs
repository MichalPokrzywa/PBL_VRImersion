using UnityEngine;

public class Stair2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject stair2;
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
        if (rotated == false && box_Of_Items.items_in_box == 2)
        {
            rotated = true;
            stair2.transform.Rotate(90,0,0);
        }
    }
}
