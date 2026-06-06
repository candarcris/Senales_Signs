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
    private GameObject cameraLookTarget; // Punto focal para mantener a ambos en pantalla
    private CinemachineOrbitalTransposer orbitalTransposer;
    private CinemachineOrbitalTransposer.Heading.HeadingDefinition originalHeading;
    private float originalMaxSpeed = -1f;
    private bool originalRecenterEnabled;
    private MonoBehaviour cinemachineInputProvider;
    
    [Header("Objetivos en Rango")]
    [SerializeField] private Transform currentTarget;
    [SerializeField] private List<Transform> targetsInRange = new List<Transform>();

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
                
                // Aplicar configuraciones iniciales forzadas para exploración libre
                orbitalTransposer.m_XAxis.m_InputAxisName = "Mouse X";
                orbitalTransposer.m_Heading.m_Definition = CinemachineOrbitalTransposer.Heading.HeadingDefinition.WorldForward;
                orbitalTransposer.m_RecenterToTargetHeading.m_enabled = false;
            }

            cinemachineInputProvider = vcam.GetComponent("CinemachineInputProvider") as MonoBehaviour;
            if (cinemachineInputProvider != null) 
            {
                // Si tienes el New Input System, ESTO es lo que probablemente causa el giro
                cinemachineInputProvider.enabled = false; 
            }
        }

        // Crear el ancla invisible para la cámara
        cameraAnchor = new GameObject("LockOnCameraAnchor");
        cameraLookTarget = new GameObject("LockOnLookTarget");

        if (lockOnIndicator != null)
        {
            lockOnIndicator.SetActive(false); // Ocultar al inicio
        }
    }

    void Update()
    {
        if (ehyalTransform == null) return;

        UpdateEnemiesInRange();

        // Control de encendido/apagado manual
        if (Input.GetMouseButtonDown(2))
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

            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0f) // Si la rueda se movió algo (hacia arriba o hacia abajo)
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

        if (cameraLookTarget != null && originalLookAt != null)
        {
            // Regresamos el punto de visión al jugador
            cameraLookTarget.transform.position = originalLookAt.position;
        }
        if (cameraAnchor != null && Camera.main != null)
        {
            // Hacemos que el ancla mire hacia donde está mirando la cámara actual.
            // Así, al volver a hacer Lock-On, el giro empezará desde donde tú estás viendo.
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0;
            if (camForward != Vector3.zero)
            {
                cameraAnchor.transform.rotation = Quaternion.LookRotation(camForward);
            }
        }

        if (vcam != null) 
        {
            vcam.LookAt = originalLookAt;
            vcam.Follow = originalFollow;
        }

        if (orbitalTransposer != null) 
        {
            // Usamos "Mouse X" para que el ratón controle el giro libre, separándolo del movimiento A/D.
            orbitalTransposer.m_XAxis.m_InputAxisName = "Mouse X";
            if (originalMaxSpeed >= 0) orbitalTransposer.m_XAxis.m_MaxSpeed = originalMaxSpeed;
            orbitalTransposer.m_RecenterToTargetHeading.m_enabled = false;
            // "WorldForward" garantiza que la rotación base de la cámara es estática al mundo.
            // Si el jugador o cualquier hijo invisible gira al presionar A/D, la cámara lo IGNORARÁ por completo,
            // rotando ÚNICAMENTE cuando el ratón modifique el X Axis.
            orbitalTransposer.m_Heading.m_Definition = CinemachineOrbitalTransposer.Heading.HeadingDefinition.WorldForward;
        }
        
        if (cinemachineInputProvider != null) cinemachineInputProvider.enabled = true;
    }

    private void UpdateEnemiesInRange()
    {
        Collider[] hits = Physics.OverlapSphere(ehyalTransform.position, attackRange, enemyLayer);
        targetsInRange.Clear();

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out IDamage objetivoDañable))
            {
                // Solo agregar si está vivo/activo y evitar duplicados si tiene varios colliders
                if (hit.gameObject.activeInHierarchy && !targetsInRange.Contains(hit.transform))
                {
                    targetsInRange.Add(hit.transform);
                }
            }
        }

        // Solución al problema del Cambio de Objetivo:
        // Physics.OverlapSphere no garantiza el mismo orden de los objetos en cada frame.
        // Si el orden salta aleatoriamente, el "CycleTarget" falla porque el índice del objetivo actual
        // y el siguiente cambian de lugar. Ordenar la lista por su ID estabiliza este orden.
        targetsInRange.Sort((a, b) => a.gameObject.GetInstanceID().CompareTo(b.gameObject.GetInstanceID()));

        // Si el objetivo actual salió del rango o fue destruido, perderlo
        if (currentTarget != null && !targetsInRange.Contains(currentTarget))
        {
            isLockOnActive = false;
            ClearLockOn();
        }
    }

    private void HandleLockOn()
    {
        if (targetsInRange.Count == 0)
        {
            // No hay enemigos para fijar, pero no apagamos el 'isLockOnActive' 
            // por si un enemigo entra en rango después. Solo limpiamos el objetivo actual.
            ClearLockOn();
            return;
        }

        // Si no hay objetivo, o si el actual se perdió, asignar el más cercano
        if (currentTarget == null)
        {
            currentTarget = GetClosestTarget();
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
                // Posicionar el ancla exactamente en Sagar usando ehyalTransform (el centro visual real)
                cameraAnchor.transform.position = ehyalTransform.position;

                // Rotar el ancla para mirar al enemigo
                Vector3 lookDir = currentTarget.position - ehyalTransform.position;
                lookDir.y = 0;
                if (lookDir != Vector3.zero)
                {
                    // Podemos usar Slerp si queremos que la cámara se mueva suave al inicio del lock on
                    Quaternion targetRot = Quaternion.LookRotation(lookDir);
                    cameraAnchor.transform.rotation = Quaternion.Slerp(cameraAnchor.transform.rotation, targetRot, Time.deltaTime * 1f);
                }

                // Posicionar el punto de LookAt exactamente a la mitad entre Sagar y el Enemigo
                // Esto asegura que la cámara enfoque el centro del combate y no pierda a ninguno de los dos.
                // Ajustamos un poco en Y para no mirar al piso.
                Vector3 midPoint = Vector3.Lerp(ehyalTransform.position, currentTarget.transform.position, 0.5f);
                midPoint.y = (ehyalTransform.position.y + currentTarget.position.y) / 2f + 1.5f;
                //cameraLookTarget.transform.position = midPoint;
                cameraLookTarget.transform.position = Vector3.Lerp(cameraLookTarget.transform.position, midPoint, Time.deltaTime * 5f);

                // Asignar el ancla como objetivo de la cámara
                vcam.Follow = cameraAnchor.transform;
                vcam.LookAt = cameraLookTarget.transform;

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

    private Transform GetClosestTarget()
    {
        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach (Transform target in targetsInRange)
        {
            float dist = Vector3.Distance(ehyalTransform.position, target.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = target;
            }
        }

        return closest;
    }

    private void CycleTarget()
    {
        if (targetsInRange.Count <= 1) return; // No hay necesidad de cambiar

        int currentIndex = targetsInRange.IndexOf(currentTarget);
        int nextIndex = (currentIndex + 1) % targetsInRange.Count;
        currentTarget = targetsInRange[nextIndex];
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
        
        //currentTarget.RecibirKnockback(knockbackDir, knockbackForce);

        if (currentTarget.TryGetComponent(out IDamage objetivoDañable))
        {
            int attackDamage = 25; // Puedes volver esta variable pública luego
            objetivoDañable.RecibirImpacto(attackDamage);
        }
        // 2. Si el objetivo reacciona a los empujes, lo empujamos
        if (currentTarget.gameObject.activeInHierarchy && currentTarget.TryGetComponent(out IKnockBack objetivoEmpujable))
        {
            objetivoEmpujable.RecibirKnockback(knockbackDir, knockbackForce);
        }
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
