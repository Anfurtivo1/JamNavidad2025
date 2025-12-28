using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Seguimiento")]
    public Transform target;
    public Vector3 offset;             // Offset base
    public float smoothSpeed = 5f;

    [Header("Zoom")]
    public float minSize = 5f;
    public float maxSize = 15f;
    public float zoomSpeed = 2f;
    private float zoomVelocity;

    [Header("Referencia")]
    public HorseController horse;

    [Header("Offset dinámico")]
    [Range(0f, 2f)] public float offsetMultiplier = 1f; // Valor actual
    [HideInInspector] public float targetOffsetMultiplier = 1f; // Valor objetivo
    public float offsetSmoothSpeed = 3f;                  // Velocidad de interpolación del offset
    private Vector3 offsetAdjusted;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null || horse == null) return;

        // --- Zoom por velocidad (suavizado) ---
        float speed01 = Mathf.InverseLerp(horse.minSpeed, horse.accelSpeed, horse.currentSpeed);
        float targetSize = Mathf.Lerp(maxSize, minSize, speed01);

        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref zoomVelocity, 0.3f);

        // --- Interpolamos suavemente el offsetMultiplier ---
        offsetMultiplier = Mathf.Lerp(offsetMultiplier, targetOffsetMultiplier, offsetSmoothSpeed * Time.deltaTime);

        // --- Offset suavizado ---
        Vector3 targetOffset = offset * (cam.orthographicSize / maxSize) * offsetMultiplier;
        offsetAdjusted = Vector3.Lerp(offsetAdjusted, targetOffset, offsetSmoothSpeed * Time.deltaTime);

        // --- Seguimiento centrando al jugador con offset ---
        Vector3 desiredPosition = target.position + offsetAdjusted;
        desiredPosition.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
