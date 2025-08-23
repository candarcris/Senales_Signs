using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    public float speed;
    public Transform target;
    private Animator anim;
    private BeamPool beamPool;
    private SphereCollider sphereCollider;
    
    [Header("Targeting")]
    public float detectionRadius = 10f; // Radio de detección de enemigos
    public LayerMask enemyLayerMask = 1; // Capa de enemigos
    public Transform ehyalTransform; // Referencia a Ehyal para dirección de disparo
    
    private Vector3 lastKnownDirection; // Última dirección conocida
    private bool hasTarget = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    public void ShootToTarget()
    {
        // Buscar enemigo más cercano
        Transform nearestEnemy = FindNearestEnemy();
        
        if (nearestEnemy != null && nearestEnemy.gameObject.activeInHierarchy)
        {
            // Tener enemigo como objetivo
            target = nearestEnemy;
            hasTarget = true;
            lastKnownDirection = (target.position - transform.position).normalized;
        }
        else
        {
            // No hay enemigos, disparar en la dirección de Ehyal
            hasTarget = false;
            if (ehyalTransform != null)
            {
                lastKnownDirection = GetEhyalDirection();
            }
        }
        
        if (hasTarget && target != null)
        {
            // Mover hacia el enemigo
            Vector3 targetPosition = target.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
            
            // Actualizar dirección para rotación
            lastKnownDirection = (targetPosition - transform.position).normalized;
        }
        else
        {
            // Mover en la dirección de Ehyal
            transform.position += lastKnownDirection * speed * Time.deltaTime;
        }

        // Rotar la bala
        RotateBeam();
    }

    private Transform FindNearestEnemy()
    {
        // Buscar enemigos en el radio de detección
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayerMask);
        
        Transform nearestEnemy = null;
        float nearestDistance = float.MaxValue;
        
        foreach (Collider enemyCollider in enemiesInRange)
        {
            if (enemyCollider.CompareTag("Enemy") && enemyCollider.gameObject.activeInHierarchy)
            {
                float distance = Vector3.Distance(transform.position, enemyCollider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemyCollider.transform;
                }
            }
        }
        
        return nearestEnemy;
    }

    private Vector3 GetEhyalDirection()
    {
        if (ehyalTransform == null) return Vector3.right;
        
        // Obtener la dirección hacia donde mira Ehyal
        // Asumiendo que Ehyal tiene un componente que indica su dirección
        Vector3 ehyalDirection = ehyalTransform.right; // O la propiedad que uses para dirección
        
        return ehyalDirection;
    }

    private void RotateBeam()
    {
        if (lastKnownDirection != Vector3.zero)
        {
            // Calcular el ángulo en Z usando atan2
            float angle = Mathf.Atan2(lastKnownDirection.y, lastKnownDirection.x) * Mathf.Rad2Deg;

            // Aplicar la rotación suavemente solo en Z
            Vector3 currentRotation = transform.rotation.eulerAngles;
            float newZ = Mathf.LerpAngle(currentRotation.z, angle, Time.deltaTime * 5f);
            transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, newZ);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            // Retornar al pool en lugar de destruir
            StartCoroutine(ReturnToPool());
        }
    }

    private void Update()
    {
        ShootToTarget();
    }
    
    // Método para configurar la bala cuando se obtiene del pool
    public void SetupBeam(Transform newTarget, BeamPool pool, Transform ehyal = null)
    {
        target = newTarget;
        beamPool = pool;
        ehyalTransform = ehyal;
        
        // Resetear posición y otros valores
        if (target != null)
        {
            lastKnownDirection = (target.position - transform.position).normalized;
            hasTarget = true;
        }
        else
        {
            hasTarget = false;
            if (ehyalTransform != null)
            {
                lastKnownDirection = GetEhyalDirection();
            }
            else
            {
                lastKnownDirection = Vector3.right; // Dirección por defecto
            }
        }
        
        // Resetear otros valores
        speed = 8f; // O el valor que tengas configurado
        sphereCollider.enabled = true;
    }

    public void ReturnInAnimation()
    {
        beamPool.ReturnBeam(gameObject);
    }
    
    IEnumerator ReturnToPool()
    {
        speed = 0;
        sphereCollider.enabled = false;
        anim.SetTrigger("Impact");

        yield return new WaitForSeconds(0.50f);

        if (beamPool != null)
        {
            beamPool.ReturnBeam(gameObject);
        }
        else
        {
            // Fallback si no hay pool
            Destroy(gameObject);
        }
    }

    // Método para debug visual
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}
