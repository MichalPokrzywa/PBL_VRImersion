using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RooftopGameManager : MonoBehaviour
{
    [SerializeField] VRClickablePanel infoPanel;
    public MovementStats movementStats;
    public GameObject player;
    private string winMssg = "Gratuluje dotarcia do mety!!!";
    private string startMssg = "Twoim zadaniem jest przedostaæ siê na ostatni dach za pomoc¹ desek z, których utworzysz most. Powodznie!";

    private void Start()
    {
        infoPanel.ShowPanel(startMssg, StartCollectingInfo);
        Finish.onFinishRoofTop += UpdatePanel;
        Death.onDeath += FallReset;
    }

    private void FallReset()
    {
        movementStats.StopMeasuring(false);
        Reset();
    }

    private void Reset()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name,LoadSceneMode.Single);
    }

    private void StartCollectingInfo()
    {
        movementStats.StartMeasuring($"Assets/Stats/Level_Rooftop_{System.DateTime.Now.ToString().Replace(' ', '_').Replace(':', '_')}.json");
    }

    void UpdatePanel()
    {
        string mssg = winMssg;
        movementStats.StopMeasuring(true);
        infoPanel.ShowPanel(winMssg, Reset);
    }

    private void OnDestroy()
    {
        Finish.onFinishRoofTop -= UpdatePanel;
        Death.onDeath -= FallReset;
    }
}
