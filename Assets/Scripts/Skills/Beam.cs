using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    public float speed = 8f;
    public Transform target;
    private Animator anim;
    private BeamPool beamPool;
    private SphereCollider sphereCollider;
    
    [Header("Targeting")]
    public float detectionRadius = 2f;
    public LayerMask enemyLayerMask = 1;
    public Transform ehyalTransform;
    
    [Header("Lifetime")]
    public float maxLifetime = 3f;
    
    private Vector3 lastKnownDirection;
    private bool hasTarget = false;
    private float lifetimeTimer = 0f;
    private bool isDisappearing = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    public void ShootToTarget()
    {
        // Si está desapareciendo, no hacer nada
        if (isDisappearing) return;

        // NO buscar enemigos aquí, solo usar el target ya asignado
        // Transform nearestEnemy = FindNearestEnemy(); // ❌ ELIMINAR ESTA LÍNEA

        if (target != null && target.gameObject.activeInHierarchy)
        {
            // Tener enemigo como objetivo
            hasTarget = true;
            // NO recalcular lastKnownDirection aquí, mantener la que se configuró
        }
        else
        {
            // No hay enemigos, disparar en la dirección configurada
            hasTarget = false;

            // Incrementar timer para beams sin target
            lifetimeTimer += Time.deltaTime;

            // Verificar si debe desaparecer
            if (lifetimeTimer >= maxLifetime && !isDisappearing)
            {
                StartCoroutine(DisappearBeam());
                return;
            }
        }

        // MOVER LA BALA
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
        
        // Resetear valores IMPORTANTE
        lifetimeTimer = 0f;
        isDisappearing = false;
        speed = 8f; // Resetear velocidad
        sphereCollider.enabled = true;
        
        // Resetear posición y otros valores
        if (target != null)
        {
            lastKnownDirection = (target.position - transform.position).normalized;
            hasTarget = true;
        }
        else
        {
            hasTarget = false;
            //if (ehyalTransform != null)
            //{
            //    lastKnownDirection = GetEhyalDirection();
            //}
            //else
            //{
            //    lastKnownDirection = Vector3.right;
            //}
            lastKnownDirection = Vector3.right;
        }
    }

    // Método para disparo direccional (sin target)
    public void SetupBeamDirectional(BeamPool pool, Transform ehyal, Vector3 direction)
    {
        target = null;
        beamPool = pool;
        ehyalTransform = ehyal;
        
        // Resetear valores
        lifetimeTimer = 0f;
        isDisappearing = false;
        speed = 8f;
        sphereCollider.enabled = true;
        
        // Configurar dirección específica
        hasTarget = false;
        lastKnownDirection = direction.normalized;
    }

    public void ReturnInAnimation()
    {
        beamPool.ReturnBeam(gameObject);
    }
    
    // Método para desaparecer sin impacto
    private IEnumerator DisappearBeam()
    {
        isDisappearing = true;
        speed = 0;
        sphereCollider.enabled = false;
        
        // Reproducir animación de desaparecer
        if (anim != null)
        {
            anim.SetTrigger("Dissapear");
        }
        
        // Esperar a que termine la animación
        yield return new WaitForSeconds(0.5f);
        
        // Retornar al pool
        if (beamPool != null)
        {
            beamPool.ReturnBeam(gameObject);
        }
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

