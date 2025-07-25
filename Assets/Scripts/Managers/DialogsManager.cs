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
    private float _timeLettersAnim = 0.05f;

    private bool _isAnimating = false;
    private Coroutine _currentCoroutine = null;
    private string _currentPhrase = "";

    public Action OnFinishDialog;

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
        // Deshabilita los controles del jugador y habilita los de UI
        _inputActions.PlayerControl.Disable();
        _inputActions.UI.Enable();
        //FindObjectOfType<PlayerController>()._sePuedeMover = false;
        _dialogPanel.gameObject.SetActive(true);
    }

    private void HideDialogPanel()
    {
        // Habilita los controles del jugador y deshabilita los de UI
        _inputActions.UI.Disable();
        _inputActions.PlayerControl.Enable();
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
        ShowNextPhrase();
    }

    private void ShowNextPhrase()
    {
        if (_queueDialogs.Count > 0)
        {
            _currentPhrase = _queueDialogs.Dequeue();
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);
            _currentCoroutine = StartCoroutine(AnimateLetters(_currentPhrase));
        }
        else
        {
            FinishDialog();
        }
    }

    private void OnNextPhrase(InputAction.CallbackContext context)
    {
        NextPhrase();
    }

    private void NextPhrase()
    {
        if (_isAnimating)
        {
            // Si la animación está corriendo, mostrar el texto completo inmediatamente
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);
            _dialogPanel._screenText.text = _currentPhrase;
            _isAnimating = false;
            _dialogPanel._nextButton.gameObject.SetActive(true);
        }
        else
        {
            // Si la animación terminó, pasar a la siguiente frase
            _dialogPanel._nextButton.gameObject.SetActive(false);
            ShowNextPhrase();
        }
    }

    private void FinishDialog()
    {
        //Se ejecutan todas las funciones suscritas a esta
        HideDialogPanel();
        OnFinishDialog?.Invoke();
    }

    private IEnumerator AnimateLetters(string showText)
    {
        _isAnimating = true;
        _dialogPanel._screenText.text = "";
        _dialogPanel._screenText.color = _color;

        foreach (char character in showText.ToCharArray())
        {
            _dialogPanel._screenText.text += character;
            yield return new WaitForSeconds(_timeLettersAnim);
            if (!_isAnimating) yield break; // Si se interrumpe la animación, salir
        }

        _isAnimating = false;
        _dialogPanel._nextButton.gameObject.SetActive(true);
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
