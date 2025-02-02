using System.Collections.Generic;
using UnityEngine;

public class BridgeBehaviour : MonoBehaviour
{
    [SerializeField] List<DisappearingBeam> beams;

    bool isPlayerOnBridge = false;

    public bool IsPlayerOnBridge
    {
        get => isPlayerOnBridge;
        set
        {
            if (isPlayerOnBridge == value)
            {
                return;
            }
            isPlayerOnBridge = value;
            UpdateBridgeState(isPlayerOnBridge);
        }
    }

    public void ResetBridgeState()
    {
        foreach (var beam in beams)
        {
            beam?.EnableBeam();
        }
    }

    void UpdateBridgeState(bool isPlayerOnBridge)
    {
        if (!isPlayerOnBridge)
        {
            // reset state - balance the bridge
            return;
        }

        // calculate the angle of inclination of the bridge
        Vector3 leftHandPos = GameLogic.LeftController.transform.position;
        Vector3 rightHandPos = GameLogic.RightController.transform.position;

    }

}
