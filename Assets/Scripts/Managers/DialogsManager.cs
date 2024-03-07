using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogsManager : MonoBehaviour
{
    private InputActions inputActions;
    private InputAction submitAction;

    public DialogPanelUI _dialogPanel;
    public Queue <string> _queueDialogs = new();
    public Dialogs _dialogs;
    private Color _color;

    private void Awake()
    {
        inputActions = new InputActions();

    }
    private void Start()
    {
        
    }

    private void OnEnable()
    {
        //InputSystem.onDeviceChange += OnDeviceChange;
        submitAction = inputActions.UI.Submit;
        submitAction.Enable();
        
    }

    private void OnDisable()
    {
        //InputSystem.onDeviceChange -= OnDeviceChange;
    }

    //private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    //{
    //    if (device is Keyboard)
    //    {
    //        // Verifica si se presionó un botón del teclado
    //        var keyboard = (Keyboard)device;
    //        if (keyboard.anyKey.isPressed)
    //        {
    //            Debug.Log("Se presionó un botón del teclado: " + device.name);
    //        }
    //    }
    //    if (device is Gamepad)
    //    {
    //        // Verifica si se presionó un botón del gamepad
    //        var gamepad = (Gamepad)device;
    //        if (gamepad.IsPressed())
    //        {
    //            Debug.Log("Se presionó un botón del gamepad: " + device.name);
    //        }
    //    }
    //}

    public void SetDialog(Color color, Dialogs dialogs)
    {
        _color = color;
        _dialogs = dialogs;
        SetText();
        ShowDialogPanel();
    }

    private void ShowDialogPanel()
    {
        FindObjectOfType<PlayerController>()._sePuedeMover = false;
        _dialogPanel.gameObject.SetActive(true);
    }

    private void HideDialogPanel()
    {
        FindObjectOfType<PlayerController>()._sePuedeMover = true;
        _dialogPanel.gameObject.SetActive(false);
    }

    private void SetText()
    {
        _queueDialogs.Clear();
        foreach (string keepText in _dialogs.arrayTexts)
        {
            _queueDialogs.Enqueue(keepText);
        }
        string currentPhrase = _queueDialogs.Dequeue();
        _dialogPanel._screenText.text = currentPhrase;
        StartCoroutine(AnimateLetters(currentPhrase));
    }

    private void OnNextPhrase(InputAction.CallbackContext context)
    {
        NextPhrase();
    }

    private void NextPhrase()
    {
        if(_queueDialogs.Count == 0)
        {
            FinishDialog();
            return;
        }
        string currentPhrase = _queueDialogs.Dequeue();
        StartCoroutine(AnimateLetters(currentPhrase));
    }

    private void FinishDialog()
    {
        //Se ejecutan todas las funciones suscritas a esta
        HideDialogPanel();
    }

    private IEnumerator AnimateLetters(string showText)
    {
        _dialogPanel._screenText.text = "";
        _dialogPanel._screenText.color = _color;
        int index = 0;
        float time = 0.05f;

        foreach (char character in showText.ToCharArray())
        {
            _dialogPanel._screenText.text += character;
            yield return new WaitForSeconds(time);
            index++;

            // Verificar si se han mostrado todos los caracteres
            if (index == showText.Length)
            {
                // La palabra showText ha sido completamente mostrada
                _dialogPanel._nextButton.onClick.AddListener(NextPhrase);
                submitAction.performed += OnNextPhrase;
                time = 0.05f;
            }
            else
            {
                _dialogPanel._nextButton.onClick.RemoveListener(NextPhrase);
                submitAction.performed -= OnNextPhrase;
                time = 0.01f;
            }
        }
    }

    public Color ChangeDialogColor(ENUM_CharTypeDialogs colorChar)
    {
        Color color = _dialogPanel._mainDialogColor;
        switch (colorChar)
        {
            case ENUM_CharTypeDialogs.mainChar:
                _dialogPanel.charType = ENUM_CharTypeDialogs.mainChar;
                color = _dialogPanel._mainDialogColor;
                break;
            case ENUM_CharTypeDialogs.secondChar:
                _dialogPanel.charType = ENUM_CharTypeDialogs.secondChar;
                color = _dialogPanel._secondDialogColor;
                break;
            case ENUM_CharTypeDialogs.thirdChar:
                _dialogPanel.charType = ENUM_CharTypeDialogs.thirdChar;
                color = _dialogPanel._thirdDialogColor;
                break;
        }
        return color;
    }
}
