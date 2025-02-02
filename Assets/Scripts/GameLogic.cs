using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class GameLogic : MonoBehaviour
{
    [SerializeField] int checkpointsTouched = 0;
    [SerializeField] int requiredTouches = 3;
    [SerializeField] VRClickablePanel infoPanel;
    [SerializeField] Timer timer;
    [SerializeField] GameObject bridgesParent;
    [SerializeField] GameObject leftController;
    [SerializeField] GameObject rightController;
    [SerializeField] MovementStats stats;

    public static GameObject LeftController { get; private set; }
    public static GameObject RightController { get; private set; }

    [Header("Game Mode")]
    [SerializeField] bool VRMode;

    [Header("Keyboard Movement")]
    [SerializeField] PlayerCollision playerKeyboard;
    [SerializeField] GameObject keyboardMovementController;
    [SerializeField] StandaloneInputModule input;

    [Header("VR Movement")]
    [SerializeField] PlayerCollision playerVR;
    [SerializeField] XRUIInputModule xrInput;
    [SerializeField] List<GameObject> VRComponents;

    const string startMssg = "Dotrzyj do wszystkich checkpointów w ustalonym czasie. Powodzenia!";
    const string winMssg = "Gratulacje! Wygra³eœ!";
    const string loseMssg = "Przegra³eœ! Spróbuj jeszcze raz.";

    HashSet<int> touchedCheckpoints = new HashSet<int>();
    PlayerCollision player;
    BridgeBehaviour[] bridges;
    bool gameWinState = false;

    void Awake()
    {
        bridges = bridgesParent.GetComponentsInChildren<BridgeBehaviour>();

        LeftController = leftController;
        RightController = rightController;

        // Use playerCollider component based on VR or standalone mode
        player = VRMode ? playerVR : playerKeyboard;
        player.VRMode = VRMode;

        player.onCollisionWithCheckpoint += OnCheckpointTouched;
        player.onCollisionWithWater += OnGameLost;
        timer.OnTimerEnd += OnGameLost;

        UpdateComponentsState();
    }

    void OnDestroy()
    {
        if (player != null)
        {
            player.onCollisionWithCheckpoint -= OnCheckpointTouched;
            player.onCollisionWithWater -= OnGameLost;
        }
        if (timer != null)
            timer.OnTimerEnd -= OnGameLost;
    }

    void OnCheckpointTouched(int checkpointId)
    {
        if (!touchedCheckpoints.Contains(checkpointId))
        {
            touchedCheckpoints.Add(checkpointId);
            checkpointsTouched++;
            Debug.Log($"Touches: {checkpointsTouched}/{requiredTouches}");

            if (checkpointsTouched >= requiredTouches)
            {
                OnGameWin();
            }
        }
    }

    void OnGameWin()
    {
        gameWinState = true;
        timer.StopTimer();
        stats.StopMeasuring();
        UpdatePanel();
    }

    void OnGameLost()
    {
        gameWinState = false;
        UpdatePanel();
        timer.StopTimer();
        stats.StopMeasuring();
    }

    public void ResetAllBridges()
    {
        foreach (BridgeBehaviour bridge in bridges)
        {
            bridge.ResetBridgeState();
        }
    }

    void RestartGame()
    {
        ResetAllBridges();
        player.Restart();
        touchedCheckpoints.Clear();
        timer.ResetTimer();
        stats.StartMeasuring("Assets/test.json");
    }

    void UpdatePanel()
    {
        if (!VRMode)
        {
            return;
        }

        if (gameWinState)
        {
            string mssg = winMssg + $"\nCzas: {timer.TimerValue:0.00}s";
            infoPanel.ShowPanel(winMssg, RestartGame);
        }
        else
        {
            infoPanel.ShowPanel(loseMssg, RestartGame);
        }
    }

    void UpdateComponentsState()
    {
        if (VRMode)
            infoPanel.ShowPanel(startMssg, RestartGame);

        foreach (GameObject component in VRComponents)
        {
            component.SetActive(VRMode);
        }
        keyboardMovementController.SetActive(!VRMode);

        xrInput.enabled = VRMode;
        input.enabled = !VRMode;
    }
}
