using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HorseController : MonoBehaviour
{
    [Header("Componentes")]
    public Rigidbody2D rb;
    public Transform horseVisual;

    [Header("Velocidades")]
    public float baseSpeed = 15f;
    public float accelSpeed = 25f;
    public float decelSpeed = 6f; // velocidad de frenado / zoom
    public float speedChangeRate = 18f;
    public float airControl = 0.5f;

    [Header("Velocidad mínima absoluta")]
    public float minSpeed = 6f;

    [Header("Salto")]
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    [Header("Estado")]
    private bool grounded;
    private bool accelerating;
    private bool decelerating;
    public bool isZooming;
    public float currentSpeed;

    private Vector2 groundNormal = Vector2.up;

    [Header("Hit")]
    public float hitDuration = 0.5f;
    public float blinkInterval = 0.1f;
    public float hitSpeedMultiplier = 0.5f;
    public float minHitSpeed = 4f;     // Nunca baja de esto
    public float hitSlowMultiplier = 0.85f; // Qué porcentaje quita al máximo


    private bool hitLocked = false;
    private SpriteRenderer spriteRenderer;

    public bool levelPassed = false;

    void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        spriteRenderer = horseVisual.GetComponent<SpriteRenderer>();
        currentSpeed = baseSpeed;

        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        isZooming = false;

        grounded = Physics2D.OverlapCircle(groundCheck.position,groundRadius,groundLayer);

        float targetSpeed = baseSpeed;

        if (accelerating && !hitLocked)
        {
            targetSpeed = accelSpeed;
        }
        else if (decelerating && !hitLocked)
        {
            targetSpeed = decelSpeed;
            isZooming = true;
        }
        else
        {
            targetSpeed = baseSpeed;
        }

        if (hitLocked)
        {
            targetSpeed *= hitSpeedMultiplier;
        }

        float maxDelta = speedChangeRate * Time.fixedDeltaTime;

        if (grounded)
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                targetSpeed,
                maxDelta
            );
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                targetSpeed,
                maxDelta * airControl
            );
        }

        currentSpeed = Mathf.Max(currentSpeed, minSpeed);

        Vector2 velocity = rb.linearVelocity;
        velocity.x = currentSpeed;
        rb.linearVelocity = velocity;
    }

    void Update()
    {
        float angle = Mathf.Atan2(
            groundNormal.y,
            groundNormal.x
        ) * Mathf.Rad2Deg;

        horseVisual.rotation = Quaternion.Lerp(
            horseVisual.rotation,
            Quaternion.Euler(0, 0, angle - 90f),
            Time.deltaTime * 8f
        );
    }

    #region Input

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && grounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
        }
    }

    public void OnAccelerate(InputAction.CallbackContext ctx)
    {
        accelerating = ctx.ReadValueAsButton();
    }

    public void OnDecelerate(InputAction.CallbackContext ctx)
    {
        decelerating = ctx.ReadValueAsButton();
        Debug.Log("Deceleramos");
    }

    public void OnReset(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );

        Time.timeScale = 1f;
    }

    #endregion

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.collider.CompareTag("Floor"))
        {
            groundNormal = col.contacts[0].normal;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Obstacle") && !hitLocked)
        {
            StartCoroutine(HitRoutine());
            Destroy(collision.collider.gameObject);
        }
        
        if (collision.collider.CompareTag("NextLevel"))
        {
            //SceneManager.LoadScene(1);
            levelPassed = true;
        }
    }

    IEnumerator HitRoutine()
    {
        hitLocked = true;

        // Normalizamos velocidad (0 → 1)
        float speed01 = Mathf.InverseLerp(baseSpeed, accelSpeed, currentSpeed);

        // Cuanto más rápido, más castigo
        float slowFactor = Mathf.Lerp(0.25f, hitSlowMultiplier, speed01);

        // Aplicar reducción
        currentSpeed *= (1f - slowFactor);

        // Nunca totalmente parado
        currentSpeed = Mathf.Max(currentSpeed, minHitSpeed);

        float elapsed = 0f;

        while (elapsed < hitDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        spriteRenderer.enabled = true;
        hitLocked = false;
    }


    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundRadius
        );
    }
}
