using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class GameOverPanelUI : UIElement
{
    Animator _anim;
    public Button _continuarButton, _exitButton;
    private GameManager gameManager;
    
    [Header("Navigation")]
    private int _currentButtonIndex = 0;
    private Button[] _buttons;
    private InputActions _inputActions;
    private InputAction _navigateAction;
    private InputAction _submitAction;
    
    [Header("Navigation Settings")]
    [SerializeField] private float _navigationCooldown = 0.2f;
    private float _lastNavigationTime = 0f;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _buttons = new Button[] { _continuarButton, _exitButton };
        gameManager = ManagerLocator.GetGameManager();

        // Obtener referencia a InputActions
        _inputActions = ManagerLocator.GetInputActions();
    }

    private void OnEnable()
    {
        _continuarButton.gameObject.SetActive(false);
        _exitButton.gameObject.SetActive(false);
        
        // Configurar inputs cuando se active el panel
        SetupInputActions();
    }

    private void OnDisable()
    {
        // Limpiar inputs cuando se desactive el panel
        CleanupInputActions();
    }

    private void OnDestroy()
    {
        CleanupInputActions();
    }

    private void SetupInputActions()
    {
        if (_inputActions == null)
        {
            _inputActions = ManagerLocator.GetInputActions();
        }
        
        if (_inputActions != null)
        {
            // Limpiar suscripciones anteriores
            if (_navigateAction != null)
            {
                _navigateAction.performed -= OnNavigate;
                _navigateAction.Disable();
            }

            if (_submitAction != null)
            {
                _submitAction.performed -= OnSubmit;
                _submitAction.Disable();
            }

            // Configurar navegación
            _navigateAction = _inputActions.UI.Navigate;
            _navigateAction.Enable();
            _navigateAction.performed += OnNavigate;
            
            // Configurar submit
            _submitAction = _inputActions.UI.Submit;
            _submitAction.Enable();
            _submitAction.performed += OnSubmit;
        }
    }

    private void CleanupInputActions()
    {
        if (_navigateAction != null)
        {
            _navigateAction.performed -= OnNavigate;
            _navigateAction.Disable();
        }
        
        if (_submitAction != null)
        {
            _submitAction.performed -= OnSubmit;
            _submitAction.Disable();
        }
    }

    public void FadeIn()
    {
        _anim.SetTrigger("Dead");
        StartCoroutine(ButtonAppear());
    }

    public void Continuar()
    {
        GameManager._sharedInstance.LoadScene(1);
    }

    public IEnumerator ButtonAppear()
    {
        yield return new WaitForSeconds(0.5f);
        gameManager.SetContext(Context.UI);

        _continuarButton.gameObject.SetActive(true);
        _exitButton.gameObject.SetActive(true);

        // Inicializar selección en el primer botón
        _currentButtonIndex = 0;
        UpdateButtonSelection();
    }

    public void ExitGame()
    {
        GameManager._sharedInstance.LoadScene(0);
    }

    private void OnNavigate(InputAction.CallbackContext context)
    {
        if (Time.time - _lastNavigationTime < _navigationCooldown) return;

        float navigation = context.ReadValue<float>();

        // Navegación horizontal (izquierda/derecha)
        if (Mathf.Abs(navigation) > 0.5f)
        {
            if (navigation > 0)
            {
                // Navegar hacia la derecha (siguiente botón)
                _currentButtonIndex = (_currentButtonIndex + 1) % _buttons.Length;
            }
            else
            {
                // Navegar hacia la izquierda (botón anterior)
                _currentButtonIndex = (_currentButtonIndex - 1 + _buttons.Length) % _buttons.Length;
            }

            UpdateButtonSelection();
            _lastNavigationTime = Time.time;
        }
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Activar el botón seleccionado
            if (_currentButtonIndex >= 0 && _currentButtonIndex < _buttons.Length)
            {
                _buttons[_currentButtonIndex].onClick.Invoke();
                Debug.Log("index: " + _currentButtonIndex);
            }
        }
    }

    private void UpdateButtonSelection()
    {
        // Actualizar la apariencia visual de los botones
        for (int i = 0; i < _buttons.Length; i++)
        {
            if (_buttons[i] != null)
            {
                // Cambiar el color o escala del botón seleccionado
                ColorBlock colors = _buttons[i].colors;
                
                if (i == _currentButtonIndex)
                {
                    // Botón seleccionado
                    colors.normalColor = Color.yellow;
                    colors.selectedColor = Color.yellow;
                    _buttons[i].transform.localScale = Vector3.one * 1.1f;
                }
                else
                {
                    // Botón no seleccionado
                    colors.normalColor = Color.white;
                    colors.selectedColor = Color.white;
                    _buttons[i].transform.localScale = Vector3.one;
                }
                
                _buttons[i].colors = colors;
            }
        }
        
        // Opcional: Hacer focus en el botón seleccionado para el EventSystem
        if (_currentButtonIndex >= 0 && _currentButtonIndex < _buttons.Length)
        {
            EventSystem.current.SetSelectedGameObject(_buttons[_currentButtonIndex].gameObject);
        }
    }
}
