using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class GroundEnemy : Enemy
{
    private enum EnemyState
    {
        Patrol,
        Chase
    }
    private EnemyState _currentState = EnemyState.Patrol;

    public float reEscaledDamageAmount = 0;
    public float reEscaledLifeAmount = 0;

    public float _detectionRadius;
    public float _attackRange;

    public float _moveSpeed;
    [SerializeField] private float _patrolDistance = 5f;
    private Vector3 _startPosition;
    private bool _movingRight = true;


    public GroundEnemy(float damage, float lifeAmount) : base(damage, lifeAmount) { }

    private void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        _startPosition = transform.position;
        reEscaledDamageAmount = ReEscale.Normalize(20, 0, _lifeAmount, 0, 1);
        reEscaledLifeAmount = ReEscale.Normalize(_lifeAmount, 0, _lifeAmount, 0, 1);

        if (_animator != null)
        {
            _animator.SetBool("IsMoving", true);
        }
    }

    private void FixedUpdate()
    {
        Transform target = FindNearestPlayer();

        if (target != null)
        {
            if(transform.position.y >= target.position.y)
            {
                // Si encuentra jugador en el radio -> cambiar a persecuci�n
                _currentState = EnemyState.Chase;
            }
        }
        else
        {
            // Si no hay jugador -> volver a patrullar
            _currentState = EnemyState.Patrol;
        }

        switch (_currentState)
        {
            case EnemyState.Patrol:
                if (_isMoving)
                {
                    _moveSpeed = 1;
                    if (transform.localScale.x > 0)
                    {
                        _movingRight = false;
                    }
                    else
                    {
                        _movingRight = true;
                    }

                    HandleMovement();
                }
                break;

            case EnemyState.Chase:
                _moveSpeed = 3;
                ChasePlayer(target);
                break;
        }
    }

    private void HandleMovement()
    {
        float distanceFromStart = transform.position.x - _startPosition.x;

        // Cambia de direcci�n al llegar a los extremos
        if (_movingRight && distanceFromStart >= _patrolDistance)
        {
            _movingRight = false;
            FlipEnemy();
        }
        else if (!_movingRight && distanceFromStart <= -_patrolDistance)
        {
            _movingRight = true;
            FlipEnemy();
        }

        // Movimiento f�sico: solo cambia la velocidad en X
        float direction = _movingRight ? 1f : -1f;
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = new Vector3(direction * _moveSpeed, _rigidbody.linearVelocity.y, 0);
        }

        // Animaci�n
        if (_animator != null)
        {
            _animator.SetBool("IsMoving", true);
            _animator.SetBool("Attacks", false); // <- muy importante
            _animator.SetFloat("Speed", Mathf.Abs(_moveSpeed));
        }
    }

    private void FlipEnemy()
    {
        // Gira el sprite/enemigo en X
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EhyalPower"))
        {
            GetDamage(20);
        }
        else if(other.CompareTag("DeadZone"))
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (collision.contacts.Length > 0)
            {
                ContactPoint contact = collision.contacts[0];

                // Direcci�n de empuje (normal del impacto)
                Vector3 pushDir = contact.normal;

                pushDir.Normalize();
                damageable.TakeDamage(2, pushDir);
            }
        }
    }

    public void LookAtTarget(Transform targetTransform)
    {
        if (targetTransform == null) return;

        Vector3 scale = transform.localScale;
        if (targetTransform.position.x < transform.position.x)
            scale.x = Mathf.Abs(scale.x);  // mira a la izquierda
        else
            scale.x = -Mathf.Abs(scale.x); // mira a la derecha
        transform.localScale = scale;
    }

    private Transform FindNearestPlayer()
    {
        // Buscar enemigos en el radio de detecci�n
        Collider[] playersInRange = Physics.OverlapSphere(transform.position, _detectionRadius);

        Transform nearestPlayer = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider playerCollider in playersInRange)
        {
            if (playerCollider.CompareTag("Player") && playerCollider.gameObject.activeInHierarchy)
            {
                float distance = Vector3.Distance(transform.position, playerCollider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPlayer = playerCollider.transform;
                }
            }
        }
        return nearestPlayer;
    }

    private void ChasePlayer(Transform target)
    {
        if (target == null) return;

        // Mirar hacia el jugador
        LookAtTarget(target);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > _attackRange)
        {
            // Si est� lejos todav�a -> moverse hacia �l
            Vector3 direction = (target.position - transform.position).normalized;
            _rigidbody.linearVelocity = new Vector3(direction.x * _moveSpeed, _rigidbody.linearVelocity.y, 0);

            if (_animator != null)
            {
                _animator.SetBool("IsMoving", true);
                _animator.SetBool("Attacks", false);
            }
        }
        else
        {
            // Si est� lo suficientemente cerca -> atacar
            //_rigidbody.velocity = Vector3.zero; // parar movimiento al atacar

            if (_animator != null)
            {
                _animator.SetBool("IsMoving", false);
            }

            Attack(); // lanzar animaci�n / l�gica de ataque
        }
    }

    public override void Attack()
    {
        if (_animator != null)
        {
            _animator.SetBool("Attacks", true);
        }
    }

    private IEnumerator ResumeMovement()
    {
        yield return new WaitForSeconds(4f);
        _isMoving = true;
        if (_animator != null)
        {
            _animator.SetBool("IsMoving", true);
            _animator.SetBool("Attacking", false);
        }
    }

    public override void GetDamage(float amount)
    {
        if (reEscaledLifeAmount <= 0) return; // Ya est� muerto

        // Usar el amount recibido en lugar del da�o fijo
        float damageToApply = ReEscale.Normalize(amount, 0, _lifeAmount, 0, 1);
        reEscaledLifeAmount -= damageToApply;

        if (reEscaledLifeAmount <= 0.01f)
        {
            reEscaledLifeAmount = 0;
        }

        // Actualizar UI
        if (_lifeUI != null)
        {
            _lifeUI.fillAmount = reEscaledLifeAmount;
        }

        StopMovement();

        // Animaci�n de da�o
        if (_animator != null)
        {
            _animator.SetTrigger("GetDamage");
        }

        StartCoroutine(ResumeMovement());

        if (reEscaledLifeAmount <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        StopMovement();
        if (_animator != null)
        {
            _animator.SetTrigger("Die");
        }
        OnEnemyDeath();
        StartCoroutine(DeactivateAfterDeath());
    }

    private IEnumerator DeactivateAfterDeath()
    {
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}