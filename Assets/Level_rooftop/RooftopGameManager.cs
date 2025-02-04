using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RooftopGameMa : MonoBehaviour
{
    [SerializeField] VRClickablePanel infoPanel;
    public MovementStats movementStats;
    public GameObject player;
    private string winMssg = "Gratuluje dotarcia do mety!!!";
    private string startMssg = "Twoim zadaniem jest przedostaæ siê na ostatni dach za pomoc¹ desek z, których utworzysz most. Powodznie!";

    private void Start()
    {
        infoPanel.ShowPanel(startMssg);
        StartCollectingInfo();
        Finish.onFinishRoofTop += UpdatePanel;
    }

    private void Reset()
    {
        movementStats.StopMeasuring();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void StartCollectingInfo()
    {
        movementStats.StartMeasuring($"Assets/Stats/Level_Rooftop_{System.DateTime.Now.ToString().Replace(' ', '_').Replace(':', '_')}.json");
    }

    void UpdatePanel()
    {
        string mssg = winMssg;
        infoPanel.ShowPanel(winMssg, Reset);
    }
}
