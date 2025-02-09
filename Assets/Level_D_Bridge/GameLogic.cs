using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class GameLogic : MonoBehaviour
{
    [SerializeField] int checkpointsTouched = 0;
    [SerializeField] int requiredTouches = 3;
    [SerializeField] VRClickablePanel infoPanel;
    [SerializeField] Timer timer;
    [SerializeField] GameObject bridgesParent;
    [SerializeField] CheckpointManager checkpointManager;
    [SerializeField] StartMovementBlockade blockade;
    [SerializeField] MovementStats stats;

    [Header("Game Mode")]
    [SerializeField] bool VRMode;
    [SerializeField] bool katVR = false;

    [Header("Keyboard Movement")]
    [SerializeField] PlayerBehaviour playerKeyboard;
    [SerializeField] GameObject keyboardMovementController;
    [SerializeField] StandaloneInputModule input;

    [Header("VR Movement")]
    [SerializeField] PlayerBehaviour playerVR;
    [SerializeField] XRUIInputModule xrInput;
    [SerializeField] List<GameObject> VRComponents;

    string startMssg;
    const string winMssg = "Wygra³eœ!";
    const string loseMssg = "Przegra³eœ! Spróbuj jeszcze raz.";

    HashSet<int> touchedCheckpoints = new HashSet<int>();
    PlayerBehaviour player;
    BridgeBehaviour[] bridges;
    bool gameWinState = false;
    bool gameActive = false;

    void Start()
    {
        string checkpointInfo = requiredTouches.ToString();
        if (requiredTouches == 1)
            checkpointInfo += " checkpointa";
        else checkpointInfo += " checkpointów";
        startMssg = "Dotrzyj do " + checkpointInfo + " przed up³ywem czasu! Im mocniej wychylisz ga³kê kontrolera, tym szybciej siê poruszasz.";

        bridges = bridgesParent.GetComponentsInChildren<BridgeBehaviour>();
        if (VRMode)
        {
            foreach (var bridge in bridges)
            {
                bridge.OnStandingBeamDisappear += ForcePlayerCollisionUpdate;
            }
        }

        player = VRMode ? playerVR : playerKeyboard;
        player.VRMode = VRMode;

        player.onCollisionWithCheckpoint += OnCheckpointTouched;
        player.onCollisionWithWater += OnGameLost;
        timer.OnTimerEnd += OnGameLost;

        UpdateComponentsState();
        ClearState();
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

        foreach (var bridge in bridges)
        {
            if (bridge != null)
                bridge.OnStandingBeamDisappear -= ForcePlayerCollisionUpdate;
        }
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
        if (!gameActive)
            return;

        Debug.Log("<color=yellow>GAME WIN!</color>");
        gameActive = false;
        gameWinState = true;
        timer.StopTimer();
        stats.StopMeasuring(true);
        string mssg = winMssg + $" Twój czas: {timer.TimePassed}";
        infoPanel.ShowPanel(mssg, RestartGame);
    }

    void OnGameLost()
    {
        if (!gameActive)
            return;

        Debug.Log("<color=yellow>GAME LOST!</color>");
        gameActive = false;
        gameWinState = false;
        timer.StopTimer();
        stats.StopMeasuring(false);
        RestartGame();
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
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    void ClearState()
    {
        ResetAllBridges();
        blockade.BlockMovement();
        checkpointManager.ActivateCheckpoints();
        touchedCheckpoints.Clear();
        player.RestartPosition();
        if (VRMode)
            infoPanel.ShowPanel(startMssg, ActivateGame);
    }

    void ActivateGame()
    {
        Debug.Log("<color=yellow>GAME STARTED!</color>");
        timer.ResetTimer();
        blockade.UnblockMovement();
        gameActive = true;

        StringBuilder sb = new StringBuilder();
        sb.Append("Level_D_");
        sb.Append(katVR ? "KatVR_" : "VR_");
        string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        sb.Append(dateTime);
        string filePath = $"Assets/Stats/{sb}.json";

        stats.StartMeasuring(filePath);
    }

    void UpdateComponentsState()
    {
        foreach (GameObject component in VRComponents)
        {
            component.SetActive(VRMode);
        }
        keyboardMovementController.SetActive(!VRMode);

        xrInput.enabled = VRMode;
        input.enabled = !VRMode;
    }

    void ForcePlayerCollisionUpdate()
    {
        CustomDynamicMoveProvider.onXRMovePositionChange?.Invoke();
    }
}
