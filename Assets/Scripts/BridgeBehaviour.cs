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
            if (value && !audioSource.isPlaying)
                audioSource.Play();
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

    void PlayerStandingOnDisabledBeam()
    {
        forcePlayerPosUpdate?.Invoke();
    }

}
