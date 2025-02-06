using UnityEngine;

public class NewBallance : MonoBehaviour
{
    [Header("Referencje do kontrolerów")]
    [Tooltip("Transform lewego kontrolera")]
    public Transform leftController;
    [Tooltip("Transform prawego kontrolera")]
    public Transform rightController;

    [Header("Ustawienia przechy³u")]
    [Tooltip("Maksymalny k¹t przechy³u (w stopniach)")]
    public float maxTiltAngle = 20f;
    [Tooltip("Prêdkoœæ przejœcia (interpolacji) do docelowego k¹ta przechy³u")]
    public float tiltSpeed = 5f;
    [Tooltip("Maksymalna oczekiwana ró¿nica odleg³oœci miêdzy kontrolerami (dla normalizacji)")]
    public float maxDiffDistance = 0.5f;

    [Header("Referencja do obiektu, który siê przechyla")]
    [Tooltip("Obiekt (najczêœciej XR Origin lub jego czêœæ), który bêdzie obracany")]
    public Transform playerBody;

    // Mo¿esz ewentualnie ustawiæ punkt odniesienia (np. œrodek postaci) – domyœlnie u¿ywamy pozycji obiektu, do którego do³¹czony jest skrypt
    [Tooltip("Punkt odniesienia, od którego liczone s¹ odleg³oœci kontrolerów. Jeœli nie ustawisz, przyjmie siê pozycja obiektu.")]
    public Transform referencePoint;

    void Start()
    {
        if (referencePoint == null)
            referencePoint = transform; // jeœli nie ustawiono, u¿yj tego obiektu

        if (playerBody == null)
            playerBody = transform; // domyœlnie obracamy obiekt, do którego do³¹czony jest skrypt
    }

    void Update()
    {
        // Obliczamy wektory od punktu odniesienia do kontrolerów
        Vector3 leftVector = leftController.position - referencePoint.position;
        Vector3 rightVector = rightController.position - referencePoint.position;

        // Rzutujemy wektory na p³aszczyznê poziom¹ (usuwamy sk³adow¹ Y)
        leftVector.y = 0;
        rightVector.y = 0;

        // Obliczamy odleg³oœci
        float leftDistance = leftVector.magnitude;
        float rightDistance = rightVector.magnitude;

        // Ró¿nica – jeœli np. prawy kontroler jest dalej, to rightDistance - leftDistance > 0
        float diff = rightDistance - leftDistance;

        // Normalizujemy ró¿nicê (przyjmujemy, ¿e maxDiffDistance to wartoœæ odpowiadaj¹ca maksymalnemu przechy³owi)
        float normalizedDiff = Mathf.Clamp(diff / maxDiffDistance, -1f, 1f);

        // Obliczamy docelowy k¹t przechy³u – dodatni k¹t przechy³u oznacza np. przechylenie w prawo (mo¿esz zmieniæ interpretacjê osi)
        float targetTiltAngle = normalizedDiff * maxTiltAngle;

        // Pobieramy bie¿¹cy k¹t przechy³u wokó³ osi Z
        float currentTilt = playerBody.localEulerAngles.z;
        // Ujednolicenie – Unity zwraca k¹ty 0-360, wiêc zamieniamy na zakres -180 do 180
        if (currentTilt > 180f)
            currentTilt -= 360f;

        // Interpolujemy k¹t bie¿¹cy do docelowego
        float newTilt = Mathf.Lerp(currentTilt, targetTiltAngle, Time.deltaTime * tiltSpeed);

        // Ustawiamy nowy k¹t obrotu – obracamy tylko wokó³ osi Z, pozostawiaj¹c X i Y bez zmian
        Vector3 newRotation = playerBody.localEulerAngles;
        newRotation.z = newTilt;
        playerBody.localEulerAngles = newRotation;
    }
}
