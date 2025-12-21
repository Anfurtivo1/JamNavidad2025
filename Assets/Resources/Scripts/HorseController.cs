using UnityEngine;
using UnityEngine.InputSystem;

public class HorseController : MonoBehaviour
{
    Rigidbody2D rb;
    GUIStyle debugStyle;

    [Header("Velocidades")]
    public float baseSpeed = 12f;     // Velocidad mínima constante
    public float accelSpeed = 20f;    // Velocidad al acelerar
    public float speedChangeRate = 18f; // Qué tan rápido se ajusta la velocidad
    public float airControl = 0.5f;   // Control en el aire
    private float currentSpeed;


    [Header("Salto")]
    public float jumpForce = 12f;

    [Header("Estado")]
    public bool grounded;
    public bool accelerating;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = baseSpeed; // Inicializamos con la velocidad base
    }

    void FixedUpdate()
    {
        // Determinar la velocidad objetivo
        float targetSpeed = accelerating ? accelSpeed : baseSpeed;

        // Calculamos la cantidad máxima de cambio por frame
        float maxDelta = speedChangeRate * Time.fixedDeltaTime;

        if (grounded)
        {
            // En tierra: ajustamos velocidad suavemente hacia la targetSpeed
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, maxDelta);
        }
        else
        {
            // En aire: control limitado, puede ganar o perder velocidad más despacio
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, maxDelta * airControl);
        }

        // Aplicar la velocidad horizontal mientras mantenemos la vertical
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    #region INPUT

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

    #region SUELO

    void OnCollisionStay2D(Collision2D col)
    {
        grounded = col.contacts[0].normal.y > 0.6f;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        grounded = false;
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
}
