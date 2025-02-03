using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class BridgeBehaviour : MonoBehaviour
{
    [SerializeField] List<DisappearingBeam> disappearingBeams;

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

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponentInParent<AudioSource>();
        disappearingBeams = new List<DisappearingBeam>();
        foreach (var beam in GetComponentsInChildren<DisappearingBeam>())
        {
            disappearingBeams.Add(beam);
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

}
