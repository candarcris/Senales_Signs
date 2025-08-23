using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIInstructivePray : UIInstructives
{
    public Dialogs _dialogs;
    public ENUM_CharTypeDialogs _type;

    public void DoDialog()
    {
        ManagerLocator.GetDialogsManager().DoDialog(_type, _dialogs);
    }

    protected override void OnShowInstruction()
    {
        DoDialog();
        ManagerLocator.GetHUDManager()?._faithBar2?.gameObject.SetActive(true);
    }
}

