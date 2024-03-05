using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerController : MonoBehaviour
{
    private InputActions inputActions;
    private InputAction jumpAction;
    private InputAction moveAction;

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


    private void Awake()
    {
        _sePuedeMover = true;
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        inputActions = new InputActions();
    }
    private void OnEnable()
    {
        jumpAction = inputActions.PlayerControl.Jump;
        jumpAction.Enable();
        jumpAction.performed += OnJump;

        moveAction = inputActions.PlayerControl.Move;
        moveAction.Enable();
    }

    private void OnDisable()
    {
        jumpAction.Disable();
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
            _rigidbody.AddForce(new Vector3(0f, _jumpForce));
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
        //_HorizontalMove = Input.GetAxisRaw("Horizontal") * _movementVelocity;
        _HorizontalMove = moveAction.ReadValue<float>() * _movementVelocity;

        if (_sePuedeMover) 
        { 
            _animator.SetFloat("Horizontal", Mathf.Abs(_HorizontalMove));
            _animator.SetFloat("VelocityY", _rigidbody.velocity.y);
        }
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

