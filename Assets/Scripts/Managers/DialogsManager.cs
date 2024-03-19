using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogsManager : MonoBehaviour
{
    private InputActions _inputActions;
    private InputAction _submitAction;

    public DialogPanelUI _dialogPanel;
    public Queue <string> _queueDialogs = new();
    public Dialogs _dialogs;
    private Color _color;
    public bool _nextButtonPressed = false;
    private float _timeLettersAnim = 0.05f;

    private void Awake()
    {
        _inputActions = new InputActions();

    }
    private void Start()
    {
        
    }

    private void OnEnable()
    {
        //InputSystem.onDeviceChange += OnDeviceChange;
        _submitAction = _inputActions.UI.Submit;
        _submitAction.Enable();
        _dialogPanel._nextButton.onClick.AddListener(NextPhrase);
        _submitAction.performed += OnNextPhrase;
    }

    private void OnDisable()
    {
        //InputSystem.onDeviceChange -= OnDeviceChange;
        _dialogPanel._nextButton.onClick.RemoveListener(NextPhrase);
        _submitAction.performed -= OnNextPhrase;
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
    public void DoDialog(ENUM_CharTypeDialogs characterImportance, Dialogs dialogs)
    {
        SetDialog(ChangeDialogColor(characterImportance), dialogs);
    }

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
        StartCoroutine(AnimateLetters(currentPhrase));
    }

    private void OnNextPhrase(InputAction.CallbackContext context)
    {
        NextPhrase();
    }

    private void NextPhrase()
    {
        _nextButtonPressed = !_nextButtonPressed;
        if (_nextButtonPressed == false)
        {
            if (_queueDialogs.Count == 0)
            {
                FinishDialog();
                return;
            }
            _timeLettersAnim = 0.05f;
            string currentPhrase = _queueDialogs.Dequeue();
            StartCoroutine(AnimateLetters(currentPhrase));
        }
        else
        {
            _timeLettersAnim = 0.01f;
            _dialogPanel._nextButton.onClick.RemoveListener(NextPhrase);
            _submitAction.performed -= OnNextPhrase;
            _dialogPanel._nextButton.gameObject.SetActive(false);
        }
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

        foreach (char character in showText.ToCharArray())
        {
            _dialogPanel._screenText.text += character;
            yield return new WaitForSeconds(_timeLettersAnim);
            index++;

            // Verificar si se han mostrado todos los caracteres
            if (index == showText.Length)
            {
                // La palabra showText ha sido completamente mostrada
                _dialogPanel._nextButton.onClick.AddListener(NextPhrase);
                _submitAction.performed += OnNextPhrase;
                _dialogPanel._nextButton.gameObject.SetActive(true);
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
