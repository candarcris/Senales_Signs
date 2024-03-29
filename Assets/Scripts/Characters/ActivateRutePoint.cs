using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateRutePoint : TriggerEvent
{
    protected override void DoTriggerEvent()
    {
        FindObjectOfType<EyalController>().StartMovement();
        gameObject.SetActive(false);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            DoTriggerEvent();
        }
    }
}
