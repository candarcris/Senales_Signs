using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class WormBossEnemy : Enemy
{
    public WormBossEnemy(float damage, float lifeAmount) : base(damage, lifeAmount) { }

    public bool _isWormFollowing = false;
    public float _moveSpeed;

    public float reEscaledLifeAmount = 0;
    public override void Attack()
    {

    }

    protected override void Start()
    {
        WormBossEnemy newBossEnemy = new WormBossEnemy(10f, 100f);
        _lifeAmount = newBossEnemy._lifeAmount;
        reEscaledLifeAmount = ReEscale.Normalize(_lifeAmount, 0, _lifeAmount, 0, 1);
    }

    public override void GetDamage(float amount)
    {
        if (reEscaledLifeAmount <= 0) return; // Ya está muerto

        // Usar el amount recibido en lugar del daño fijo
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
        StartCoroutine(SlowDownHit());
    }

    IEnumerator SlowDownHit()
    {
        _moveSpeed = 0.1f;
        yield return new WaitForSeconds(1);
        _moveSpeed = 0.3f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EhyalPower"))
        {
            GetDamage(20);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];

            Vector3 pushDir = contact.normal;
            if (damageable != null)
            {
                damageable.TakeDamage(20, pushDir);
            }
        }
    }
}
