using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialog : MonoBehaviour
{
    public TMP_Text _text;
    public Button _nextButton;
    public bool _haveNextDialog = false;
    public event Action OnNextDialog;

    public void ShowDialog()
    {
        this.gameObject.SetActive(true);
    }

    public void HideDialog()
    {
        this.gameObject.SetActive(false);
    }

    public void SetDialogText(string dialog)
    {
        _text.SetText(dialog);
    }

    public void NextDialog()
    {
        _text.SetText("");

        if (OnNextDialog != null)
        {
            OnNextDialog.Invoke();
        }
    }
}
