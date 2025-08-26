using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] public float _damage { get; set; }
    [SerializeField] public float _lifeAmount { get; set; }
    [SerializeField] public Image _lifeUI;

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
        // Lógica base cuando muere cualquier enemigo
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
            _rigidbody.velocity = Vector3.zero;
        }
    }
}
