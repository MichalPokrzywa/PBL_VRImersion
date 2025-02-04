using System;
using System.Collections.Generic;
using UnityEngine;

public class BridgeBehaviour : MonoBehaviour
{
    [SerializeField] List<DisappearingBeam> disappearingBeams;

    AudioSource audioSource;
    bool isPlayerOnBridge = false;

    public Action forcePlayerPosUpdate;

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


    void Awake()
    {
        audioSource = GetComponentInParent<AudioSource>();
        disappearingBeams = new List<DisappearingBeam>();
        foreach (var beam in GetComponentsInChildren<DisappearingBeam>())
        {
            disappearingBeams.Add(beam);
            beam.playerStandingOnDisabledBeam += PlayerStandingOnDisabledBeam;
        }
    }

    public void ResetBridgeState()
    {
        foreach (var beam in disappearingBeams)
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

        if (!audioSource.isPlaying)
            audioSource.Play();

        // calculate the angle of inclination of the bridge
        Vector3 leftHandPos = GameLogic.LeftController.transform.position;
        Vector3 rightHandPos = GameLogic.RightController.transform.position;
    }

    void PlayerStandingOnDisabledBeam()
    {
        forcePlayerPosUpdate?.Invoke();
    }

}
