using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFallingIntro : MonoBehaviour
{
    public Dialogs _dialogs;

    public void DoDialog(ENUM_CharTypeDialogs characterImportance, Dialogs dialogs)
    {
        ManagerLocator.GetDialogsManager().SetDialog(ManagerLocator.GetDialogsManager().ChangeDialogColor(characterImportance), dialogs);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            DoDialog(ENUM_CharTypeDialogs.secondChar, _dialogs);
        }
        gameObject.SetActive(false);
    }
}
