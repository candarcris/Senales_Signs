using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    public float chaseRange = 10f;
    private int currentPoint = 0;

    private NavMeshAgent agent;
    private Transform player;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GoToNextPoint();
    }

    void Update()
    {
        if (isChasing && player != null)
        {
            // ESTADO: PERSIGUIENDO
            agent.SetDestination(player.position);
        }
        else
        {
            // ESTADO: PATRULLANDO
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GoToNextPoint();
            }
        }

        // Lógica de Flip (Giro de cara)
        // Usamos la velocidad del agente para saber si va a izq o der
        if (agent.velocity.x > 0.1f) transform.localScale = new Vector3(-1, 1, 1);
        else if (agent.velocity.x < -0.1f) transform.localScale = new Vector3(1, 1, 1);
    }

    void GoToNextPoint()
    {
        if (waypoints.Length == 0) return;

        agent.destination = waypoints[currentPoint].position;
        currentPoint = (currentPoint + 1) % waypoints.Length;
    }

    // DETECCIÓN
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = true;
            player = other.transform;
            agent.speed += 10;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = false;
            agent.speed -= 10;
            // Al salir del rango, vuelve inmediatamente a su ruta
            GoToNextPoint();
        }
    }
}
