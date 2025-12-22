using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Seguido")]
    public Transform target;       
    public Vector3 offset;         
    public float smoothSpeed = 5f; 

    [Header("Zoom")]
    public float normalSize = 5f;  
    public float zoomedOutSize = 15f; 
    public float zoomSpeed = 2f;
    public HorseController horse;

    private Camera cam;
    private bool zoomedOut;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = normalSize;
    }

    void LateUpdate()
    {
        zoomedOut = horse.isZooming; // para detectar si está decelerando o no
        if (target == null) return;

        // --- Seguimiento ---
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // --- Zoom smooth ---
        float targetSize = zoomedOut ? zoomedOutSize : normalSize;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
    }

    // --- Función toggle --- No utilizado
    public void ToggleZoom()
    {
        zoomedOut = !zoomedOut;
    }

    // --- Opcional: forzar un estado específico ---
    public void SetZoom(bool value)
    {
        zoomedOut = value;
    }
}
