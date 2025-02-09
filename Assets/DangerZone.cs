using UnityEngine;
public class DangerZone : MonoBehaviour
{
    enum PlaceType
    {
        NearDangerous,
        Dangerous
    }

    [SerializeField] Collider col;
    [SerializeField] PlaceType type = PlaceType.NearDangerous;

    void OnTriggerEnter(Collider other)
    {
        if (type == PlaceType.NearDangerous)
        {
            MovementStats.onNearDangerousPlaceEnter?.Invoke();
        }
        else
        {
            MovementStats.onDangerousPlaceEnter?.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (type == PlaceType.NearDangerous)
        {
            MovementStats.onNearDangerousPlaceExit?.Invoke();
        }
    }
}
