using UnityEngine;

public class UpDown : MonoBehaviour
{
    [SerializeField] float upDown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotation = Vector3.right * Input.GetAxisRaw("Mouse Y") * upDown;
        transform.Rotate(rotation, Space.Self);
    }
}
