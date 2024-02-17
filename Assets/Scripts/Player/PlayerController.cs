using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;


public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]

    private float _HorizontalMove = 0f;
    [SerializeField] private float _movementVelocity;
    [SerializeField] private float _moveSoftener;
    private Vector3 _velocity = Vector3.zero;
    private bool _isLookingRight = true;
    public Transform _initParent;
    public bool _firstFall = false;

    [Header("Salto")]

    [SerializeField] private float _jumpForce; // fuerza que se le anade al rigidbody para el salto
    [SerializeField] private float _fallMultiplier = 2.5f; // Ajusta este valor según sea necesario
    [SerializeField] private LayerMask _whatIsGround; // capa para el suelo
    [SerializeField] private Transform _groundController; // objeto en los pies del personaje que detecta el suelo
    [SerializeField] private Vector3 _boxDimensions; // dimensiones de la caja de los pies del personaje
    [SerializeField] private bool _inGround; // tocando el piso?
    [SerializeField] private float _extraGravityMultiplier; // fuerza extra de gravedad al caer
    [SerializeField] private float _gravityMultiplier = 2.5f; // Ajusta este valor según sea necesario
    private bool _jump = false;

    [Header("Animacion")]

    private Animator _anim;
    private Rigidbody _rb2D;

    [Header("Habilidades")]

    public bool _isRizpaForm = false;
    public List<GameObject> _eventsTrigerList = new();

    [Header("Damage and Die")]
    public bool _inDamage;
    public bool _sePuedeMover;
    [SerializeField] private Vector2 _velocidadRebote;

    private void Start()
    {
        _sePuedeMover = false;
        _anim = GetComponent<Animator>();
        _rb2D = GetComponent<Rigidbody>();
        _anim.SetLayerWeight(1, 1f);
        _rb2D.drag = 50;
        _rb2D.drag = 49;

        DialogsManager.OnFinishDialog += SetPlayerValues;
    }

    private void OnDestroy()
    {
        DialogsManager.OnFinishDialog -= SetPlayerValues;
    }


    public void SetPlayerValues()
    {
        _rb2D.drag = 0;
    }

    public IEnumerator ReductFallingVelocity()
    {
        float totalTime = 1f; 
        int totalIterations = 30;
        float waitTime = totalTime / totalIterations; 

        for (int i = 0; i < totalIterations; i++)
        {
            _rb2D.drag = i;
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.tag == "Sign")
        //{
        //    _isRizpaForm = true;
        //}
        //if(collision.tag == "MovingPlatform")
        //{
        //    this.transform.parent = collision.transform.parent;
        //}
        //if (collision.tag == "Dialogo1")
        //{
        //    StartCoroutine(FirstDialogueAppear());
        //    OnStartDialogue();
        //}
        //if (collision.tag == "Dialogo2")
        //{
        //    OnSecondDialog();
        //}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.tag == "MovingPlatform")
        //{
        //    this.transform.parent = _initParent;
        //}
        //if (collision.tag == "Dialogo1")
        //{
        //    _bocadillo.SetActive(false);
        //    _eventsTrigerList[0].SetActive(false);
        //}
        //if (collision.tag == "Dialogo2")
        //{
        //    _eventsTrigerList[1].SetActive(false);
        //}
    }

    private void OnGUI()
    {
        // Define el tamaño del área central
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

        // Añade espacios flexibles para centrar verticalmente
        GUILayout.FlexibleSpace();

        // Comienza un área para el botón
        GUILayout.BeginHorizontal();
        // Añade espacios flexibles para centrar horizontalmente
        GUILayout.FlexibleSpace();

        // Coloca el botón en el centro
        if (GUILayout.Button("Mi Botón", GUILayout.Width(200), GUILayout.Height(50)))
        {
            // Acción cuando se hace clic en el botón
            Debug.Log("¡Has hecho clic en el botón!");
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Añade espacios flexibles para centrar verticalmente
        GUILayout.FlexibleSpace();
        GUILayout.EndArea();
    }

    private IEnumerator fieldOfViewAnim(float duration)
    {
        float elapsedTime = 0f;
        float startFOV = ManagerLocator.GetCamerasManager().GetFieldOfView();
        float targetFOV = Mathf.Lerp(40, 60, duration);

        while (elapsedTime < duration)
        {
            float newFOV = Mathf.Lerp(startFOV, targetFOV, elapsedTime / duration);
            ManagerLocator.GetCamerasManager().SetFieldOfView(newFOV);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegurémonos de que el FOV sea exactamente el valor final
        ManagerLocator.GetCamerasManager().SetFieldOfView(targetFOV);
    }

    public void SetGameValues()
    {
        StartCoroutine(fieldOfViewAnim(2));
        StartCoroutine(ReductFallingVelocity());
    }

    public void PlayerValuesDialogMeanTime()
    {
        _anim.SetFloat("Horizontal", 0);
    }

    public void FallingIntro()
    {
        _firstFall = true;
        _anim.SetBool("InGround", _inGround);
        _anim.SetBool("FirstFall", false);
        _rb2D.drag = 0;
    }

    private void Move(float mover, bool saltar)
    {
        Vector3 targetVelocity = new Vector3(mover, _rb2D.velocity.y);
        _rb2D.velocity = Vector3.SmoothDamp(_rb2D.velocity, targetVelocity, ref _velocity, _moveSoftener);

        if (mover > 0 && !_isLookingRight)
        {
            Rotate();
        }
        else if(mover < 0 && _isLookingRight)
        {
            Rotate();
        }
        if (_inGround && _jump)
        {
            Jump();
        }

        if (_rb2D.velocity.y < 0)
        {
            _rb2D.AddForce(Vector3.down * _extraGravityMultiplier, ForceMode.Acceleration);
        }
    }

    private void Jump()
    {
        _inGround = false;
        _rb2D.AddForce(new Vector3(0f, _jumpForce));
    }

    private void Rotate()
    {
        _isLookingRight = !_isLookingRight;
        Vector3 escala = transform.localScale;
        escala.x *= -1; //multiplica escala.x por menos uno
        transform.localScale = escala;
    }

    public void Rebote(Vector2 hitPoint)
    {
        _rb2D.velocity = new Vector2(-_velocidadRebote.x * hitPoint.x, _velocidadRebote.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_groundController.position, _boxDimensions);
    }

    private void Update()
    {
        if(_firstFall)
        {
            _HorizontalMove = Input.GetAxisRaw("Horizontal") * _movementVelocity;
        }

        if (_sePuedeMover) 
        { 
            _anim.SetFloat("Horizontal", Mathf.Abs(_HorizontalMove));
            _anim.SetFloat("VelocityY", _rb2D.velocity.y);
            if (Input.GetButtonDown("Jump") && _inGround)
            {
                _jump = true;
                _rb2D.velocity = new Vector2(_rb2D.velocity.x, 0); // Resetea la velocidad vertical antes de saltar
            }
        }
        //EnterRizpaForm();
    }

    /// <summary>
    /// para cambios en las fisicas
    /// </summary>
    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapBox(_groundController.position, _boxDimensions, Quaternion.identity, _whatIsGround);
        _inGround = colliders.Length > 0;
        if(_firstFall)
        {
            _anim.SetBool("InGround", _inGround);
            if (_sePuedeMover)
            {
                Move(_HorizontalMove * Time.deltaTime, _jump);
            }
        }
        else
        {
            _anim.SetBool("InGround", true);
            _anim.SetBool("FirstFall", true);
        }

        _jump = false;

        _rb2D.AddForce(Vector3.down * Physics.gravity.magnitude * _gravityMultiplier);
        //_rb2D.AddForce(Vector2.down * Physics2D.gravity.y);

        if (_rb2D.velocity.y < 0)
        {
            Vector2 gravityModifier = Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1) * Time.fixedDeltaTime * 2f;
            _rb2D.velocity = new Vector2(_rb2D.velocity.x, _rb2D.velocity.y + gravityModifier.y);
        }
    }
}
