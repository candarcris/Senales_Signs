using System;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public static event Action OnRockHitPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnRockHitPlayer?.Invoke();
            RockPool.Instance.ReturnRock(this.gameObject);
        }
        else if (other.CompareTag("Floor"))
        {
            RockPool.Instance.ReturnRock(this.gameObject);
        }
    }
}
