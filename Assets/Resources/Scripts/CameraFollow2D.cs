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

    [Header("Referencia")]
    public HorseController horse;

    [Header("Offset dinámico")]
    [Range(0f, 2f)] public float offsetMultiplier = 1f; // Este valor se ajusta desde HorseController

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null || horse == null) return;

        // --- Zoom por velocidad ---
        float speed01 = Mathf.InverseLerp(
            horse.minSpeed,
            horse.accelSpeed,
            horse.currentSpeed
        );

        float targetSize = Mathf.Lerp(maxSize, minSize, speed01);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);

        // --- Ajustar offset proporcional al zoom y multiplicador dinámico ---
        Vector3 offsetAdjusted = offset * (cam.orthographicSize / maxSize) * offsetMultiplier;

        // --- Seguimiento centrando al jugador con offset ---
        Vector3 desiredPosition = target.position + offsetAdjusted;
        desiredPosition.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
