using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public class PlayerController : MonoBehaviour
{
    private InputActions inputActions;
    private InputAction jumpAction;
    private InputAction moveAction;
    private InputAction prayAction;
    private InputAction actionAction;

    [Header("Movimiento")]
    [Space]
    public bool _sePuedeMover;
    public bool _isLookingRight = true;
    [SerializeField] private float _HorizontalMove = 0f;
    [SerializeField] private float _movementVelocity;
    [SerializeField] private float _moveSoftener;
    [SerializeField] private Vector3 _velocity = Vector3.zero;

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

    [Header("Animacion y fisicas")]
    [Space]
    private Animator _animator;
    private Rigidbody _rigidbody;
    public Transform _handPoint;
    public Transform _mainParent;

    [Header("Skills")]
    [Space]
    [SerializeField] private HUDManager _hudManager;
    public float _faithMaxAmount;
    public float _faithAmount;
    private bool _isHolding;

    public static event Action OnHold;
    public static event Action OnDrop;

    private void Awake()
    {
        //_sePuedeMover = true;
        _mainParent = this.transform.parent;
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        inputActions = new InputActions();
    }

    private void Start()
    {
        _hudManager = ManagerLocator.GetHUDManager();
        _faithMaxAmount = ReEscale.Normalize(100, 0, 100, 0, 1);
        _hudManager.SetFaithAmount(0);
    }

    private void OnEnable()
    {
        jumpAction = inputActions.PlayerControl.Jump;
        jumpAction.Enable();
        jumpAction.performed += OnJump;

        moveAction = inputActions.PlayerControl.Move;
        moveAction.Enable();

        prayAction = inputActions.PlayerControl.Pray;
        prayAction.Enable();
        prayAction.performed += OnPray;

        actionAction = inputActions.PlayerControl.Action;
        actionAction.Enable();
        actionAction.performed += OnAction;
    }

    private void OnDisable()
    {
        jumpAction.Disable();
        moveAction.Disable();
        prayAction.Disable();
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

    public void OnPray(InputAction.CallbackContext context)
    {
        if(_sePuedeMover)
        {
            Debug.Log("Orando");
            _faithAmount = _hudManager.GetFaithFillAmount();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IHangable>(out var hangable) && hangable.HangPoint != null)
        {
            HoldJumpEhyal(hangable.HangPoint);
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

    public void OnJump(InputAction.CallbackContext context)
    {
        if (_inGround && _sePuedeMover)
        {
            _inGround = false;
            _rigidbody.velocity = new Vector2(0, 0); // Resetea la velocidad vertical antes de saltar
            _rigidbody.AddForce(new Vector2(0f, _jumpForce));
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
        _sePuedeMover = false;
        OnHold?.Invoke();
    }

    public void OnAction(InputAction.CallbackContext context)
    { 
        if(_isHolding)
        {
            Debug.Log("Drop ejecutandose");
            this.transform.parent = _mainParent;
            _animator.SetBool("HoldAir2", false);
            //this.transform.parent = hangPoint.parent; // O el objeto que prefieras
            _rigidbody.isKinematic = false;      // Desactiva la física
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
    }

    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapBox(_groundController.position, _boxDimensions, Quaternion.identity, _whatIsGround);
        _inGround = colliders.Length > 0;
        _animator.SetBool("InGround", _inGround);
        OnMove(_HorizontalMove);

        if (!_inGround) 
        { 
            _rigidbody.AddForce(Vector3.down * Physics.gravity.magnitude * _gravityMultiplier);
            if (_rigidbody.velocity.y < 0)
            {
                Vector2 gravityModifier = Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1) * Time.fixedDeltaTime * 2f;
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y + gravityModifier.y);
            }
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

