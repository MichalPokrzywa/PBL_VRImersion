using UnityEngine;

public class BalanceManager : MonoBehaviour
{
    public Transform head; // HMD (g³owa)
    public Transform leftController; // Lewy kontroler
    public Transform rightController; // Prawy kontroler
    public Transform xrOrigin; // XR Origin (ca³y gracz w œwiecie)

    [Header("Balansowanie")]
    public float balanceThreshold = 0.5f; // Próg utraty równowagi
    public float swayMultiplier = 0.1f; // Jak bardzo przechyla
    public float instabilityNoise = 0.02f; // Mikrodrgania

    [Header("Rotacja przy upadku")]
    public float fallRotationSpeed = 5f; // Jak szybko gracz siê przewraca
    public float maxTiltAngle = 45f; // Maksymalny k¹t przechy³u (stopnie)
    public float fallMoveSpeed = 0.05f;

    private Vector3 initialPosition; // Pozycja startowa XR Origin
    private Quaternion initialRotation; // Rotacja startowa XR Origin
    private float currentTiltAngle = 0f; // Aktualny przechy³

    void Start()
    {
        if (xrOrigin != null)
        {
            initialPosition = xrOrigin.localPosition;
            initialRotation = xrOrigin.localRotation;
        }
    }

    void LateUpdate()
    {
        if (head == null || leftController == null || rightController == null || xrOrigin == null)
            return;

        // Oblicz œrodek miêdzy kontrolerami
        Vector3 handCenter = (leftController.localPosition + rightController.localPosition) / 2f;

        // Oblicz wektor równowagi
        Vector3 balanceVector = head.localPosition - handCenter;

        // Oblicz przesuniêcie na podstawie balansu
        Vector3 swayOffset = new Vector3(balanceVector.x * swayMultiplier, 0, balanceVector.z * swayMultiplier);

        // Dodaj losowe mikrodrgania
        Vector3 noise = new Vector3(
            Mathf.PerlinNoise(Time.time, 0) - 0.5f,
            0,
            Mathf.PerlinNoise(0, Time.time) - 0.5f
        ) * instabilityNoise;

        // Przesuñ XR Origin (lekkie bujanie)
        // xrOrigin.localPosition += swayOffset + noise;

        // SprawdŸ, czy gracz traci równowagê
        if (balanceVector.magnitude > balanceThreshold)
        {
            ApplyFall(balanceVector);
        }
        else
        {
            ResetBalance();
        }
    }

    void ApplyFall(Vector3 balanceVector)
    {
        Debug.Log("Gracz traci równowagê!");

        // Kierunek przechy³u na podstawie pozycji g³owy wzglêdem œrodka kontrolerów
        float tiltDirection = balanceVector.x > 0 ? 1f : -1f; // 1 = przechy³ w prawo, -1 = przechy³ w lewo

        // Oblicz docelowy k¹t przechy³u (tylko na osi Z)
        float targetTilt = Mathf.Min(currentTiltAngle + fallRotationSpeed * Time.deltaTime, maxTiltAngle);

        // Ustaw rotacjê tylko na osi Z (przechylanie na boki)
        Quaternion targetRotation = Quaternion.Euler(0, 0, -tiltDirection * targetTilt);
        xrOrigin.localRotation = Quaternion.Slerp(xrOrigin.localRotation, targetRotation, Time.deltaTime);

        // Przesuniêcie w stronê upadku
        Vector3 fallOffset = new Vector3(tiltDirection * fallMoveSpeed * Time.deltaTime, 0, 0);
        xrOrigin.localPosition += fallOffset;

        currentTiltAngle = targetTilt;
    }

    void ResetBalance()
    {
        // Stopniowy powrót do normalnej pozycji
        xrOrigin.localRotation = Quaternion.Slerp(xrOrigin.localRotation, initialRotation, Time.deltaTime);
        currentTiltAngle = Mathf.Max(currentTiltAngle - fallRotationSpeed * Time.deltaTime, 0);
    }
}
