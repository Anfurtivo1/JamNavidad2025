using System.Collections;
using System.Globalization;
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
    public Sprite collecionableImage;
    public TextMeshProUGUI speedText;
    public GameObject[] collectableSprites;
    private bool hitLocked = false;
    private SpriteRenderer spriteRenderer;
    public CameraFollow2D cameraFollow;

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

    [Header("Slopes")]
    public float slopeDownhillBonus = 6f;   // cuánto puede sumar al bajar 
    public float slopeUphillPenalty = 6f;   // cuánto puede restar al subir
    [Range(0f, 89f)] public float slopeMaxAngle = 45f;

    [Header("Slope Momentum")]
    public float slopeAccel = 20f;          // cuánto empuja cuesta abajo 
    public float slopeBrake = 8f;           // cuánto frena cuesta arriba 
    public float slopeDrag = 6f;            // rozamiento: cuánto se pierde en plano 
    public float maxSlopeBonus = 10f;       // máximo extra de velocidad por inercia

    private float slopeBonusSpeed = 0f;

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

    [Header("Nivel")]

    public bool levelPassed = false;

    public Timer timer;
    bool estrella1Nivel = false;
    bool estrella2Collecionables = false;
    bool estrella3Tiempo = false;



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


        //  Slope momentum 
        if (grounded)
        {
            Vector2 tangent = new Vector2(groundNormal.y, -groundNormal.x).normalized;
            if (tangent.x < 0f) tangent = -tangent;

            // Inclinación
            float slopeAngle = Vector2.Angle(groundNormal, Vector2.up);
            float angle01 = Mathf.Clamp01(slopeAngle / slopeMaxAngle);

            bool downhill = tangent.y < 0f; // bajando hacia la derecha
            bool uphill = tangent.y > 0f; // subiendo hacia la derecha

            if (downhill)
            {
                // aceleración proporcional a la inclinación
                slopeBonusSpeed += slopeAccel * angle01 * Time.fixedDeltaTime;
            }
            else if (uphill)
            {
                // perdemos bonus al subir
                slopeBonusSpeed -= slopeBrake * angle01 * Time.fixedDeltaTime;
            }
            else
            {
                // se va perdiendo poco a poco por rozamiento (ni subir ni bajar)
                slopeBonusSpeed -= slopeDrag * Time.fixedDeltaTime;
            }
        }

        slopeBonusSpeed = Mathf.Clamp(slopeBonusSpeed, 0f, maxSlopeBonus);
        // si frenamos de golpe cuando vamos muy rapido
        if (decelerating && !hitLocked)
        {
            slopeBonusSpeed = Mathf.MoveTowards(
                slopeBonusSpeed,
                0f,
                (slopeDrag + slopeBrake + 20f) * Time.fixedDeltaTime
            );
             slopeBonusSpeed = 0f;
        }

        targetSpeed += slopeBonusSpeed;

        

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
            string tiempoTotal = timer.text.text.Trim().Replace(',', '.');

            timer.totalTime = float.Parse(tiempoTotal, NumberStyles.Float, CultureInfo.InvariantCulture);

            timer.completed = true;

            Debug.Log("El tiempo final fue de: "+ timer.totalTime);
            levelPassed = true;

            estrella1Nivel = true;

            //contadorCollectables = 3;
            //timer.totalTime = 5.5f;

            if ( contadorCollectables >= 3)
            {
                estrella2Collecionables = true;
            }

            if (SceneManager.GetActiveScene().buildIndex == 1 && timer.totalTime < 60)//1 Es el primer nivel, el 0 sería el menu principal
            {
                estrella3Tiempo = true;
            }

            if (SceneManager.GetActiveScene().buildIndex == 2 && timer.totalTime < 40)//2 Es el segundo nivel, el 0 sería el menu principal
            {
                estrella3Tiempo = true;
            }



            if (estrella1Nivel && estrella2Collecionables && estrella3Tiempo)
            {
                Debug.Log("Se han conseguido las tres estrellas");
            }
            else if (estrella1Nivel && estrella2Collecionables)
            {
                Debug.Log("Se han conseguido la primera y la segunda estrella");
            }
            else if (estrella1Nivel && estrella3Tiempo)
            {
                Debug.Log("Se han conseguido la primera y la tercera estrella");
            }
            else if (estrella2Collecionables && estrella3Tiempo)
            {
                Debug.Log("Se han conseguido la segunda y la tercera estrella");
            }
            else if (estrella1Nivel)
            {
                Debug.Log("Solo se ha conseguido la primera estrella");
            }
            else if (estrella2Collecionables)
            {
                Debug.Log("Solo se ha conseguido la segunda estrella");
            }
            else if (estrella3Tiempo)
            {
                Debug.Log("Solo se ha conseguido la tercera estrella");
            }



        }

        if (collision.collider.CompareTag("Collectable"))
        {
            collision.gameObject.GetComponent<Animator>().SetTrigger("Collected");

            contadorCollectables += 1;
            if (contadorCollectables - 1 < collectableSprites.Length)
            {
                //collectableSprites[contadorCollectables - 1].SetActive(true);
                collectableSprites[contadorCollectables - 1].GetComponent<Image>().sprite = collecionableImage;
            }
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
