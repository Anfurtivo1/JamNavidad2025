using UnityEngine;
using UnityEngine.InputSystem;

public class HorseController : MonoBehaviour
{
    Rigidbody2D rb;

    [Header("Velocidad")]
    public float baseSpeed = 5f;
    public float maxSpeed = 8f;
    public float brakeSpeed = 3f;
    public float accel = 10f;

    [Header("Salto")]
    public float jumpForce = 10f;

    bool grounded;
    bool accelerating;
    bool braking;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float targetSpeed = baseSpeed;

        if (braking)
            targetSpeed = brakeSpeed;

        if (accelerating && grounded)
            targetSpeed = maxSpeed;

        float newSpeed = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetSpeed,
            accel * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(newSpeed, rb.linearVelocity.y);
    }

    // ===== INPUT SYSTEM =====

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.9f, jumpForce);
        }
    }

    public void OnAccelerate(InputAction.CallbackContext ctx)
    {
        accelerating = ctx.ReadValueAsButton();
    }

    public void OnBrake(InputAction.CallbackContext ctx)
    {
        braking = ctx.ReadValueAsButton();
    }


    // ===== SUELO =====

    void OnCollisionStay2D(Collision2D col)
    {
        grounded = col.contacts[0].normal.y > 0.6f;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        grounded = false;
    }
}
