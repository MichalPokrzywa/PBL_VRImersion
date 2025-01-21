using UnityEngine;

public class FollowAndRotate : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0, 1, 2);
    [SerializeField] float followSpeed = 5f;

    void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + target.TransformDirection(offset);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

            Vector3 directionToTarget = target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * followSpeed);
        }
    }
}
