using UnityEngine;

public class Patrol_area : MonoBehaviour
{
    const float WayPointDrawRadius = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnDrawGizmos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            int j = NextIndex(i);
            Gizmos.DrawSphere(GetWayPoint(i), WayPointDrawRadius);
            Gizmos.DrawLine(GetWayPoint(i), GetWayPoint(j));
        }
    }
    public int NextIndex(int ind)
    {
        if (ind + 1 == transform.childCount)
        {
            return 0;
        }
        return ind + 1;
    }
    public Vector3 GetWayPoint(int i)
    {
        return transform.GetChild(i).position;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
