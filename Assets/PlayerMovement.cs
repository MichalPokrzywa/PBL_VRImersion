using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float rotationSensitivity;
    [SerializeField] float jumpSpeed;
    [SerializeField] float jumpdColdown;
    float jumpColdownLeft;
    Rigidbody rgbd;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rgbd= GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotation = Vector3.up * Input.GetAxisRaw("Mouse X") * rotationSensitivity;
        transform.Rotate(rotation, Space.Self);
        Vector3 newVelocity = new Vector3();
        newVelocity.x = Input.GetAxisRaw("Horizontal") * speed;
        newVelocity.z = Input.GetAxisRaw("Vertical") * speed;
        newVelocity.y = rgbd.linearVelocity.y;

        jumpColdownLeft -= Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Space) && jumpColdownLeft <= 0f)
        {
            newVelocity.y = jumpSpeed;
            jumpColdownLeft = jumpdColdown;
        }
        newVelocity = transform.TransformVector(newVelocity);
        rgbd.linearVelocity = newVelocity;
    }
}
