using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EyalController : MonoBehaviour, IHangable
{
    private LevelManager levelManager;
    private GameManager gameManager;
    public Transform[] _rutePoints;
    private bool _isMoving = false; // Nueva variable para controlar si ya est� movi�ndose
    private float velocidad = 5f;
    [SerializeField] private int indicePuntoRutaActual = 0;
    [SerializeField] private bool _puedeMoverse = true;
    private SpriteRenderer _spriteRenderer;
    public SphereCollider _ehyalCollider;
    [SerializeField] private Transform _sagarTransform;
    [SerializeField] private Transform hangPoint;
    public Transform HangPoint => hangPoint;

    private InputActions _inputActions;
    private InputAction _eyalAction;

    private bool _sagarPraying = false;

    [Header("Skills")]
    public GameObject _beamPrefab;
    [SerializeField] private BeamPool beamPool;
    [SerializeField] private Transform enemyTarget; // Nueva variable para el objetivo


    private void Awake()
    {
        _inputActions = ManagerLocator.GetInputActions();
        levelManager = ManagerLocator.GetLevelManager();

        // Si GameManager a�n no est� inicializado, esperar
        if (_inputActions == null)
        {
            StartCoroutine(WaitForGameManager());
        }

        PlayerController.OnHold += HoldSagarAir;
        PlayerController.OnDrop += DropSagar;
        PlayerController.OnPray += FollowSagar;
        PlayerController.OnPrayEnd += StopFollowingSagar;
        PlayerController.OnCombat += ShootAttack;

        _sagarTransform = FindObjectOfType<PlayerController>().transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private IEnumerator WaitForGameManager()
    {
        while (GameManager._sharedInstance == null)
        {
            yield return null;
        }
        _inputActions = ManagerLocator.GetInputActions();
    }

    private void OnEnable()
    {
        if (_inputActions != null)
        {
            SetupInputActions();
        }
    }

    private void SetupInputActions()
    {
        _eyalAction = _inputActions.EhyalControl.EhyalAction;
        _eyalAction.Enable();
    }

    private void OnDisable()
    {
        PlayerController.OnHold -= HoldSagarAir;
        PlayerController.OnDrop -= DropSagar;
        PlayerController.OnPray -= FollowSagar;
        PlayerController.OnPrayEnd -= StopFollowingSagar;
        PlayerController.OnCombat -= ShootAttack;
    }

    private void Start()
    {
        gameManager = ManagerLocator.GetGameManager();
        Movement(false);
    }

    public void Movement(bool activeCollider)
    {
        if (_isMoving) return; // Si ya est� movi�ndose, no hacer nada
        StartCoroutine(MoveToNextPositionCoroutine(activeCollider));
    }

    public void LookAtSagar()
    {
        Vector2 distance = transform.position - _sagarTransform.position;

        if(distance.x > 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if(distance.x < 0)
        {
            _spriteRenderer.flipX = false;
        }
    }

    public void FollowSagar()
    {
        _sagarPraying = true;
    }

    public void StopFollowingSagar()
    {
        _sagarPraying = false;
    }

    public void HoldSagarAir()
    {
        gameManager.SetContext(Context.Ehyal);
        Movement(false);
    }

    public void DropSagar()
    {
        _ehyalCollider.enabled = false;
    }

    private IEnumerator MoveToNextPositionCoroutine(bool activeCollider)
    {
        if (_isMoving) yield break; // Doble verificaci�n
        _isMoving = true; // Marcar que est� movi�ndose
        ResumeMovement();
        while (_puedeMoverse)
        {
            //_spriteRenderer.flipX = false;
            if (_rutePoints.Length == 0)
            {
                _isMoving = false;
                yield break;
            }

            // Obtiene los puntos de ruta actual y siguiente
            Vector3 startPoint = transform.position;
            Vector3 endPoint = _rutePoints[indicePuntoRutaActual].position;
            Vector3 lastPos = startPoint;

            // Calcula el punto intermedio (para crear el efecto de arco)
            Vector3 middlePoint = (startPoint + endPoint) / 2f;
            middlePoint += Vector3.up * Mathf.Abs(endPoint.y - startPoint.y) * 0.5f;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * (velocidad / Vector3.Distance(startPoint, endPoint));

                // Interpola entre los puntos de inicio, medio y final para crear el arco
                Vector3 newPos = Vector3.Lerp(Vector3.Lerp(startPoint, middlePoint, t), Vector3.Lerp(middlePoint, endPoint, t), t);

                // Comparar desplazamiento en X
                if (newPos.x > lastPos.x)
                {
                    _spriteRenderer.flipX = false;
                }
                else if (newPos.x < lastPos.x)
                {
                    _spriteRenderer.flipX = true;
                }
                // Mueve el objeto hacia la nueva posici�n
                transform.position = newPos;

                yield return null;
            }
            transform.position = endPoint;
            _ehyalCollider.enabled = activeCollider;

            // Si el objeto llega al punto de ruta actual, pasa al siguiente
            if (Vector3.Distance(transform.position, endPoint) < 0.1f)
            {
                if(!ManagerLocator.GetDialogsManager()._dialogPanel.gameObject.activeInHierarchy)
                {
                    gameManager.SetContext(Context.Player);
                }
                // Detiene el movimiento
                _puedeMoverse = false;
                //_spriteRenderer.flipX = true;
                // Establece el �ndice al siguiente punto de ruta
                indicePuntoRutaActual = (indicePuntoRutaActual + 1) % _rutePoints.Length;
            }
        }
        _isMoving = false; // Marcar que termin� de moverse
    }

    // Funci�n para reanudar el movimiento
    private void ResumeMovement()
    {
        _puedeMoverse = true;
    }

    public void ShootAttack()
    {
        // Verificar que el beamPool esté asignado
        if (beamPool == null)
        {
            Debug.LogWarning("BeamPool no está asignado en EyalController");
            return;
        }

        FindClosestEnemy();

        GameObject beamObj;
        
        if (enemyTarget != null)
        {
            // Disparar hacia enemigo
            beamObj = beamPool.GetBeam(enemyTarget, transform);
        }
        else
        {
            // Disparar en dirección de Ehyal cuando no hay enemigos
            beamObj = beamPool.GetBeam(null, transform);
        }
        
        if (beamObj != null)
        {
            // Posicionar la bala en la posición de Ehyal
            beamObj.transform.position = transform.position;
        }
        else
        {
            Debug.LogError("No se pudo obtener un beam del pool");
        }
    }

    private void FindClosestEnemy()
    {
        if (levelManager != null && levelManager._enemyList.Count > 0)
        {
            Transform closestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (Enemy enemy in levelManager._enemyList)
            {
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                {
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemy.transform;
                    }
                }
            }

            enemyTarget = closestEnemy;
        }
        else
        {
            enemyTarget = null;
        }
    }

    private void Update()
    {
        LookAtSagar();
        if (_sagarPraying)
        {
            // Solo seguir si est� rezando, no cuando se mueve
            Vector3 targetPosition = _sagarTransform.position + Vector3.up * 4f;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 3f);
        }
    }
}
