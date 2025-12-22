using UnityEngine;
using UnityEngine.InputSystem;

public class HorseController : MonoBehaviour
{
    [Header("Componentes")]
    public Rigidbody2D rb;
    public Transform horseVisual; // Hijo con el sprite

    [Header("Velocidades")]
    public float baseSpeed = 12f;
    public float accelSpeed = 20f;
    public float speedChangeRate = 18f;
    public float airControl = 0.5f;

    [Header("Salto")]
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    // Estado
    private bool grounded;
    private bool accelerating;
    private float currentSpeed;
    private Vector2 groundNormal = Vector2.up;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        currentSpeed = baseSpeed;
    }

    void FixedUpdate()
    {
        // Detectar suelo
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // Velocidad horizontal suave
        float targetSpeed = accelerating ? accelSpeed : baseSpeed;
        float maxDelta = speedChangeRate * Time.fixedDeltaTime;

        if (grounded)
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, maxDelta);
        else if (currentSpeed > targetSpeed)
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, maxDelta * airControl);

        // Aplicar velocidad horizontal
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    void Update()
    {
        // Rotación visual del sprite según pendiente
        float angle = Mathf.Atan2(groundNormal.y, groundNormal.x) * Mathf.Rad2Deg;
        horseVisual.rotation = Quaternion.Lerp(horseVisual.rotation, Quaternion.Euler(0, 0, angle - 90f), Time.deltaTime * 8f);
    }

    #region Input
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    public void OnAccelerate(InputAction.CallbackContext ctx)
    {
        accelerating = ctx.ReadValueAsButton();
    }
    #endregion

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.collider.CompareTag("Floor"))
            groundNormal = col.contacts[0].normal;
    }
}
