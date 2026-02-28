using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    public float _damage { get; set; }
    public float _lifeAmount { get; set; }
    public Image _lifeUI;

    protected Animator _animator;
    [SerializeField] protected Rigidbody _rigidbody;
    [SerializeField] protected bool _isMoving = true;

    public System.Action OnDeath;

    public Enemy(float damage, float lifeAmount)
    {
        _damage = damage;
        _lifeAmount = lifeAmount;
    }

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        _isMoving = true;
        if (_animator != null)
        {
            _animator.SetBool("IsMoving", _isMoving);
        }
    }

    public abstract void Attack();
    public abstract void GetDamage(float amount);

    protected virtual void OnEnemyDeath()
    {
        // L�gica base cuando muere cualquier enemigo
        OnDeath?.Invoke();
    }

    protected virtual void StopMovement()
    {
        _isMoving = false;
        if (_animator != null)
        {
            _animator.SetBool("IsMoving", false);
        }
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
        }
    }
}
