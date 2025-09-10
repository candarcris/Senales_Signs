using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogsManager : MonoBehaviour
{
    private GameManager gameManager;
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
        _inputActions = ManagerLocator.GetInputActions();

        // Si GameManager a�n no est� inicializado, esperar
        if (_inputActions == null)
        {
            StartCoroutine(WaitForGameManager());
        }
    }
    private IEnumerator WaitForGameManager()
    {
        while (GameManager._sharedInstance == null)
        {
            yield return null;
        }
        _inputActions = ManagerLocator.GetInputActions();

        // Configurar las acciones una vez que tengamos la referencia
        SetupInputActions();
    }

    private void SetupInputActions()
    {
        if (_inputActions == null) return;

        // Limpiar suscripción anterior si existe
        if (_submitAction != null)
        {
            _submitAction.performed -= OnNextPhrase;
            _submitAction.Disable();
        }

        _submitAction = _inputActions.UI.Submit;
        _submitAction.Enable();
        _submitAction.performed += OnNextPhrase;
    }

    private void Start()
    {
        gameManager = ManagerLocator.GetGameManager();

        // Si ya tenemos la referencia, configurar las acciones
        if (_inputActions != null)
        {
            SetupInputActions();
        }
    }

    private void OnEnable()
    {
        // Solo configurar si ya tenemos la referencia
        if (_inputActions != null)
        {
            SetupInputActions();
        }
        else
        {
            // Si no tenemos la referencia, esperar a que esté disponible
            StartCoroutine(WaitForInputActionsAndSetup());
        }
        _dialogPanel._nextButton.onClick.AddListener(NextPhrase);
    }

    private IEnumerator WaitForInputActionsAndSetup()
    {
        // Esperar hasta que tengamos la referencia a InputActions
        while (_inputActions == null)
        {
            _inputActions = ManagerLocator.GetInputActions();
            yield return null;
        }

        // Una vez que tenemos la referencia, configurar los inputs
        SetupInputActions();
    }

    private void OnDisable()
    {
        //InputSystem.onDeviceChange -= OnDeviceChange;
        _dialogPanel._nextButton.onClick.RemoveListener(NextPhrase);
        _submitAction.performed -= OnNextPhrase;
    }

    private void OnDestroy()
    {
        // Limpiar todos los eventos al destruir el objeto
        if (_submitAction != null)
        {
            _submitAction.performed -= OnNextPhrase;
            _submitAction.Disable();
        }

        if (_dialogPanel != null && _dialogPanel._nextButton != null)
        {
            _dialogPanel._nextButton.onClick.RemoveListener(NextPhrase);
        }

        // Limpiar el evento personalizado
        OnFinishDialog = null;
    }

    //private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    //{
    //    if (device is Keyboard)
    //    {
    //        // Verifica si se presion� un bot�n del teclado
    //        var keyboard = (Keyboard)device;
    //        if (keyboard.anyKey.isPressed)
    //        {
    //            Debug.Log("Se presion� un bot�n del teclado: " + device.name);
    //        }
    //    }
    //    if (device is Gamepad)
    //    {
    //        // Verifica si se presion� un bot�n del gamepad
    //        var gamepad = (Gamepad)device;
    //        if (gamepad.IsPressed())
    //        {
    //            Debug.Log("Se presion� un bot�n del gamepad: " + device.name);
    //        }
    //    }
    //}
    public void DoDialog(ENUM_CharTypeDialogs characterImportance, Dialogs dialogs)
    {
        // Asegurar que los inputs estén configurados antes de iniciar el diálogo
        if (_inputActions == null)
        {
            _inputActions = ManagerLocator.GetInputActions();
            SetupInputActions();
        }
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
        // Asegurar que los inputs estén configurados antes de cambiar el contexto
        if (_inputActions == null)
        {
            _inputActions = ManagerLocator.GetInputActions();
            SetupInputActions();
        }

        // Deshabilita los controles del jugador y habilita los de UI
        gameManager.SetContext(Context.UI);
        FindObjectOfType<PlayerController>().StopState();
        _dialogPanel.gameObject.SetActive(true);
    }

    private void HideDialogPanel()
    {
        // Habilita los controles del jugador y deshabilita los de UI
        gameManager.SetContext(Context.Player);
        //_inputActions.UI.Disable();
        //_inputActions.PlayerControl.Enable();
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
            // Si la animaci�n est� corriendo, mostrar el texto completo inmediatamente
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);
            _dialogPanel._screenText.text = _currentPhrase;
            _isAnimating = false;
            _dialogPanel._nextButton.gameObject.SetActive(true);
        }
        else
        {
            // Si la animaci�n termin�, pasar a la siguiente frase
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
            if (!_isAnimating) yield break; // Si se interrumpe la animaci�n, salir
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
