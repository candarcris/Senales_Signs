using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimDialogs : MonoBehaviour
{
    public string _text;

    public Dialog _dialog;
    public Transform _parent;

    public void setDialog()
    {
        Instantiate(_dialog, _parent);
        _dialog.SetDialogText(_text);
        _dialog.ShowDialog();
    }
}
