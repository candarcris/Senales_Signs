using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GroundEnemy : Enemy
{
    public float reEscaledDamageAmount = 0;
    public float reEscaledLifeAmount = 0;

    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _patrolDistance = 5f;
    private Vector3 _startPosition;
    private bool _movingRight = true;

    public GroundEnemy(float damage, float lifeAmount) : base(damage, lifeAmount) { }

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
        if (_isMoving)
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        float distanceFromStart = transform.position.x - _startPosition.x;

        // Cambia de dirección al llegar a los extremos
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

        // Movimiento físico: solo cambia la velocidad en X
        float direction = _movingRight ? 1f : -1f;
        if (_rigidbody != null)
        {
            _rigidbody.velocity = new Vector3(direction * _moveSpeed, _rigidbody.velocity.y, 0);
        }

        // Animación
        if (_animator != null)
        {
            _animator.SetBool("IsMoving", true);
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
        if (other.CompareTag("Player"))
        {
            GetDamage(20);
        }
    }

    public override void Attack()
    {
        StopMovement();
        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }
        StartCoroutine(ResumeMovementAfterAttack());
    }

    private IEnumerator ResumeMovementAfterAttack()
    {
        yield return new WaitForSeconds(1f);
        _isMoving = true;
        if (_animator != null)
        {
            _animator.SetBool("IsMoving", true);
        }
    }

    public override void GetDamage(float amount)
    {
        reEscaledLifeAmount -= reEscaledDamageAmount;
        if (reEscaledLifeAmount <= reEscaledDamageAmount || reEscaledLifeAmount < 0.01f)
        {
            reEscaledLifeAmount = 0;
        }
        _lifeUI.fillAmount = reEscaledLifeAmount;
        Debug.Log("Ground enemy received damage. Remaining life: " + reEscaledLifeAmount);

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
        Debug.Log("Ground enemy has been defeated.");
        StartCoroutine(DeactivateAfterDeath());
    }

    private IEnumerator DeactivateAfterDeath()
    {
        yield return new WaitForSeconds(0.5f);
        this.gameObject.SetActive(false);
    }
}