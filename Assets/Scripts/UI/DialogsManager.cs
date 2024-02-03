using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static DialogEvent;

public class DialogsManager : MonoBehaviour
{
    public DialogPanelUI _dialogPanel;
    public Queue <string> _queueDialogs = new();
    Dialogs _dialogs;
    private Color _color;
    [SerializeField] private TextMeshProUGUI _screenText;
    public static event Action OnFinishDialog;

    public void SetDialog(Dialogs dialogEvent, Color color)
    {
        _color = color;
        _dialogPanel.ShowDialog();
        _dialogs = dialogEvent;
        SetText();
    }

    public void SetText()
    {
        _queueDialogs.Clear();
        foreach (string keepText in _dialogs.arrayTexts)
        {
            _queueDialogs.Enqueue(keepText);
        }
        NextPhrase();
    }

    public void NextPhrase()
    {
        if(_queueDialogs.Count == 0)
        {
            FinishDialog();
            return;
        }
        string currentPhrase = _queueDialogs.Dequeue();
        _screenText.text = currentPhrase;
        StartCoroutine(ShowCharacters(currentPhrase));
    }

    public static void FinishDialog()
    {
        //Se ejecutan todas las funciones suscritas a esta
        OnFinishDialog?.Invoke();
    }

    public IEnumerator ShowCharacters(string showText)
    {
        _screenText.text = "";
        _screenText.color = _color;
        int index = 0;

        foreach (char character in showText.ToCharArray())
        {
            _screenText.text += character;
            yield return new WaitForSeconds(0.05f);
            index++;

            // Verificar si se han mostrado todos los caracteres
            if (index == showText.Length)
            {
                // La palabra showText ha sido completamente mostrada
                _dialogPanel._nextButton.gameObject.SetActive(true);
            }
        }
    }
}
