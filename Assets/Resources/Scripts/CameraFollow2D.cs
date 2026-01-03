using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Seguimiento")]
    public Transform target;
    public Vector3 offset;           
    public float smoothSpeed = 5f;

    [Header("Zoom")]
    public float minSize = 5f;
    public float regSize = 10f;
    public float maxSize = 20f;
    public float zoomSpeed = 2f;
    private float zoomVelocity;

    [Header("Referencia")]
    public HorseController horse;

    [Header("Offset dinámico")]
    [Range(0f, 2f)] public float offsetMultiplier = 1f; 
    [HideInInspector] public float targetOffsetMultiplier = 1f; 
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

        // ZOOM POR VELOCIDAD (POR TRAMOS)

        float speed = Mathf.Clamp(
            horse.currentSpeed,
            horse.decelSpeed,
            horse.accelSpeed
        );

        float targetSize;

        if (speed <= horse.baseSpeed)
        {
            // decelSpeed a maxSize
            // baseSpeed a regSize
            float t = Mathf.InverseLerp(
                horse.decelSpeed,
                horse.baseSpeed,
                speed
            );
            targetSize = Mathf.Lerp(maxSize, regSize, t);
        }
        else
        {
            // baseSpeed a regSize
            // accelSpeed a minSize
            float t = Mathf.InverseLerp(
                horse.baseSpeed,
                horse.accelSpeed,
                speed
            );
            targetSize = Mathf.Lerp(regSize, minSize, t);
        }

        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetSize, ref zoomVelocity, 0.3f);


        // OFFSET DINÁMICO
        offsetMultiplier = Mathf.Lerp(offsetMultiplier, targetOffsetMultiplier, offsetSmoothSpeed * Time.deltaTime);

        Vector3 targetOffset = offset * (cam.orthographicSize / maxSize) * offsetMultiplier;
        offsetAdjusted = Vector3.Lerp(offsetAdjusted, targetOffset, offsetSmoothSpeed * Time.deltaTime);

        // SEGUIMIENTO DE CÁMARA

        Vector3 desiredPosition = target.position + offsetAdjusted;
        desiredPosition.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }

}
