using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public class PlayerController : MonoBehaviour
{
    private InputActions inputActions;
    private InputAction jumpAction;
    private InputAction moveAction;
    private InputAction prayAction;

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
    [SerializeField] private float _fallMultiplier; // Ajusta este valor según sea necesario
    [SerializeField] private LayerMask _whatIsGround; // capa para el suelo
    [SerializeField] private Transform _groundController; // objeto en los pies del personaje que detecta el suelo
    [SerializeField] private Vector3 _boxDimensions; // dimensiones de la caja de los pies del personaje
    [SerializeField] private bool _inGround; // tocando el piso?
    [SerializeField] private float _extraGravityMultiplier; // fuerza extra de gravedad al caer
    [SerializeField] private float _gravityMultiplier = 2.5f; // Ajusta este valor según sea necesario

    [Header("Animacion y fisicas")]
    [Space]
    private Animator _animator;
    private Rigidbody _rigidbody;

    [Header("Skills")]
    [Space]
    [SerializeField] private HUDManager _hudManager;
    public float _faithMaxAmount = 0;
    public float _faithAmount = 0;

    private void Awake()
    {
        _sePuedeMover = true;
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        inputActions = new InputActions();
        _hudManager = ManagerLocator.GetHUDManager();
    }

    private void Start()
    {
        _faithMaxAmount = ReEscale.Normalize(100, 0, 100, 0, 1);
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
    }

    private void OnDisable()
    {
        jumpAction.Disable();
        moveAction.Disable();
        prayAction.Disable();
    }

    public void OnPray(InputAction.CallbackContext context)
    {
        if(_sePuedeMover)
        {
            Debug.Log("Orando");
            _faithAmount = _hudManager.GetFaithFillAmount();
        }
    }

    public void OnMove(float mover)
    {
        if(_sePuedeMover)
        {
            Vector3 targetVelocity = new Vector3(mover, _rigidbody.velocity.y);
            _rigidbody.velocity = Vector3.SmoothDamp(_rigidbody.velocity, targetVelocity, ref _velocity, _moveSoftener);

            if ((_HorizontalMove > 0 && !_isLookingRight) || (_HorizontalMove < 0 && _isLookingRight))
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

    private void Rotate()
    {
        _isLookingRight = !_isLookingRight;
        Vector3 escala = transform.localScale;
        escala.x *= -1; //multiplica escala.x por menos uno
        transform.localScale = escala;
    }

    private void Update()
    {
        _HorizontalMove = moveAction.ReadValue<float>() * _movementVelocity;

        if (_sePuedeMover) 
        { 
            _animator.SetFloat("Horizontal", Mathf.Abs(_HorizontalMove));
            _animator.SetFloat("VelocityY", _rigidbody.velocity.y);
        }

        Debug.Log("fase de pray " + prayAction.phase);
    }

    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapBox(_groundController.position, _boxDimensions, Quaternion.identity, _whatIsGround);
        _inGround = colliders.Length > 0;
        _animator.SetBool("InGround", _inGround);
        OnMove(_HorizontalMove * Time.deltaTime);

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

