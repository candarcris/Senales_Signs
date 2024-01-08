using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAnimDialog : Event
{
    public override void ExecuteEvent()
    {
        ManagerLocator.GetDialogsManager()._queueDialogsList.Clear();

        ManagerLocator.GetDialogsManager()._queueDialogsList.Add("No se donde estoy");
        ManagerLocator.GetDialogsManager()._queueDialogsList.Add("pero siento mucha paz, quisiera estar así por siempre");
        ManagerLocator.GetDialogsManager()._queueDialogsList.Add("...");

        ManagerLocator.GetDialogsManager().setDialog();
    }
}
