using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [SerializeField] int checkpointsTouched = 0;
    [SerializeField] int requiredTouches = 3;
    [SerializeField] PlayerCollision player;
    [SerializeField] VRClickablePanel infoPanel;
    [SerializeField] Timer timer;

    const string startMssg = "Dotrzyj do wszystkich checkpointów w ustalonym czasie. Powodzenia!";
    const string winMssg = "Gratulacje! Wygra³eœ!";
    const string loseMssg = "Przegra³eœ! Spróbuj jeszcze raz.";

    HashSet<int> touchedCheckpoints = new HashSet<int>();
    bool gameEnded = false;

    void Start()
    {
        player.onCollisionWithCheckpoint += OnCheckpointTouched;
        player.onCollisionWithWater += OnGameLost;
        timer.OnTimerEnd += OnGameLost;

        infoPanel.ShowPanel(startMssg);
    }

    private void OnDestroy()
    {
        player.onCollisionWithCheckpoint -= OnCheckpointTouched;
        player.onCollisionWithWater -= OnGameLost;
        timer.OnTimerEnd -= OnGameLost;
    }

    void OnCheckpointTouched(int checkpointId)
    {
        if (!touchedCheckpoints.Contains(checkpointId))
        {
            touchedCheckpoints.Add(checkpointId);
            checkpointsTouched++;
            Debug.Log($"Touches: {checkpointsTouched}/{requiredTouches}");

            // Win condition
            if (checkpointsTouched >= requiredTouches)
            {
                gameEnded = true;
                UpdatePanel();
            }
        }
    }

    void OnGameLost()
    {
        gameEnded = false;
        UpdatePanel();
    }

    void UpdatePanel()
    {
        if (gameEnded)
        {
            infoPanel.ShowPanel(winMssg);
        }
        else
        {
            infoPanel.ShowPanel(loseMssg);
        }
    }
}
