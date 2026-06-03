using Signs;
using System;
using UnityEngine;

public class Rock : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IDamage>(out IDamage damage))
        {
            damage.RecibirImpacto(25);
            RockPool.Instance.ReturnRock(this.gameObject);
        }

        else if (other.CompareTag("Floor"))
        {
            RockPool.Instance.ReturnRock(this.gameObject);
        }
    }
}
