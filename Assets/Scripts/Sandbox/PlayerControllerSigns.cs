using UnityEngine;

public class PlayerControllerSigns : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 5f;
    private float initialSpeed;
    public float gravity = -9.81f;

    [Header("Salto y Físicas")]
    public float jumpHeight = 3f;

    [Tooltip("Tiempo de gracia para saltar tras caer de una orilla")]
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    
    [Tooltip("Tiempo que el juego recuerda que presionaste el botón de salto antes de tocar el suelo")]
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private Vector3 velocity;
    public Transform targetLockOn;

    public void FrenarEnSeco()
    {
        velocity = Vector3.zero;
    }
    private Transform cam;

    [Header("Componentes Visuales")]
    // Ahora guardaremos TODOS los pedazos del cuerpo
    private SpriteRenderer[] todasLasPartes;
    private MaterialPropertyBlock propertyBlock;

    public bool puedeMoverse = true;     // Controla si recibe inputs
    private float lastLockOnAngle;

    private void Awake()
    {
        initialSpeed = speed;
        cam = Camera.main.transform;

        // 1. Busca todos los SpriteRenderers en este objeto y en sus hijos (el PSD completo)
        todasLasPartes = GetComponentsInChildren<SpriteRenderer>();

        // 2. Inicializamos el bloque de propiedades (súper optimizado)
        propertyBlock = new MaterialPropertyBlock();
    }

    void Start()
    {
        
    }

    public void RecibirImpacto()
    {
        if (puedeMoverse) // Evita que se reinicie el contador si le pegan 2 rocas a la vez
        {
            StartCoroutine(RutinaDeImpacto());
        }
    }

    private System.Collections.IEnumerator RutinaDeImpacto()
    {
        // 1. Pierde el control
        puedeMoverse = false;

        // 2. Truco Ninja: Le quitamos el Tag "Player" para que los enemigos no lo reconozcan
        //gameObject.tag = "Untagged";

        // Definimos los colores (asumiendo que el color base es blanco/normal)
        Color colorOriginal = Color.white;
        Color colorDanio = new Color(1f, 0.3f, 0.3f, 0f);

        // El nombre de la variable de color en URP suele ser "_BaseColor"
        string nombrePropiedad = "_BaseColor";
        // Nota: Si tus sprites se ponen negros en vez de titilar, cambia la línea de arriba a "_Color"

        // 3. Espacio para futura animación
        // animator.SetTrigger("Hit");

        // 4. Bucle de Titileo (Dura 2 segundos)
        float tiempoTotal = 2f;
        float tiempoPasado = 0f;
        bool mostrarDanio = true;

        while (tiempoPasado < tiempoTotal)
        {
            // Elegimos qué color toca en este frame
            Color colorActual = mostrarDanio ? colorDanio : colorOriginal;

            // Le metemos el color al bloque de propiedades
            propertyBlock.SetColor(nombrePropiedad, colorActual);

            // Se lo aplicamos a CADA pedazo del cuerpo al mismo tiempo
            foreach (SpriteRenderer parte in todasLasPartes)
            {
                parte.SetPropertyBlock(propertyBlock);
            }

            mostrarDanio = !mostrarDanio;
            yield return new WaitForSeconds(0.15f);
            tiempoPasado += 0.15f;
        }

        // Restaurar todo a la normalidad
        propertyBlock.SetColor(nombrePropiedad, colorOriginal);
        foreach (SpriteRenderer parte in todasLasPartes)
        {
            parte.SetPropertyBlock(propertyBlock);
        }

        gameObject.tag = "Player";
        puedeMoverse = true;
    }

    void Update()
    {
        if (!puedeMoverse) return;

        // --- SISTEMAS DE SALTO MEJORADO ---
        // 1. Coyote Time
        if (controller.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // 2. Jump Buffer
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // 1. Obtener inputs de movimiento
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (targetLockOn != null)
        {
            // Opcional: Podrías reducir un poco la velocidad de strafing aquí multiplicando horizontal o vertical,
            // pero por ahora lo dejamos libre.
        }

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f || targetLockOn != null)
        {
            // --- MOVIMIENTO ---
            if (direction.magnitude >= 0.1f)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift)) speed = initialSpeed * 2;
                if (Input.GetKeyUp(KeyCode.LeftShift)) speed = initialSpeed;

                // Movimiento libre normal relativo a la cámara
                // Como la cámara tiene un "LookAt" hacia el enemigo, "W" siempre será hacia el enemigo
                // "S" siempre será hacia la cámara, y "A"/"D" harán strafing lateral naturalmente.
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }

            // --- GIRO (FLIP VISUAL) ---
            if (targetLockOn != null)
            {
                // Sprite bloqueado mirando hacia el enemigo
                float dirHaciaObjetivo = targetLockOn.position.x - transform.position.x;
                if (Mathf.Abs(dirHaciaObjetivo) > 0.05f) 
                {
                    float flip = (dirHaciaObjetivo > 0) ? 1f : -1f;
                    transform.localScale = new Vector3(flip, 1f, 1f);
                }
            }
            else if (horizontal != 0)
            {
                // Usamos el input horizontal libre si no hay objetivo
                float flip = (horizontal > 0) ? 1f : -1f;
                transform.localScale = new Vector3(flip, 1f, 1f);
            }
        }

        // Gravedad simple: Resetear si toca el suelo
        if (controller.isGrounded && velocity.y < 0)
        {
            // Ojo: un valor de -2f en el suelo nos mantiene pegados al CharacterController
            velocity.y = -2f;
        }

        // --- LÓGICA DE SALTO ---
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            // Ecuación física para el salto. Invertimos gravedad porque es negativa
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            
            // Consumimos el buffer y el coyote time para no hacer doble saltos accidentales
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        // Cancelación de salto (si suelta el botón antes de tiempo, cae más rápido)
        if (Input.GetButtonUp("Jump") && velocity.y > 0f)
        {
            velocity.y *= 0.5f; 
            coyoteTimeCounter = 0f;
        }

        velocity.y += gravity * Time.deltaTime;

        // Limite de velocidad maxima de caida
        if (velocity.y < -120f)
        {
            velocity.y = -120f;
        }

        // Aplicamos el movimiento en Y (gravedad/salto)
        controller.Move(velocity * Time.deltaTime);
    }
}
