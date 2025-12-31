using UnityEngine;

public class InfiniteParallax : MonoBehaviour
{
    public Transform cam;
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    float spriteWidth;
    Vector3 lastCamPos;

    void Start()
    {
        if (!cam)
            cam = Camera.main.transform;

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        spriteWidth = sr.bounds.size.x;

        lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPos;
        transform.position += new Vector3(delta.x * parallaxFactor, 0f, 0f);

        float camDeltaX = cam.position.x - transform.position.x;

        if (Mathf.Abs(camDeltaX) >= spriteWidth)
        {
            float offset = camDeltaX > 0 ? spriteWidth : -spriteWidth;
            transform.position += new Vector3(offset, 0f, 0f);
        }

        lastCamPos = cam.position;
    }
}
