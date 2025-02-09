using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Windows;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class CustomDynamicMoveProvider : DynamicMoveProvider
{
    public static Action onXRMovePositionChange;

    // Update is called once per frame
    protected override void Awake()
    {
        base.Awake();
        onXRMovePositionChange += PositionChange;
    }

    private void PositionChange()
    {
        Vector2 input = new Vector2(0, 0f);
        var translationInWorldSpace = ComputeDesiredMove(input);
        MoveRig(translationInWorldSpace);
        TryEndLocomotion();
    }
}
