using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleLadder : AparicibleObject
{
    private BoxCollider _boxCollider;
    [Header("Movement Settings")]
    [SerializeField] private Vector3 _targetPosition; // Posici�n cuando Ehyal est� cerca
    [SerializeField] private float _moveSpeed = 2f; // Velocidad de movimiento

    private Vector3 _initialPosition; // Posici�n inicial
    private bool _isMovingToTarget = false;

    [Header("Player Floating Path")]
    [SerializeField] private Transform[] _waypoints; // Puntos del camino verde
    [SerializeField] private float _floatingSpeed = 2f; // Velocidad de flotación
    [SerializeField] private float _floatingHeight = 1f; // Altura de flotación
    public PlayerController _playerController;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    protected override void Start()
    {
        base.Start();
        // Guardar la posici�n inicial
        _initialPosition = transform.position;
    }

    public override void UpdateOpacityBasedOnDistance(Transform transform, Vector3 offset, float multiplyValue)
    {
        base.UpdateOpacityBasedOnDistance(_ehyalTransform, Vector3.up, 15);

        if (_evaluationDistance <= _closeDistance || _distance <= _closeDistance)
        {
            _targetOpacity = 1f;
            MoveToTargetPosition();
        }
        else if (_evaluationDistance >= _farDistance)
        {
            _targetOpacity = 0f;
            MoveToInitialPosition();
        }
        else
        {
            // Interpolaci�n suave entre las distancias
            float normalizedDistance = (_evaluationDistance - _closeDistance) / (_farDistance - _closeDistance);
            _targetOpacity = 1f - normalizedDistance;

            // Movimiento proporcional
            MoveToIntermediatePosition(normalizedDistance);
        }
    }

    private void MoveToTargetPosition()
    {
        if (!_isMovingToTarget)
        {
            _isMovingToTarget = true;
        }
        _boxCollider.enabled = true;

        // Mover hacia la posici�n objetivo
        Vector3 targetPos = _initialPosition + _targetPosition;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * _moveSpeed);

        // Si est� muy cerca del objetivo, establecer la posici�n exacta
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            transform.position = targetPos;
        }
    }

    private void MoveToInitialPosition()
    {
        if (_isMovingToTarget)
        {
            _isMovingToTarget = false;
        }

        // Regresar a la posici�n inicial
        transform.position = Vector3.Lerp(transform.position, _initialPosition, Time.deltaTime * _moveSpeed);

        // Si est� muy cerca de la posici�n inicial, establecer la posici�n exacta
        if (Vector3.Distance(transform.position, _initialPosition) < 0.01f)
        {
            transform.position = _initialPosition;
            _boxCollider.enabled = false;
        }
    }

    private void MoveToIntermediatePosition(float normalizedDistance)
    {
        // Calcular posici�n intermedia basada en la distancia
        Vector3 targetPos = _initialPosition + _targetPosition;
        Vector3 intermediatePos = Vector3.Lerp(_initialPosition, targetPos, 1f - normalizedDistance);

        // Mover hacia la posici�n intermedia
        transform.position = Vector3.Lerp(transform.position, intermediatePos, Time.deltaTime * _moveSpeed);
        _boxCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // Disparar el evento con los waypoints para que PlayerController los use
            _playerController.TriggerFloatingAcrossPath(_waypoints, _floatingSpeed, _floatingHeight);
        }
    }

    private void Update()
    {
        if (_ehyalTransform != null)
        {
            UpdateOpacityBasedOnDistance(_ehyalTransform, Vector3.up, 15);
            UpdateOpacity();
        }
    }
}
