using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateRutePoint : TriggerEvent
{
    protected override void DoTriggerEvent()
    {
        FindObjectOfType<EyalController>().Movement();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            DoTriggerEvent();
        }
    }
}
