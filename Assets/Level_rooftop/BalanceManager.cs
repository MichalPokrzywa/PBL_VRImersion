using UnityEngine;

public class BalanceManager : MonoBehaviour
{
    public Transform head; // HMD (g�owa)
    public Transform leftController; // Lewy kontroler
    public Transform rightController; // Prawy kontroler
    public Transform xrOrigin; // XR Origin (ca�y gracz w �wiecie)

    [Header("Balansowanie")]
    public float balanceThreshold = 0.5f; // Pr�g utraty r�wnowagi
    public float swayMultiplier = 0.15f; // Jak bardzo przechyla

    [Header("Rotacja przy upadku")]
    public float fallRotationSpeed = 5f; // Jak szybko gracz si� przewraca
    public float maxTiltAngle = 45f; // Maksymalny k�t przechy�u (stopnie)
    public float fallMoveSpeed = 0.05f;

    private Vector3 initialPosition; // Pozycja startowa XR Origin
    private Quaternion initialRotation; // Rotacja startowa XR Origin
    private float currentTiltAngle = 0f; // Aktualny przechy�
    //private bool isOnPlank;

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

        // Oblicz, o ile stopni zwi�kszamy przechy� w tej klatce
        float tiltStep = fallRotationSpeed * Time.deltaTime;

        // Ogranicz k�t przechy�u do maxTiltAngle
        currentTiltAngle = Mathf.Clamp(currentTiltAngle + tiltStep, 0, maxTiltAngle);

        // Tworzymy rotacj� tylko wok� osi Z (przechylenie)
        Quaternion tiltRotation = Quaternion.Euler(0, 0, -tiltDirection * tiltStep);

        if(maxTiltAngle * Mathf.Deg2Rad <= Mathf.Abs(xrOrigin.localRotation.z) && tiltDirection * xrOrigin.localRotation.z > 0)
           tiltRotation = Quaternion.Euler(0, 0, 0);
        // Mno�ymy now� rotacj� przez aktualn�, aby doda� przechy� zamiast go nadpisywa�
        xrOrigin.localRotation = tiltRotation * xrOrigin.localRotation;

        // Przesuni�cie w stron� upadku
        Vector3 fallOffset = new Vector3(tiltDirection * fallMoveSpeed * Time.deltaTime, 0, 0);
        xrOrigin.localPosition += fallOffset;

        CustomDynamicMoveProvider.onXRMovePositionChange?.Invoke();
    }

    void ResetBalance()
    {
        if(xrOrigin.localRotation.z ==0) return;

        float tiltStep = fallRotationSpeed * Time.deltaTime;
        currentTiltAngle = Mathf.Max(currentTiltAngle - tiltStep, 0);
        
        Quaternion correctionRotation = Quaternion.Euler(0, 0, (currentTiltAngle > 0 ? 1 : -1) * tiltStep);
        
        xrOrigin.localRotation = Quaternion.Slerp(xrOrigin.localRotation, correctionRotation * xrOrigin.localRotation, Time.deltaTime * 5f);

    }

}
