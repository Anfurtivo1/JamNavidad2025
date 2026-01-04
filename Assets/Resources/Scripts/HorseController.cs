using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HorseController : MonoBehaviour
{
    [Header("Componentes")]
    public Rigidbody2D rb;
    public Transform horseVisual;
    public Animator respawnAnim;
    public Animator estelaAnim;
    public GameObject estela;
    public Image meter;

    [Header("Velocidades")]
    public float baseSpeed = 15f;
    public float accelSpeed = 25f;
    public float decelSpeed = 6f;
    public float speedAccelRate = 18f;
    public float speedDecelRate = 18f;
    public float airControl = 0.5f;
    public Sprite[] meterSprites;

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
    public bool decelerating;
    public float currentSpeed;

    private Vector2 groundNormal = Vector2.up;

    [Header("Hit")]
    public float hitDuration = 0.5f;
    public float blinkInterval = 0.1f;
    public float hitSpeedMultiplier = 0.5f;
    public float minHitSpeed = 4f;
    public float hitSlowMultiplier = 0.85f;
    public int contadorCollectables = 0;

    public GameObject[] collectableSprites;

    public TextMeshProUGUI speedText;

    private bool hitLocked = false;
    private SpriteRenderer spriteRenderer;
    public CameraFollow2D cameraFollow; // Arrastra tu cámara aquí

    public bool levelPassed = false;

    

    void Awake()
    {
        //Animator respawnAnim = this.GetComponent<Animator>();

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
        speedText.text = $"Speed: {currentSpeed:F1}";
        grounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        if(currentSpeed <= 6)
        {
            meter.sprite = meterSprites[1];
            
        }

        if(currentSpeed >= 10 && currentSpeed < 20)
        {
            meter.sprite = meterSprites[2];
        }

        if(currentSpeed >= 13 && currentSpeed < 20)
        {
            meter.sprite = meterSprites[3];
        }

        if(currentSpeed >= 15 && currentSpeed < 20)
        {
            meter.sprite = meterSprites[4];
        }

        if(currentSpeed >= 20)
        {
            meter.sprite = meterSprites[5];
        }

        float targetSpeed = baseSpeed;

        if (accelerating && !hitLocked)
        {
            targetSpeed = accelSpeed;
        }
        else if (decelerating && !hitLocked)
        {
            targetSpeed = decelSpeed;
        }

        if (hitLocked)
        {
            targetSpeed *= hitSpeedMultiplier;
        }

        float changeRate;

        // Si el objetivo está por encima acelera hacia targetSpeed
        if (targetSpeed > currentSpeed)
            changeRate = speedAccelRate;
        else
            changeRate = speedDecelRate; // incluye frenar y volver a baseSpeed 

        float maxDelta = changeRate * Time.fixedDeltaTime;


        if (grounded)
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                targetSpeed,
                maxDelta
            );
            estelaAnim.SetBool("Grounded", true);
            estela.SetActive(true);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                targetSpeed,
                maxDelta * airControl
            );
            estelaAnim.SetBool("Grounded", false);
            estela.SetActive(false);
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

    if (ctx.performed)
    {
        // Al pulsar D
        estelaAnim.SetBool("Acelerando", true);

        if (cameraFollow != null)
        {
            cameraFollow.offsetMultiplier = 1.5f;
        }
    }
    else if (ctx.canceled)
    {
        // Al SOLTAR D
        estelaAnim.SetBool("Acelerando", false);

        if (cameraFollow != null)
        {
            cameraFollow.offsetMultiplier = 1f;
        }
    }
}


    public void OnDecelerate(InputAction.CallbackContext ctx)
{
    decelerating = ctx.ReadValueAsButton();

    if (ctx.performed)
    {
        estelaAnim.SetBool("Acelerando", false);

        if (cameraFollow != null)
        {
            cameraFollow.offsetMultiplier = 0.7f;
        }
    }
    else if (ctx.canceled)
    {
        if (cameraFollow != null)
        {
            cameraFollow.offsetMultiplier = 1f;
        }
    }
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
            levelPassed = true;
        }

        if (collision.collider.CompareTag("Collectable"))
        {
            collision.gameObject.GetComponent<Animator>().SetTrigger("Collected");

            contadorCollectables += 1;
            if (contadorCollectables - 1 < collectableSprites.Length)
            {
                collectableSprites[contadorCollectables - 1].SetActive(true);
            }
            //Destroy(collision.collider.gameObject);
        }

        if (collision.collider.CompareTag("Respawn"))
        {
            respawnAnim.SetTrigger("HorseFell");
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }

    IEnumerator HitRoutine()
    {
        hitLocked = true;

        float speed01 = Mathf.InverseLerp(
            baseSpeed,
            accelSpeed,
            currentSpeed
        );

        float slowFactor = Mathf.Lerp(
            0.25f,
            hitSlowMultiplier,
            speed01
        );

        currentSpeed *= (1f - slowFactor);
        currentSpeed = Mathf.Max(currentSpeed, minHitSpeed);

        float elapsed = 0f;
        bool visible = true;

        Color originalColor = spriteRenderer.color;

        while (elapsed < hitDuration)
        {
            visible = !visible;

            Color c = originalColor;
            c.a = visible ? 1f : 0.2f; // parpadeo suave
            spriteRenderer.color = c;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        spriteRenderer.color = originalColor;
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
