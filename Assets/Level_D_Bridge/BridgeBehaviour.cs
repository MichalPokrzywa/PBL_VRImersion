using System;
using System.Collections.Generic;
using UnityEngine;

public class BridgeBehaviour : MonoBehaviour
{
    [SerializeField] List<DisappearingBeam> disappearingBeams;

    AudioSource audioSource;
    bool isPlayerOnBridge = false;

    public Action OnStandingBeamDisappear;

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
            beam.OnStandingBeamDisappear += PlayerStandingOnDisabledBeam;
        }
    }

    void OnDestroy()
    {
        foreach (var beam in disappearingBeams)
        {
            if (beam != null)
            {
                beam.OnStandingBeamDisappear -= PlayerStandingOnDisabledBeam;
            }
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
        OnStandingBeamDisappear?.Invoke();
    }

}
