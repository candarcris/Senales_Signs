using Signs;
using System.Collections.Generic;
using UnityEngine;

public class EhyalStrikeMechanic : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El transform de Ehyal de donde se calculará el rango.")]
    public Transform ehyalTransform;
    
    [Tooltip("Indicador visual (ej. un Sprite o Quad en la escena) que se moverá al objetivo. Debe ser un GameObject instanciado en la escena.")]
    public GameObject lockOnIndicator;
    
    [Tooltip("Prefab del sistema de partículas a instanciar al golpear.")]
    public GameObject hitParticlesPrefab;

    [Header("Configuración de Ataque")]
    public float attackRange = 8f;
    public float attackCooldown = 0.5f;
    public float knockbackForce = 15f;
    public LayerMask enemyLayer; // Asegúrate de asignar la capa de los enemigos aquí

    private float lastAttackTime;
    [SerializeField] Vector3 slashEffectPos;
    
    [Header("Objetivos en Rango")]
    [SerializeField] private EnemyPatrol currentTarget;
    [SerializeField] private List<EnemyPatrol> enemiesInRange = new List<EnemyPatrol>();

    void Start()
    {
        if (ehyalTransform == null)
        {
            Ehyal ehyal = FindAnyObjectByType<Ehyal>();
            if (ehyal != null) ehyalTransform = ehyal.transform;
        }

        if (lockOnIndicator != null)
        {
            lockOnIndicator.SetActive(false); // Ocultar al inicio
        }
    }

    void Update()
    {
        if (ehyalTransform == null) return;

        UpdateEnemiesInRange();
        HandleLockOn();
        HandleAttack();
    }

    private void UpdateEnemiesInRange()
    {
        Collider[] hits = Physics.OverlapSphere(ehyalTransform.position, attackRange, enemyLayer);
        enemiesInRange.Clear();

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out EnemyPatrol enemy))
            {
                // Solo agregar si está vivo/activo y evitar duplicados si tiene varios colliders
                if (enemy.gameObject.activeInHierarchy && !enemiesInRange.Contains(enemy))
                {
                    enemiesInRange.Add(enemy);
                }
            }
        }

        // Solución al problema del Cambio de Objetivo:
        // Physics.OverlapSphere no garantiza el mismo orden de los objetos en cada frame.
        // Si el orden salta aleatoriamente, el "CycleTarget" falla porque el índice del objetivo actual
        // y el siguiente cambian de lugar. Ordenar la lista por su ID estabiliza este orden.
        enemiesInRange.Sort((a, b) => a.gameObject.GetInstanceID().CompareTo(b.gameObject.GetInstanceID()));

        // Si el objetivo actual salió del rango o fue destruido, perderlo
        if (currentTarget != null && !enemiesInRange.Contains(currentTarget))
        {
            currentTarget = null;
        }
    }

    private void HandleLockOn()
    {
        if (enemiesInRange.Count == 0)
        {
            currentTarget = null;
            if (lockOnIndicator != null) lockOnIndicator.SetActive(false);
            return;
        }

        // Si no hay objetivo, o si el actual se perdió, asignar el más cercano
        if (currentTarget == null)
        {
            currentTarget = GetClosestEnemy();
        }

        // Cambiar objetivo con Control Derecho
        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            CycleTarget();
        }

        // Actualizar la posición del indicador visual
        if (currentTarget != null && lockOnIndicator != null)
        {
            lockOnIndicator.SetActive(true);
            // Posicionar el indicador un poco más arriba del enemigo (puedes ajustar el offset)
            lockOnIndicator.transform.position = currentTarget.transform.position + Vector3.up * 8f;
        }
    }

    private EnemyPatrol GetClosestEnemy()
    {
        EnemyPatrol closest = null;
        float minDistance = float.MaxValue;

        foreach (EnemyPatrol enemy in enemiesInRange)
        {
            float dist = Vector3.Distance(ehyalTransform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemy;
            }
        }

        return closest;
    }

    private void CycleTarget()
    {
        if (enemiesInRange.Count <= 1) return; // No hay necesidad de cambiar

        int currentIndex = enemiesInRange.IndexOf(currentTarget);
        int nextIndex = (currentIndex + 1) % enemiesInRange.Count;
        currentTarget = enemiesInRange[nextIndex];
    }

    private void HandleAttack()
    {
        // Click izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                if (currentTarget != null)
                {
                    PerformStrike();
                }
            }
        }
    }

    private void PerformStrike()
    {
        lastAttackTime = Time.time;

        // 1. Instanciar partículas de golpe
        if (hitParticlesPrefab != null)
        {
            // Instanciar en la posición del enemigo (un poco más arriba)
            Instantiate(hitParticlesPrefab, currentTarget.transform.position + slashEffectPos, Quaternion.identity);
        }

        // 2. Calcular la dirección del retroceso (desde Ehyal hacia el enemigo)
        Vector3 knockbackDir = (currentTarget.transform.position - ehyalTransform.position).normalized;
        knockbackDir.y = 0; // Mantenerlo horizontal
        
        // 3. Aplicar daño / retroceso al enemigo
        currentTarget.RecibirGolpe(knockbackDir, knockbackForce);
    }

    private void OnDrawGizmosSelected()
    {
        if (ehyalTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(ehyalTransform.position, attackRange);
        }
    }
}
