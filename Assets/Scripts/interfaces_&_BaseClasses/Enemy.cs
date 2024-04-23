using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] public float _damage { get; set; }
    [SerializeField] public float _lifeAmount { get; set; }
    [SerializeField] public Image _lifeUI;
    public Enemy(float damage, float lifeAmount)
    {
        _damage = damage;
        _lifeAmount = lifeAmount;
    }

    public abstract void Attack();
    public abstract void GetDamage(float amount);
}
