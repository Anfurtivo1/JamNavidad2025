using UnityEngine;
using UnityEngine.InputSystem;

public class HorseController : MonoBehaviour
{
    private GUIStyle debugStyle;

    [Header("Componentes")]
    public Rigidbody2D rb;
    public Transform horseVisual; // hijo con el sprite

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

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        currentSpeed = baseSpeed;
    }

    void FixedUpdate()
    {
        // --- Detectar suelo ---
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // --- Mantener Rigidbody sin rotación ---
        rb.rotation = 0f;

        // --- Determinar velocidad objetivo ---
        float targetSpeed = accelerating ? accelSpeed : baseSpeed;
        float maxDelta = speedChangeRate * Time.fixedDeltaTime;

        if (grounded)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, maxDelta);
        }
        else
        {
            // En aire solo perder velocidad suavemente
            if (currentSpeed > targetSpeed)
                currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, maxDelta * airControl);
        }

        // --- Aplicar velocidad horizontal ---
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);

        // --- Rotación visual del sprite ---
        if (horseVisual != null)
        {
            // Ángulo según pendiente / movimiento
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            horseVisual.rotation = Quaternion.Euler(0, 0, angle);
        }
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

    
    void OnGUI()
    {
        debugStyle = new GUIStyle(GUI.skin.label);
        debugStyle.fontSize = 40;
        debugStyle.normal.textColor = Color.white;

        string velocidad = rb.linearVelocity.x.ToString("F2");

        if (velocidad[0] == '-')
        {
            velocidad = "0";
            GUI.Label(
                new Rect(20, 70, 400, 60),
                "¡Estás yendo hacia atrás!",
                debugStyle
            );
            
        }


        GUI.Label(
            new Rect(20, 20, 400, 60),
            $"Speed: {velocidad:F2}",
            debugStyle
        );
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }

}


