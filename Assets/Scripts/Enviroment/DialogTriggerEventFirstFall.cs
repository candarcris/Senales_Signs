using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Dialogs))]
public class DialogTriggerEventFirstFall : TriggerEvent
{
    public Dialogs _dialogs;
    public ENUM_CharTypeDialogs _type;
    public PlayerController _playerController;

    public void SagarContinueFalling()
    {
        _playerController.SetFallingDrag(6);
    }

    protected override void DoTriggerEvent()
    {
        ManagerLocator.GetDialogsManager().OnFinishDialog = null;
        ManagerLocator.GetDialogsManager().OnFinishDialog += SagarContinueFalling;
        ManagerLocator.GetDialogsManager().DoDialog(_type, _dialogs);
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
