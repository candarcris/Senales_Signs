using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogEvent : MonoBehaviour
{
    public Dialogs _dialogs;
    public enum ENUM_CharTypeDialogs
    {
        mainChar,
        secondChar,
        thirdChar
    }
    public ENUM_CharTypeDialogs charType;

    public Color ChangeCharacterDialogs()
    {
        Color color = ManagerLocator.GetDialogsManager()._dialogPanel._mainDialogColor;
        switch (charType)
        {
            case ENUM_CharTypeDialogs.mainChar:
                charType = ENUM_CharTypeDialogs.mainChar;
                color = ManagerLocator.GetDialogsManager()._dialogPanel._mainDialogColor;
                break;
            case ENUM_CharTypeDialogs.secondChar:
                charType = ENUM_CharTypeDialogs.secondChar;
                color = ManagerLocator.GetDialogsManager()._dialogPanel._secondDialogColor;
                break;
            case ENUM_CharTypeDialogs.thirdChar:
                color = ManagerLocator.GetDialogsManager()._dialogPanel._thirdDialogColor;
                charType = ENUM_CharTypeDialogs.thirdChar;
                break;
        }
        return color;
    }

    public void ExecuteDialogEvent()
    {
        FindObjectOfType<DialogsManager>().SetDialog(_dialogs, ChangeCharacterDialogs());
    }
}
