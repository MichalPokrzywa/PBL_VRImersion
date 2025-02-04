using UnityEngine;

public class BalanceManager : MonoBehaviour
{
    public Transform head; // HMD (g�owa)
    public Transform leftController; // Lewy kontroler
    public Transform rightController; // Prawy kontroler
    public Transform xrOrigin; // XR Origin (ca�y gracz w �wiecie)

    [Header("Balansowanie")]
    public float balanceThreshold = 0.5f; // Pr�g utraty r�wnowagi
    public float swayMultiplier = 0.1f; // Jak bardzo przechyla
    public float instabilityNoise = 0.02f; // Mikrodrgania

    [Header("Rotacja przy upadku")]
    public float fallRotationSpeed = 5f; // Jak szybko gracz si� przewraca
    public float maxTiltAngle = 45f; // Maksymalny k�t przechy�u (stopnie)
    public float fallMoveSpeed = 0.05f;

    private Vector3 initialPosition; // Pozycja startowa XR Origin
    private Quaternion initialRotation; // Rotacja startowa XR Origin
    private float currentTiltAngle = 0f; // Aktualny przechy�

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

        // Oblicz �rodek mi�dzy kontrolerami
        Vector3 handCenter = (leftController.localPosition + rightController.localPosition) / 2f;

        // Oblicz wektor r�wnowagi
        Vector3 balanceVector = head.localPosition - handCenter;

        // Oblicz przesuni�cie na podstawie balansu
        Vector3 swayOffset = new Vector3(balanceVector.x * swayMultiplier, 0, balanceVector.z * swayMultiplier);

        // Dodaj losowe mikrodrgania
        Vector3 noise = new Vector3(
            Mathf.PerlinNoise(Time.time, 0) - 0.5f,
            0,
            Mathf.PerlinNoise(0, Time.time) - 0.5f
        ) * instabilityNoise;

        // Przesu� XR Origin (lekkie bujanie)
        // xrOrigin.localPosition += swayOffset + noise;

        // Sprawd�, czy gracz traci r�wnowag�
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
        Debug.Log("Gracz traci r�wnowag�!");

        // Kierunek przechy�u na podstawie pozycji g�owy wzgl�dem �rodka kontroler�w
        float tiltDirection = balanceVector.x > 0 ? 1f : -1f; // 1 = przechy� w prawo, -1 = przechy� w lewo

        // Oblicz docelowy k�t przechy�u (tylko na osi Z)
        float targetTilt = Mathf.Min(currentTiltAngle + fallRotationSpeed * Time.deltaTime, maxTiltAngle);

        // Ustaw rotacj� tylko na osi Z (przechylanie na boki)
        Quaternion targetRotation = Quaternion.Euler(0, 0, -tiltDirection * targetTilt);
        xrOrigin.localRotation = Quaternion.Slerp(xrOrigin.localRotation, targetRotation, Time.deltaTime);

        // Przesuni�cie w stron� upadku
        Vector3 fallOffset = new Vector3(tiltDirection * fallMoveSpeed * Time.deltaTime, 0, 0);
        xrOrigin.localPosition += fallOffset;

        currentTiltAngle = targetTilt;
    }

    void ResetBalance()
    {
        // Stopniowy powr�t do normalnej pozycji
        xrOrigin.localRotation = Quaternion.Slerp(xrOrigin.localRotation, initialRotation, Time.deltaTime);
        currentTiltAngle = Mathf.Max(currentTiltAngle - fallRotationSpeed * Time.deltaTime, 0);
    }
}
