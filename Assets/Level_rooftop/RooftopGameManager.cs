using Unity.VisualScripting;
using UnityEngine;

public class RooftopGameMa : MonoBehaviour
{
    public MovementStats movementStats;

    private void Start()
    {
        movementStats.StartMeasuring($"Assets/Stats/Level_Rooftop_{System.DateTime.Now.ToString().Replace(' ','_').Replace(':','_')}.json");
    }
    private void OnDestroy()
    {
        movementStats.StopMeasuring();
    }
}
