using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFallingIntro : MonoBehaviour, IDialog
{
    public Dialogs _dialogs;
    public void DoDialog()
    {
        ManagerLocator.GetDialogsManager().SetDialog(ManagerLocator.GetDialogsManager().ChangeDialogColor(ENUM_CharTypeDialogs.mainChar), _dialogs);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            DoDialog();
        }
        gameObject.SetActive(false);
    }
}
