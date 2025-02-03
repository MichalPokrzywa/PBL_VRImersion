using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class MovementStats : MonoBehaviour
{
    public GameObject xr;
    public GameObject eye;

    public static Action onNearDangerousPlaceEnter;
    public static Action onNearDangerousPlaceExit;
    public static Action onDangerousPlaceEnter;

    bool isMeasuring = false;
    string fileName = "Assets/movement_stats.json";

    // 1. Basic movement measurements
    float startTime;
    float endTime;
    List<Sample> samples = new List<Sample>();
    Vector3 lastPosition;
    Quaternion lastRotation;
    Quaternion lastHeadRotation;

    // 2. Movement efficiency
    List<float> dirChangeMovementResumeTime = new List<float>(); // TODO

    // 3. Movement consistency
    List<PauseData> pauseData = new List<PauseData>();
    float pauseTimer = 0;
    float velocityLastFrame = 0;

    // 4. Reaction time
    List<PauseData> teleportMovementResumeTime = new List<PauseData>();
    bool teleported = false;
    bool resetPositionAfterTeleport = false;
    float teleportTimer = 0;
    List<PauseData> dangerousPlaceMovementResumeTime = new List<PauseData>();
    float timeNeededToEnterDangerousPlace = 0;
    bool nearDangerousPlaceEntered = false;

    [Serializable]
    struct Sample
    {
        public float time;
        public MovementData movement;
        public RotationData rotation;
    }

    [Serializable]
    struct MovementData
    {
        public Vector3 position;
        public float distance;
        public Vector3 velocity;
    }

    [Serializable]
    struct RotationData
    {
        public float bodyRotationAngle; // [deg]
        public float headRotationAngle; // [deg]
        public float bodyRotationSpeed; // [deg/s]
        public float headRotationSpeed; // [deg/s]
    }

    [Serializable]
    struct PauseData
    {
        public float timestamp; // [s]
        public float duration;  // [s]
    }

    [Serializable]
    struct FileData
    {
        public string date;
        public float timeDuration;
        public float maxVelocity;
        public float avgVelocity;
        public float totalHeadRotation;
        public float totalBodyRotation;
        public float maxHeadRotationSpeed;
        public float maxBodyRotationSpeed;
        public float avgHeadRotationSpeed;
        public float avgBodyRotationSpeed;
        public List<PauseData> pauseData;
        public List<PauseData> teleportResumeTime;
        public List<PauseData> dangerousPlaceResumeTime;
        public List<Sample> samples;
    }

    void Start()
    {
        ResetState();
        CustomTeleportationProvider.OnTeleported += OnTeleport;
        onNearDangerousPlaceEnter += OnNearDangerousPlaceEntered;
        onNearDangerousPlaceExit += OnNearDangerousPlaceExited;
        onDangerousPlaceEnter += OnDangerousPlaceEntered;
    }

    // Call it when user started game
    public void StartMeasuring(string fileName)
    {
        ResetState();
        startTime = Time.time;
        isMeasuring = true;
        this.fileName = fileName;
    }

    // Call it when user finished game
    public void StopMeasuring()
    {
        endTime = Time.time;
        isMeasuring = false;
        SerializeData();
    }

    #region Reaction on events
    void OnTeleport()
    {
        if (!isMeasuring)
            return;

        teleported = true;
        resetPositionAfterTeleport = true;
        teleportTimer = 0;
        Debug.Log("Teleport");
    }

    void OnDangerousPlaceEntered()
    {
        if (!isMeasuring)
            return;

        dangerousPlaceMovementResumeTime.Add(new PauseData
        {
            timestamp = GetCurrentTime() - timeNeededToEnterDangerousPlace,
            duration = timeNeededToEnterDangerousPlace,
        });
        Debug.Log("<color=green>Dangerous place time: </color>" + timeNeededToEnterDangerousPlace);
    }

    void OnNearDangerousPlaceEntered()
    {
        if (!isMeasuring)
            return;

        timeNeededToEnterDangerousPlace = 0;
        nearDangerousPlaceEntered = true;
        Debug.Log("<color=red>Near dangerous place</color>");
    }

    void OnNearDangerousPlaceExited()
    {
        if (!isMeasuring)
            return;

        nearDangerousPlaceEntered = false;
        Debug.Log("<color=orange>Exit near dangerous place</color>");
    }
    #endregion

    void Update()
    {
        if (!isMeasuring)
            return;

        if (resetPositionAfterTeleport)
        {
            lastPosition = xr.transform.position;
            resetPositionAfterTeleport = false;
        }

        var velocity = (xr.transform.position - lastPosition) / Time.deltaTime;
        var bodyRotation = Quaternion.Angle(lastRotation, xr.transform.rotation);
        var headRotation = Quaternion.Angle(lastHeadRotation, eye.transform.rotation);

        samples.Add(new Sample
        {
            time = GetCurrentTime(),
            movement = new MovementData
            {
                position = xr.transform.position,
                distance = Vector3.Distance(lastPosition, xr.transform.position),
                velocity = velocity,
            },
            rotation = new RotationData
            {
                bodyRotationAngle = bodyRotation,
                headRotationAngle = headRotation,
                bodyRotationSpeed = bodyRotation / Time.deltaTime,
                headRotationSpeed = headRotation / Time.deltaTime,
            }
        });

        CalcPauseInMovement();
        CalcTimeNeededAfterTeleport();
        CalcTimeNeededAfterDangerousPlaceEnter();

        lastPosition = xr.transform.position;
        lastRotation = xr.transform.rotation;
        lastHeadRotation = eye.transform.rotation;
    }

    void CalcPauseInMovement()
    {
        float displacement = Vector3.Distance(lastPosition, xr.transform.position);
        float velocity = displacement / Time.deltaTime;

        const float movementThreshold = 0.002f;
        const float minPauseDuration = 0.5f;

        // If there is no movement
        if (velocity < movementThreshold)
        {
            pauseTimer += Time.deltaTime;
        }
        // If we resumed movement
        else if (velocityLastFrame < movementThreshold && pauseTimer > minPauseDuration)
        {
            // Save pause data
            pauseData.Add(new PauseData
            {
                timestamp = GetCurrentTime() - pauseTimer,
                duration = pauseTimer,
            });

            // If we resumed movement after teleport
            if (teleported)
            {
                teleportMovementResumeTime.Add(new PauseData
                {
                    timestamp = GetCurrentTime() - teleportTimer,
                    duration = teleportTimer,
                });
                teleported = false;
                Debug.Log("Movement resumption time after teleport: " + teleportTimer);
            }

            Debug.Log("Movement resumption time: " + pauseTimer);
            pauseTimer = 0;
        }

        velocityLastFrame = velocity;
    }

    void CalcTimeNeededAfterTeleport()
    {
        if (teleported)
        {
            teleportTimer += Time.deltaTime;
        }
    }

    void CalcTimeNeededAfterDangerousPlaceEnter()
    {
        if (nearDangerousPlaceEntered)
        {
            timeNeededToEnterDangerousPlace += Time.deltaTime;
        }
    }

    void ResetState()
    {
        lastHeadRotation = eye.transform.rotation;
        lastPosition = xr.transform.position;
        lastRotation = xr.transform.rotation;

        pauseTimer = 0;
        teleportTimer = 0;
        timeNeededToEnterDangerousPlace = 0;
        velocityLastFrame = 0;

        nearDangerousPlaceEntered = false;
        teleported = false;
        resetPositionAfterTeleport = false;

        samples = new List<Sample>();
        pauseData = new List<PauseData>();
        teleportMovementResumeTime = new List<PauseData>();
        dangerousPlaceMovementResumeTime = new List<PauseData>();
    }

    float GetCurrentTime()
    {
        return Time.time - startTime;
    }

    void SerializeData()
    {
        var fileData = new FileData
        {
            date = DateTime.Now.ToString(),
            timeDuration = endTime - startTime,
            maxVelocity = samples.Max(s => s.movement.velocity.magnitude),
            avgVelocity = samples.Average(s => s.movement.velocity.magnitude),
            totalHeadRotation = samples.Sum(s => s.rotation.headRotationAngle),
            totalBodyRotation = samples.Sum(s => s.rotation.bodyRotationAngle),
            maxHeadRotationSpeed = samples.Max(s => s.rotation.headRotationSpeed),
            maxBodyRotationSpeed = samples.Max(s => s.rotation.bodyRotationSpeed),
            avgHeadRotationSpeed = samples.Average(s => s.rotation.headRotationSpeed),
            avgBodyRotationSpeed = samples.Average(s => s.rotation.bodyRotationSpeed),
            pauseData = pauseData,
            teleportResumeTime = teleportMovementResumeTime,
            dangerousPlaceResumeTime = dangerousPlaceMovementResumeTime,
            samples = samples,
        };

        var json = JsonUtility.ToJson(fileData);
        System.IO.File.WriteAllText(fileName, json);
    }
}

