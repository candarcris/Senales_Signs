using Signs;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
    
    [Header("Configuración Lock-On")]
    [Tooltip("Si es verdadero, el sistema de fijación está encendido por el usuario")]
    public bool isLockOnActive = false;

    [Header("Referencias Opcionales")]
    public PlayerControllerSigns playerController;
    public CinemachineVirtualCamera vcam;
    private Transform originalLookAt;
    private Transform originalFollow;
    
    // Variables para el sistema de cámara ancla
    private GameObject cameraAnchor;
    private CinemachineOrbitalTransposer orbitalTransposer;
    private CinemachineOrbitalTransposer.Heading.HeadingDefinition originalHeading;
    private float originalMaxSpeed = -1f;
    private bool originalRecenterEnabled;
    private MonoBehaviour cinemachineInputProvider;
    
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

        if (playerController == null)
        {
            playerController = FindAnyObjectByType<PlayerControllerSigns>();
        }

        if (vcam == null)
        {
            CamerasManager camManager = FindAnyObjectByType<CamerasManager>();
            if (camManager != null) 
            {
                vcam = camManager._cinemachineCam;
            }
            else
            {
                vcam = FindAnyObjectByType<CinemachineVirtualCamera>();
            }
        }

        if (vcam != null)
        {
            originalLookAt = vcam.LookAt;
            originalFollow = vcam.Follow;
            
            orbitalTransposer = vcam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            if (orbitalTransposer != null)
            {
                originalHeading = orbitalTransposer.m_Heading.m_Definition;
                originalMaxSpeed = orbitalTransposer.m_XAxis.m_MaxSpeed;
                originalRecenterEnabled = orbitalTransposer.m_RecenterToTargetHeading.m_enabled;
            }

            cinemachineInputProvider = vcam.GetComponent("CinemachineInputProvider") as MonoBehaviour;
        }

        // Crear el ancla invisible para la cámara
        cameraAnchor = new GameObject("LockOnCameraAnchor");

        if (lockOnIndicator != null)
        {
            lockOnIndicator.SetActive(false); // Ocultar al inicio
        }
    }

    void Update()
    {
        if (ehyalTransform == null) return;

        UpdateEnemiesInRange();

        // Control de encendido/apagado manual con Click Derecho
        if (Input.GetMouseButtonDown(1)) // 1 es el botón derecho del mouse
        {
            isLockOnActive = !isLockOnActive;
            
            // Si lo acabamos de apagar, soltamos todo inmediatamente
            if (!isLockOnActive)
            {
                ClearLockOn();
            }
        }

        // Solo procesamos la fijación y el ciclo si está activo
        if (isLockOnActive)
        {
            HandleLockOn();

            // Cambiar objetivo con Control Derecho mientras esté activo
            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                CycleTarget();
            }
        }

        HandleAttack();
    }

    private void ClearLockOn()
    {
        currentTarget = null;
        if (lockOnIndicator != null) lockOnIndicator.SetActive(false);
        if (playerController != null) playerController.targetLockOn = null;
        
        if (vcam != null) 
        {
            vcam.LookAt = originalLookAt;
            vcam.Follow = originalFollow;
        }

        if (orbitalTransposer != null) 
        {
            orbitalTransposer.m_XAxis.m_InputAxisName = "Horizontal";
            if (originalMaxSpeed >= 0) orbitalTransposer.m_XAxis.m_MaxSpeed = originalMaxSpeed;
            orbitalTransposer.m_RecenterToTargetHeading.m_enabled = originalRecenterEnabled;
            orbitalTransposer.m_Heading.m_Definition = originalHeading;
        }
        
        if (cinemachineInputProvider != null) cinemachineInputProvider.enabled = true;
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
            ClearLockOn();
        }
    }

    private void HandleLockOn()
    {
        if (enemiesInRange.Count == 0)
        {
            // No hay enemigos para fijar, pero no apagamos el 'isLockOnActive' 
            // por si un enemigo entra en rango después. Solo limpiamos el objetivo actual.
            ClearLockOn();
            return;
        }

        // Si no hay objetivo, o si el actual se perdió, asignar el más cercano
        if (currentTarget == null)
        {
            currentTarget = GetClosestEnemy();
        }

        // Actualizar la posición del indicador visual
        if (currentTarget != null && lockOnIndicator != null)
        {
            lockOnIndicator.SetActive(true);
            // Posicionar el indicador un poco más arriba del enemigo (puedes ajustar el offset)
            lockOnIndicator.transform.position = currentTarget.transform.position + Vector3.up * 8f;
        }

        // Informar al controlador del jugador cuál es el objetivo
        if (playerController != null)
        {
            playerController.targetLockOn = currentTarget != null ? currentTarget.transform : null;
        }

        // Girar la cámara de Cinemachine hacia el enemigo y usar el Ancla
        if (vcam != null)
        {
            if (currentTarget != null)
            {
                // Posicionar el ancla exactamente en Sagar
                cameraAnchor.transform.position = playerController.transform.position;

                // Rotar el ancla para mirar al enemigo
                Vector3 lookDir = currentTarget.transform.position - playerController.transform.position;
                lookDir.y = 0;
                if (lookDir != Vector3.zero)
                {
                    // Podemos usar Slerp si queremos que la cámara se mueva suave al inicio del lock on
                    Quaternion targetRot = Quaternion.LookRotation(lookDir);
                    cameraAnchor.transform.rotation = Quaternion.Slerp(cameraAnchor.transform.rotation, targetRot, Time.deltaTime * 10f);
                }

                // Asignar el ancla como objetivo de la cámara
                vcam.Follow = cameraAnchor.transform;
                vcam.LookAt = currentTarget.transform;

                if (orbitalTransposer != null)
                {
                    if (cinemachineInputProvider != null) cinemachineInputProvider.enabled = false;

                    // Apagar motores internos de Cinemachine
                    orbitalTransposer.m_XAxis.m_InputAxisName = "";
                    orbitalTransposer.m_XAxis.m_InputAxisValue = 0f;
                    orbitalTransposer.m_XAxis.m_MaxSpeed = 0f;
                    orbitalTransposer.m_RecenterToTargetHeading.m_enabled = false;

                    // El secreto: Usar TargetForward significa que usará la rotación de "cameraAnchor"
                    // Al forzar el eje X a 0, la cámara se pondrá exactamente detrás del ancla!
                    orbitalTransposer.m_Heading.m_Definition = CinemachineOrbitalTransposer.Heading.HeadingDefinition.TargetForward;
                    
                    // Interpolar hacia 0 suavemente por si veníamos de otro ángulo al explorar
                    orbitalTransposer.m_XAxis.Value = Mathf.LerpAngle(orbitalTransposer.m_XAxis.Value, 0f, Time.deltaTime * 5f);
                }
            }
            else
            {
                ClearLockOn();
            }
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
