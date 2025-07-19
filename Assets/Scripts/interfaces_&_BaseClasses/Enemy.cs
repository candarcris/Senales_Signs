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
    protected Rigidbody _rigidbody;
    protected bool _isMoving = true;

    public Enemy(float damage, float lifeAmount)
    {
        _damage = damage;
        _lifeAmount = lifeAmount;
    }

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();

        if (_rigidbody != null)
        {
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            // Congela todas las rotaciones para evitar inclinaciones
        }
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
