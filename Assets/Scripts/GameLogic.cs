using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class GameLogic : MonoBehaviour
{
    [SerializeField] int checkpointsTouched = 0;
    [SerializeField] int requiredTouches = 3;
    [SerializeField] VRClickablePanel infoPanel;
    [SerializeField] Timer timer;
    [SerializeField] GameObject environment;

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
    DisappearingBeam[] beams;
    PlayerCollision player;
    bool gameWinState = false;

    void Awake()
    {
        beams = environment.GetComponentsInChildren<DisappearingBeam>();

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
        UpdatePanel();
    }

    void OnGameLost()
    {
        gameWinState = false;
        UpdatePanel();
        RestartGame();
    }

    public void ResetBeams()
    {
        foreach (DisappearingBeam beam in beams)
        {
            beam.EnableBeam();
        }
    }

    void RestartGame()
    {
        ResetBeams();
        player.Restart();
        touchedCheckpoints.Clear();
        timer.ResetTimer();
    }

    void UpdatePanel()
    {
        if (!VRMode)
        {
            return;
        }

        if (gameWinState)
        {
            infoPanel.ShowPanel(winMssg, RestartGame);
        }
        else
        {
            infoPanel.ShowPanel(loseMssg);
        }
    }

    void UpdateComponentsState()
    {
        if (VRMode)
            infoPanel.ShowPanel(startMssg);

        foreach (GameObject component in VRComponents)
        {
            component.SetActive(VRMode);
        }
        keyboardMovementController.SetActive(!VRMode);

        xrInput.enabled = VRMode;
        input.enabled = !VRMode;
    }
}
