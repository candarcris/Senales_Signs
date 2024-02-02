using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogPanelUI : MonoBehaviour
{
    public Button _nextButton;
    public Color _mainDialogColor, _secondDialogColor, _thirdDialogColor;

    public void ShowDialog()
    {
        FindObjectOfType<PlayerController>()._sePuedeMover = false;
        this.gameObject.SetActive(true);
    }

    public void HideDialog()
    {
        FindObjectOfType<PlayerController>()._sePuedeMover = true;
        this.gameObject.SetActive(false);
    }

    public void NextPhrase()
    {
        ManagerLocator.GetDialogsManager().NextPhrase();
    }
}
