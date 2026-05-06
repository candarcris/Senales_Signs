using System;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public static event Action OnRockHitPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //PlayerControllerSigns playerScript = other.gameObject.GetComponent<PlayerControllerSigns>();
            OnRockHitPlayer?.Invoke();

            //if (playerScript != null)
            //{
            //    playerScript.RecibirImpacto();
            //}
            RockPool.Instance.ReturnRock(this.gameObject);
        }
        else if (other.CompareTag("Floor"))
        {
            RockPool.Instance.ReturnRock(this.gameObject);
        }
    }
}
