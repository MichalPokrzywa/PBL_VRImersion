using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    Checkpoint[] checkpoints;

    void Awake()
    {
        checkpoints = GetComponentsInChildren<Checkpoint>();
    }

    public void ActivateCheckpoints()
    {
        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.ActivateObject();
        }
    }
}
