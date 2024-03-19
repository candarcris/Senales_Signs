using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Dialogs))]
public class DialogTriggerEvent : TriggerEvent
{
    public Dialogs _dialogs;
    public ENUM_CharTypeDialogs _type;

    protected override void DoTriggerEvent()
    {
        ManagerLocator.GetDialogsManager().DoDialog(_type, _dialogs);
        gameObject.SetActive(false);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            DoTriggerEvent();
        }
    }
}
