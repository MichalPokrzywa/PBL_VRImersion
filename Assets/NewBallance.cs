using UnityEngine;

public class NewBallance : MonoBehaviour
{
    [Header("Referencje do kontroler�w")]
    [Tooltip("Transform lewego kontrolera")]
    public Transform leftController;
    [Tooltip("Transform prawego kontrolera")]
    public Transform rightController;

    [Header("Ustawienia przechy�u")]
    [Tooltip("Maksymalny k�t przechy�u (w stopniach)")]
    public float maxTiltAngle = 20f;
    [Tooltip("Pr�dko�� przej�cia (interpolacji) do docelowego k�ta przechy�u")]
    public float tiltSpeed = 5f;
    [Tooltip("Maksymalna oczekiwana r�nica odleg�o�ci mi�dzy kontrolerami (dla normalizacji)")]
    public float maxDiffDistance = 0.5f;

    [Header("Referencja do obiektu, kt�ry si� przechyla")]
    [Tooltip("Obiekt (najcz�ciej XR Origin lub jego cz��), kt�ry b�dzie obracany")]
    public Transform playerBody;

    // Mo�esz ewentualnie ustawi� punkt odniesienia (np. �rodek postaci) � domy�lnie u�ywamy pozycji obiektu, do kt�rego do��czony jest skrypt
    [Tooltip("Punkt odniesienia, od kt�rego liczone s� odleg�o�ci kontroler�w. Je�li nie ustawisz, przyjmie si� pozycja obiektu.")]
    public Transform referencePoint;

    void Start()
    {
        if (referencePoint == null)
            referencePoint = transform; // je�li nie ustawiono, u�yj tego obiektu

        if (playerBody == null)
            playerBody = transform; // domy�lnie obracamy obiekt, do kt�rego do��czony jest skrypt
    }

    void Update()
    {
        // Obliczamy wektory od punktu odniesienia do kontroler�w
        Vector3 leftVector = leftController.position - referencePoint.position;
        Vector3 rightVector = rightController.position - referencePoint.position;

        // Rzutujemy wektory na p�aszczyzn� poziom� (usuwamy sk�adow� Y)
        leftVector.y = 0;
        rightVector.y = 0;

        // Obliczamy odleg�o�ci
        float leftDistance = leftVector.magnitude;
        float rightDistance = rightVector.magnitude;

        // R�nica � je�li np. prawy kontroler jest dalej, to rightDistance - leftDistance > 0
        float diff = rightDistance - leftDistance;

        // Normalizujemy r�nic� (przyjmujemy, �e maxDiffDistance to warto�� odpowiadaj�ca maksymalnemu przechy�owi)
        float normalizedDiff = Mathf.Clamp(diff / maxDiffDistance, -1f, 1f);

        // Obliczamy docelowy k�t przechy�u � dodatni k�t przechy�u oznacza np. przechylenie w prawo (mo�esz zmieni� interpretacj� osi)
        float targetTiltAngle = normalizedDiff * maxTiltAngle;

        // Pobieramy bie��cy k�t przechy�u wok� osi Z
        float currentTilt = playerBody.localEulerAngles.z;
        // Ujednolicenie � Unity zwraca k�ty 0-360, wi�c zamieniamy na zakres -180 do 180
        if (currentTilt > 180f)
            currentTilt -= 360f;

        // Interpolujemy k�t bie��cy do docelowego
        float newTilt = Mathf.Lerp(currentTilt, targetTiltAngle, Time.deltaTime * tiltSpeed);

        // Ustawiamy nowy k�t obrotu � obracamy tylko wok� osi Z, pozostawiaj�c X i Y bez zmian
        Vector3 newRotation = playerBody.localEulerAngles;
        newRotation.z = newTilt;
        playerBody.localEulerAngles = newRotation;
    }
}
