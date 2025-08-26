using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

[System.Serializable]
public class PlayerController : MonoBehaviour, IDamageable
{
    private InputActions inputActions;
    private InputAction jumpAction;
    private InputAction moveAction;
    private InputAction prayAction;
    private InputAction actionAction;
    private InputAction combatAction;

    [SerializeField] private GameManager gameManager;

    [Header("Movimiento")]
    [Space]
    public bool _sePuedeMover;
    public bool _isLookingRight = true;
    [SerializeField] private float _HorizontalMove = 0f;
    [SerializeField] private float _movementVelocity;
    [SerializeField] private float _moveSoftener;

    [Header("Salto")]
    [Space]
    [SerializeField] private float _jumpForce; // fuerza que se le anade al rigidbody para el salto
    [SerializeField] private float _fallMultiplier; // Ajusta este valor seg�n sea necesario
    [SerializeField] private LayerMask _whatIsGround; // capa para el suelo
    [SerializeField] private Transform _groundController; // objeto en los pies del personaje que detecta el suelo
    [SerializeField] private Vector3 _boxDimensions; // dimensiones de la caja de los pies del personaje
    [SerializeField] private bool _inGround; // tocando el piso?
    [SerializeField] private float _extraGravityMultiplier; // fuerza extra de gravedad al caer
    [SerializeField] private float _gravityMultiplier = 2.5f; // Ajusta este valor seg�n sea necesario
    [SerializeField] private float _coyoteTime = 0.1f; // Tiempo extra para saltar
    private float _coyoteTimeCounter;

    [Header("Animacion y fisicas")]
    [Space]
    private Animator _animator;
    private Rigidbody _rigidbody;
    public Transform _handPoint;
    public Transform _mainParent;
    [SerializeField] private bool _isFloating;



    [Header("Vida")]
    [Space]

    [SerializeField] private float _maxLife = 100f;
    [SerializeField] private float _currentLife;
    public float CurrentLife => _currentLife;
    public float MaxLife => _maxLife;
    public float LifePercentage => _currentLife / _maxLife;
    public float _damage { get; set; }
    public float _lifeAmount { get; set; }
    public Image _lifeUI;
    public float reEscaledDamageAmount = 0;
    public float reEscaledLifeAmount = 0;

    // Eventos para el sistema de suscripción
    public static event Action<float> OnLifeChanged;
    public static event Action<float> OnDamageReceived;
    public static event Action OnPlayerDeath;


    [Header("Skills")]
    [Space]
    [SerializeField] private HUDManager _hudManager;
    public float _faithMaxAmount;
    public float _faithAmount;
    [SerializeField] private bool _isHolding;
    public bool _canPray;
    public bool _canCombat;

    public static event Action OnHold;
    public static event Action OnDrop;
    public static event Action OnPray;
    public static event Action OnPrayEnd;
    public static event Action OnCombat;

    private void Awake()
    {
        //_sePuedeMover = true;
        _mainParent = this.transform.parent;
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        inputActions = ManagerLocator.GetInputActions();
        gameManager = ManagerLocator.GetGameManager();

        if (inputActions == null)
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
        inputActions = ManagerLocator.GetInputActions();

        // Configurar las acciones una vez que tengamos la referencia
        SetupInputActions();
    }

    private void Start()
    {
        _hudManager = ManagerLocator.GetHUDManager();
        _faithMaxAmount = ReEscale.Normalize(100, 0, 100, 0, 1);
        // Inicializar vida
        _currentLife = _maxLife;
        _lifeAmount = _maxLife;
        reEscaledDamageAmount = ReEscale.Normalize(20, 0, _maxLife, 0, 1);
        reEscaledLifeAmount = ReEscale.Normalize(_currentLife, 0, _maxLife, 0, 1);

        // Actualizar UI inicial
        if (_lifeUI != null)
        {
            _lifeUI.fillAmount = LifePercentage;
        }
        _hudManager.SetFaithAmount(0);

        // Si ya tenemos la referencia, configurar las acciones
        if (inputActions != null)
        {
            SetupInputActions();
        }
    }

    private void SetupInputActions()
    {
        jumpAction = inputActions.PlayerControl.Jump;
        jumpAction.Enable();
        jumpAction.performed += Jump;

        moveAction = inputActions.PlayerControl.Move;
        moveAction.Enable();

        prayAction = inputActions.PlayerControl.Pray;
        prayAction.Enable();
        prayAction.performed += Pray;
        prayAction.canceled += Pray;

        actionAction = inputActions.PlayerControl.Action;
        actionAction.Enable();
        actionAction.performed += GeneralAction;

        combatAction = inputActions.PlayerControl.Combat;
        combatAction.Enable();
        combatAction.performed += Combat;
    }

    private void OnEnable()
    {
        if (inputActions != null)
        {
            SetupInputActions();
        }
    }

    private void OnDisable()
    {
        // Desuscribirse de los eventos
        if (jumpAction != null)
            jumpAction.performed -= Jump;
        if (prayAction != null)
            prayAction.performed -= Pray;
        if (prayAction != null)
            prayAction.canceled -= Pray;
        if (actionAction != null)
            actionAction.performed -= GeneralAction;
        if (combatAction != null)
            combatAction.performed -= Combat;


        jumpAction.Disable();
        moveAction.Disable();
        prayAction.Disable();
        actionAction.Disable();
        combatAction.Disable();
    }

    public void SetFallingDrag(float falling)
    {
        _rigidbody.drag = falling;
    }

    public void SetAnimation(string stateName, bool stateValue, bool typeBool = false, bool typeFloat = false, bool typeTrigger = false)
    {
        if (typeBool)
        {
            _animator.SetBool(stateName, stateValue);
        }
    }

    public void TakeDamage(int amount, Vector3 direction, Vector3 position)
    {
        if (_currentLife <= 0) return; // Ya está muerto

        _currentLife = Mathf.Max(0, _currentLife - amount);
        reEscaledLifeAmount = ReEscale.Normalize(_currentLife, 0, _maxLife, 0, 1);

        // Actualizar UI
        if (_lifeUI != null)
        {
            _lifeUI.fillAmount = LifePercentage;
        }

        // Disparar eventos para el sistema de suscripción
        OnLifeChanged?.Invoke(LifePercentage);
        OnDamageReceived?.Invoke(amount);

        // Animación de daño
        if (_animator != null)
        {
            _sePuedeMover = false;
            _HorizontalMove = 0;
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0); // Detiene movimiento horizontal

            Vector3 knockbackDir = direction.normalized;
            //transform.position += -knockbackDir * 0.8f;
            //StartCoroutine(Knockback(knockbackDir, 5f, 1f));
            transform.position = Vector3.Lerp(
                transform.position,
                transform.position + (-knockbackDir * 5f),
                Time.deltaTime * 8f // factor de interpolación
            );

            _animator.SetBool("InDamage", true);
        }

        // Verificar muerte
        if (_currentLife <= 0)
        {
            OnPlayerDeath?.Invoke();
            Die();
        }
    }

    IEnumerator Knockback(Vector3 knockbackDir, float distance, float duration)
    {
        Vector3 start = transform.position;
        Vector3 end = start + (-knockbackDir * distance);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end; // asegurar la posición final
    }

    public IEnumerator GetingDamage()
    {
        //StopState();


        yield return new WaitForSeconds(1f);

        _animator.SetBool("InDamage", false);
        _sePuedeMover = true;
    }

    private void Die()
    {
        Debug.Log("Player muerto");
        StopState();
        gameManager.GameOver();

        // Aquí puedes agregar lógica adicional de muerte
        // Por ejemplo, cambiar animación, desactivar colisiones, etc.
    }

    // Método para curar al jugador
    //public void Heal(float amount)
    //{
    //    if (_currentLife <= 0) return; // No curar si está muerto
    //
    //    _currentLife = Mathf.Min(_maxLife, _currentLife + amount);
    //    reEscaledLifeAmount = ReEscale.Normalize(_currentLife, 0, _maxLife, 0, 1);
    //
    //    // Actualizar UI
    //    if (_lifeUI != null)
    //    {
    //        _lifeUI.fillAmount = LifePercentage;
    //    }
    //
    //    // Disparar evento
    //    OnLifeChanged?.Invoke(LifePercentage);
    //}
    //
    //// Método para establecer vida específica
    //public void SetLife(float newLife)
    //{
    //    _currentLife = Mathf.Clamp(newLife, 0, _maxLife);
    //    reEscaledLifeAmount = ReEscale.Normalize(_currentLife, 0, _maxLife, 0, 1);
    //
    //    // Actualizar UI
    //    if (_lifeUI != null)
    //    {
    //        _lifeUI.fillAmount = LifePercentage;
    //    }
    //
    //    // Disparar evento
    //    OnLifeChanged?.Invoke(LifePercentage);
    //}

    public void Pray(InputAction.CallbackContext context)
    {
        if(_sePuedeMover && _canPray)
        {
            if (context.performed)
            {
                _faithAmount = _hudManager.GetFaithFillAmount();
                OnPray?.Invoke();
            }
            else if (context.canceled)
            {
                OnPrayEnd?.Invoke();
            }
        }
        else if (!_canPray && context.performed)
        {
            // Si no se puede orar pero el botón se presiona, cancelar inmediatamente
            OnPrayEnd?.Invoke();
        }
        else if (!_canPray && context.canceled)
        {
            // También cancelar si se suelta el botón y no se puede orar
            OnPrayEnd?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IHangable>(out var hangable) && hangable.HangPoint != null)
        {
            HoldJumpEhyal(hangable.HangPoint);
        }
        if(other.CompareTag("DeadZone"))
        {
            //animacion de muerte
        }
    }

    public void StopState()
    {
        _sePuedeMover = false;
        _HorizontalMove = 0;
        _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0); // Detiene movimiento horizontal
    }

    public void OnMove(float mover)
    {
        if(_sePuedeMover)
        {
            _rigidbody.velocity = new Vector3(mover * _movementVelocity, _rigidbody.velocity.y, 0);

            if ((mover > 0 && !_isLookingRight) || (mover < 0 && _isLookingRight))
            {
                Rotate();
            }

            if (_rigidbody.velocity.y < 0)
            {
                _rigidbody.AddForce(Vector3.down * _extraGravityMultiplier, ForceMode.Acceleration);
            }
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && _sePuedeMover)
        {
            if (_inGround || _coyoteTimeCounter > 0f)
            {
                _inGround = false;
                _coyoteTimeCounter = 0f;
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, 0);
                _rigidbody.AddForce(new Vector3(0f, _jumpForce), ForceMode.Impulse);
            }
        }
    }

    public void HoldJumpEhyal(Transform hangPoint)
    {
        _isHolding = true;
        if (_handPoint != null && hangPoint != null)
        {
            // Calcula el offset entre el pivote del personaje y la mano
            Vector3 offset = this.transform.position - _handPoint.position;
            // Coloca el personaje de modo que la mano coincida con el hangPoint
            this.transform.position = hangPoint.position + offset;
        }
        else
        {
            this.transform.position = hangPoint.position; // fallback
        }
        //_animator.SetTrigger("HoldAir");
        _animator.SetBool("HoldAir2", true);
        this.transform.parent = hangPoint.parent; // O el objeto que prefieras
        _rigidbody.velocity = Vector3.zero; // Detén cualquier movimiento
        _rigidbody.isKinematic = true;      // Desactiva la física
        //_sePuedeMover = false;
        StopState();
        OnHold?.Invoke();
    }

    public void GeneralAction(InputAction.CallbackContext context)
    { 
        if(_isHolding)
        {
            this.transform.parent = _mainParent;
            _animator.SetBool("HoldAir2", false);
            _rigidbody.isKinematic = false;
            _isHolding = false;
            _sePuedeMover = true;
            OnDrop?.Invoke();
        }
    }

    private void Rotate()
    {
        _isLookingRight = !_isLookingRight;
        Vector3 escala = transform.localScale;
        escala.x *= -1; //multiplica escala.x por menos uno
        transform.localScale = escala;
    }

    public void TriggerFloatingAcrossPath(Transform[] waypoints, float speed, float height)
    {
        if (_isFloating) return;
        StartFloatingAcrossPath(waypoints, speed, height);
    }

    private void StartFloatingAcrossPath(Transform[] waypoints, float speed, float height)
    {
        StartCoroutine(FloatAlongPath(waypoints, speed, height));
    }

    private IEnumerator FloatAlongPath(Transform[] waypoints, float speed, float height)
    {
        if (_isFloating) yield break;

        _isFloating = true;
        yield return new WaitForSeconds(1f);

        _rigidbody.isKinematic = true;
        StopState();

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("No hay waypoints configurados");
            yield break;
        }

        try
        {
            // Mover el player a través de cada waypoint
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null) continue; // Saltar waypoints nulos

                Vector3 targetPoint = waypoints[i].position;
                targetPoint.y += height;

                float timeout = 10f; // Timeout de 10 segundos por waypoint
                float elapsed = 0f;

                while (Vector3.Distance(transform.position, targetPoint) > 0.1f)
                {
                    if (elapsed > timeout)
                    {
                        Debug.LogWarning("Timeout alcanzado en waypoint " + i);
                        break;
                    }

                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        targetPoint,
                        Time.deltaTime * speed
                    );

                    elapsed += Time.deltaTime;
                }

                transform.position = targetPoint;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error en FloatAlongPath: " + e.Message);
        }
        finally
        {
            ResetFloatingState();
        }
    }

    private void ResetFloatingState()
    {
        _rigidbody.isKinematic = false;
        _sePuedeMover = true;
        _isFloating = false;
    }

    private void Update()
    {
        _HorizontalMove = moveAction.ReadValue<float>();

        if (_sePuedeMover) 
        { 
            _animator.SetFloat("Horizontal", Mathf.Abs(_HorizontalMove));
            _animator.SetFloat("VelocityY", _rigidbody.velocity.y);
        }
        else
        {
            _animator.SetFloat("Horizontal", 0); // Asegura que la animación de caminar pare
        }
        
        // Coyote time simple
        if (!_inGround)
        {
            _coyoteTimeCounter -= Time.deltaTime;
        }
    }

    public void Combat(InputAction.CallbackContext context)
    {
        if (_canCombat)
        {
            OnCombat?.Invoke();
        }
    }

    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapBox(_groundController.position, _boxDimensions, Quaternion.identity, _whatIsGround);
        _inGround = colliders.Length > 0;
        
        _animator.SetBool("InGround", _inGround);
        OnMove(_HorizontalMove);

        // Gravedad mejorada
        if (!_inGround) 
        { 
            float gravityMultiplier = _rigidbody.velocity.y < 0 ? _fallMultiplier : _gravityMultiplier;
            _rigidbody.AddForce(Vector3.down * Physics.gravity.magnitude * gravityMultiplier);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_groundController.position, _boxDimensions);
    }
}

[CustomEditor(typeof(PlayerController))]
public class PLayerControllerEditor : Editor
{
    void OnSceneGUI()
    {
        Handles.color = Color.yellow;
        PlayerController miScript = (PlayerController)target;
        var lookingDirection = (miScript._isLookingRight ? Vector3.right : Vector3.left) * 2;
        Handles.DrawLine(miScript.transform.position, miScript.transform.position + lookingDirection);
    }
}

