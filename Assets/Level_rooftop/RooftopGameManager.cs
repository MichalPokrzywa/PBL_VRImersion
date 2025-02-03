using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RooftopGameMa : MonoBehaviour
{
    public MovementStats movementStats;
    public GameObject player;

    private void Start()
    {
        StartCollectingInfo();
        Finish.onFinishRoofTop += Reset;
    }

    private void Reset()
    {
        movementStats.StopMeasuring();
        SetUp();
    }

    private void SetUp()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void StartCollectingInfo()
    {
        movementStats.StartMeasuring($"Assets/Stats/Level_Rooftop_{System.DateTime.Now.ToString().Replace(' ', '_').Replace(':', '_')}.json");
    }
}
