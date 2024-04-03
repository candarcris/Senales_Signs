using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GroundEnemy : Enemy
{
    public float reEscaledDamageAmount = 0;
    public float reEscaledLifeAmount = 0;
    public GroundEnemy(float damage, float lifeAmount) : base(damage, lifeAmount)
    {
        //Toda la implementación específica para un enemigo tipo GroundEnemy (animacion, sonido, etc...)
    }

    private void Start()
    {
        AdjustScale(20);// este valor es el entrante, no debe ser definido sino referenciado
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GetDamage(20); // este valor es el entrante, no debe ser definido sino referenciado
        }
    }

    public override void Attack()
    {
        
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
        // Implementación de lo que sucede cuando el enemigo terrestre muere
        Debug.Log("Ground enemy has been defeated.");
        // animacion de muerte, 
        this.gameObject.SetActive(false);
    }

    public override void AdjustScale(float amount)
    {
        reEscaledDamageAmount = ReEscale.Normalize(amount, 0, _lifeAmount, 0, 1);
        reEscaledLifeAmount = ReEscale.Normalize(_lifeAmount, 0, _lifeAmount, 0, 1);
    }
}
