using UnityEngine;

public class ZoomManager : MonoBehaviour
{
    public CameraFollow2D camFollow;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            camFollow.ToggleZoom();
        }
    }
}
