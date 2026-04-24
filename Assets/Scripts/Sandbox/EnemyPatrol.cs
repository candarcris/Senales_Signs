using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyPatrol : MonoBehaviour
{
    // 1. Definimos los estados posibles
    public enum EstadoEnemigo { Patrullando, Persiguiendo, Atacando, Golpeado }
    public EstadoEnemigo estadoActual = EstadoEnemigo.Patrullando;

    public Transform[] waypoints;
    public Transform shootPoint;
    [SerializeField] private float attackRange = 5f; // Rango del ataque (reemplaza a la esfera pequeña)

    private NavMeshAgent agent;
    private Transform player;
    private float nextAttackTime;
    private int currentPoint = 0;
    private float baseSpeed; // Guardaremos la velocidad original aquí
    private float baseAcceleration; // Guardaremos la velocidad original aquí

    [Header("Fuerza del Lanzamiento")]
    public float fuerzaAdelante = 15f; // Sube este valor para más velocidad directa
    public float fuerzaArriba = 5f;    // Sube este valor para un arco más alto

    [Header("Gizmos")]
    public Vector3 centroOffset = new Vector3(0, 4.1f, 0); // Ajusta la "Y" para subirlo al pecho

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        baseSpeed = agent.speed; // Guardamos la velocidad normal de patrulla
        baseAcceleration = agent.acceleration;
        GoToNextPoint();
    }

    void Update()
    {
        if (!agent.isActiveAndEnabled) return;
        if (player == null || !player.CompareTag("Player"))
        {
            player = null; // Lo olvidamos

            if (estadoActual != EstadoEnemigo.Patrullando)
            {
                CambiarEstado(EstadoEnemigo.Patrullando);
            }

            ComportamientoPatrulla();
            FlipSprite();
            return;
        }

        // 2. El "Cerebro" del enemigo: Solo hace lo que dicta su estado actual
        switch (estadoActual)
        {
            case EstadoEnemigo.Patrullando:
                ComportamientoPatrulla();
                break;

            case EstadoEnemigo.Persiguiendo:
                ComportamientoPersecucion();
                break;

            case EstadoEnemigo.Atacando:
                ComportamientoAtaque();
                break;

            case EstadoEnemigo.Golpeado:
                // No hace nada mientras está siendo golpeado/retrocediendo
                break;
        }

        FlipSprite();
    }

    void FlipSprite()
    {
        if (agent.velocity.x > 0.1f) transform.localScale = new Vector3(-1, 1, 1);
        else if (agent.velocity.x < -0.1f) transform.localScale = new Vector3(1, 1, 1);
    }

    void ComportamientoPatrulla()
    {
        // Transición: Si ve al jugador, cambia a persecución
        if (player != null)
        {
            CambiarEstado(EstadoEnemigo.Persiguiendo);
            return;
        }

        agent.isStopped = false;
        agent.speed = baseSpeed;
        agent.acceleration = baseAcceleration;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }

    void ComportamientoPersecucion()
    {
        // Transición: Si pierde al jugador, vuelve a patrullar
        if (player == null)
        {
            CambiarEstado(EstadoEnemigo.Patrullando);
            return;
        }

        // Transición: Si está lo suficientemente cerca, ataca
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            CambiarEstado(EstadoEnemigo.Atacando);
            return;
        }

        agent.isStopped = false;
        agent.speed = baseSpeed + 20f; // Corre un poco más rápido
        agent.acceleration = baseAcceleration + 20f;
        agent.SetDestination(player.position);
    }

    void ComportamientoAtaque()
    {
        // Transición: Si el jugador desaparece, vuelve a patrullar
        if (player == null)
        {
            CambiarEstado(EstadoEnemigo.Patrullando);
            return;
        }

        // Transición: Si el jugador se aleja, vuelve a perseguirlo
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > attackRange)
        {
            CambiarEstado(EstadoEnemigo.Persiguiendo);
            return;
        }

        agent.isStopped = true; // Se queda quieto para disparar

        if (Time.time >= nextAttackTime)
        {
            LanzarRoca();
            nextAttackTime = Time.time + 3f;
        }
    }

    void CambiarEstado(EstadoEnemigo nuevoEstado)
    {
        estadoActual = nuevoEstado;

        // ¡AQUÍ ES DONDE CONECTAREMOS EL ANIMATOR LUEGO!
        // Ejemplo: animator.SetInteger("Estado", (int)estadoActual);
    }

    void LanzarRoca()
    {
        GameObject rock = RockPool.Instance.GetRock();
        if (rock != null)
        {
            rock.transform.position = shootPoint.position;
            rock.SetActive(true);

            // Cálculo de Tiro Parabólico simple
            Rigidbody rb = rock.GetComponent<Rigidbody>();
            Vector3 direction = (player.position - shootPoint.position);
            Vector3 dirXZ = new Vector3(direction.x, 0, direction.z);

            rb.linearVelocity = Vector3.zero; // Limpiar velocidad previa

            // Usamos las variables en lugar de los números fijos
            rb.AddForce(dirXZ.normalized * fuerzaAdelante + Vector3.up * fuerzaArriba, ForceMode.Impulse);
        }
    }

    void GoToNextPoint()
    {
        if (waypoints.Length == 0 || !agent.isActiveAndEnabled) return;

        agent.destination = waypoints[currentPoint].position;
        currentPoint = (currentPoint + 1) % waypoints.Length;
    }

    // DETECCIÓN
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform; // El enemigo detecta al jugador y guarda su referencia
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null; // Pierde de vista al jugador
            GoToNextPoint(); // Vuelve a su ruta de patrulla
        }
    }

    private void OnDrawGizmos()
    {
        // 1. Calculamos el centro real sumando el offset al pivot (los pies)
        Vector3 centroReal = transform.position + centroOffset;

#if UNITY_EDITOR
        // 2. Usamos Handles para dibujar un círculo perfecto
        Handles.color = Color.red;

        // DrawWireDisc necesita: Posición, Dirección hacia donde mira, y Radio.
        // Vector3.up dibuja el círculo "acostado" paralelo al suelo.
        // Si lo quisieras "de pie" (como un escudo), usarías Vector3.forward
        Handles.DrawWireDisc(centroReal, Vector3.up, attackRange);
#endif
    }

    // MECANICA DE GOLPE EHYAL
    public void RecibirGolpe(Vector3 direccionRetroceso, float fuerzaRetroceso)
    {
        CambiarEstado(EstadoEnemigo.Golpeado);
        StartCoroutine(RutinaGolpeCinematico(direccionRetroceso, fuerzaRetroceso));
    }

    private System.Collections.IEnumerator RutinaGolpeCinematico(Vector3 dir, float fuerza)
    {
        // 1. HIT-STOP (Pausa o congelamiento inicial para dar sensación de impacto)
        if (agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero; // Frenado en seco
        }
        
        // Aquí podrías pausar su animación también ej: animator.speed = 0;
        yield return new WaitForSeconds(0.08f); // 80ms de "congelamiento"
        // animator.speed = 1;

        // 2. RETROCESO CINEMÁTICO (Ease-Out)
        float duracionRetroceso = 0.25f;
        float tiempo = 0f;

        while (tiempo < duracionRetroceso)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracionRetroceso;
            
            // Calculamos una curva Ease-Out Cubic (rápido al inicio, lento al final)
            float easeOutT = 1f - Mathf.Pow(1f - t, 3f);
            
            // La velocidad va de mucha (fuerza * multiplicador) hasta frenar a 0
            float velocidadActual = Mathf.Lerp(fuerza * 2f, 0f, easeOutT);

            if (agent.isActiveAndEnabled)
            {
                // Movemos usando el agente para que respete colisiones y no traspase paredes
                agent.Move(dir * velocidadActual * Time.deltaTime);
            }

            yield return null;
        }

        // 3. RECUPERACIÓN
        if (agent.isActiveAndEnabled)
        {
            agent.isStopped = false;
        }
        
        CambiarEstado(player != null ? EstadoEnemigo.Persiguiendo : EstadoEnemigo.Patrullando);
    }
}
