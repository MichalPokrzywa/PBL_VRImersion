using UnityEngine;
using UnityEngine.AI;

public class Guard_patrol : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    public Patrol_area patrol_area;
    public Vector3 nextPos;
    public float AtPoint = 1.5f;
    public Vector3 GuardPos;
    int currentWayPointIndex = 0;
    void Update()
    {
        Patroling();
    }
    private void Patroling()
    {
        nextPos = GuardPos;
        if(patrol_area != null)
        {
            if (AtWayPoint())
            {
                CycleWayPoints();
            }
            nextPos = GetCurrentWayPoint();
        }
        GetComponent<NavMeshAgent>().destination = nextPos;
    }
    private bool AtWayPoint()
    {
        float distanceToWayPoint = Vector3.Distance(transform.position, GetCurrentWayPoint());
        return distanceToWayPoint < AtPoint;
    }
    private void CycleWayPoints()
    {
        currentWayPointIndex = patrol_area.NextIndex(currentWayPointIndex);
    }
    private Vector3 GetCurrentWayPoint()
    {
        return patrol_area.GetWayPoint(currentWayPointIndex);
    }
}
