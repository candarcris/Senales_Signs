using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AparicibleObject : MonoBehaviour, IAparicibleByEhyal
{
    [Header("Ehyal Reference")]
    public Transform _ehyalTransform;

    [Header("Distance Settings")]
    [SerializeField] protected float _closeDistance = 2f;
    [SerializeField] protected float _farDistance = 6f;

    [Header("Opacity Settings")]
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected float _lerpSpeed;

    protected float _targetOpacity = 0f;
    protected float _currentOpacity = 0f;
    protected float _distance;
    protected float _evaluationDistance; // Nueva variable para la distancia de evaluación

    protected virtual void Start()
    {
        InitializeRenderer();
        InitializeOpacity();
    }

    protected virtual void InitializeRenderer()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    protected virtual void InitializeOpacity()
    {
        if (_spriteRenderer != null)
        {
            Color color = _spriteRenderer.color;
            color.a = 0f;
            _spriteRenderer.color = color;
            _currentOpacity = 0f;
        }
    }

    protected virtual void ApplyOpacity()
    {
        Color color = _spriteRenderer.color;
        color.a = _currentOpacity;
        _spriteRenderer.color = color;
    }

    protected virtual void UpdateOpacity()
    {
        if (_spriteRenderer == null) return;

        if (Mathf.Abs(_currentOpacity - _targetOpacity) > 0.01f)
        {
            _currentOpacity = Mathf.Lerp(_currentOpacity, _targetOpacity, Time.deltaTime * _lerpSpeed);
            _currentOpacity = Mathf.Clamp01(_currentOpacity);

            if (Mathf.Abs(_currentOpacity - _targetOpacity) < 0.01f)
            {
                _currentOpacity = _targetOpacity;
            }

            ApplyOpacity();
        }
    }

    public virtual float GetTargetOpacity() => _targetOpacity;
    public virtual float GetCurrentOpacity() => _currentOpacity;

    public virtual void SetTargetOpacity(float opacity)
    {
        _targetOpacity = Mathf.Clamp01(opacity);
    }

    public virtual void SetOpacity(float opacity)
    {
        _targetOpacity = Mathf.Clamp01(opacity);
        _currentOpacity = opacity;

        if (_spriteRenderer != null)
        {
            Color color = _spriteRenderer.color;
            color.a = opacity;
            _spriteRenderer.color = color;
        }
    }

    public virtual void UpdateOpacityBasedOnDistance(Transform ehyalTransform, Vector3 offset, float multiplicatorValue)
    {
        if (ehyalTransform == null) return;

        _distance = Vector3.Distance(transform.position, ehyalTransform.position);
        _evaluationDistance = Vector3.Distance(transform.position, ehyalTransform.position + offset * multiplicatorValue);

        // Determinar la opacidad objetivo basada en la distancia
        // Considerar cerca si está cerca del punto de evaluación O si está muy cerca del objeto
        bool isClose = _evaluationDistance <= _closeDistance || _distance <= _closeDistance * 2;
        bool isFar = _evaluationDistance >= _farDistance;

        if (isClose)
        {
            _targetOpacity = 1f;
        }
        else if (isFar)
        {
            _targetOpacity = 0f;
        }
        else
        {
            // Interpolación suave entre las distancias
            float normalizedDistance = (_evaluationDistance - _closeDistance) / (_farDistance - _closeDistance);
            _targetOpacity = 1f - normalizedDistance;
        }

        _targetOpacity = Mathf.Clamp01(_targetOpacity);

        //if (ehyalTransform == null) return;
        //
        //_distance = Vector3.Distance(transform.position, ehyalTransform.position);
        //_evaluationDistance = Vector3.Distance(transform.position, ehyalTransform.position + offset * multiplicatorValue);
        //
        //if (_evaluationDistance <= _closeDistance || _distance <= _closeDistance)
        //{
        //    _targetOpacity = 1f;
        //}
        //else if (_evaluationDistance >= _farDistance && _distance >= _closeDistance)
        //{
        //    _targetOpacity = 0f;
        //}
        //else
        //{
        //    float distanceForInterpolation = Mathf.Min(_evaluationDistance, _distance);
        //    if (distanceForInterpolation <= _closeDistance)
        //    {
        //        _targetOpacity = 1f;
        //    }
        //    else if (distanceForInterpolation >= _farDistance)
        //    {
        //        _targetOpacity = 0f;
        //    }
        //    else
        //    {
        //        float normalizedDistance = (distanceForInterpolation - _closeDistance) / (_farDistance - _closeDistance);
        //        _targetOpacity = 1f - normalizedDistance;
        //    }
        //}
        //
        //_targetOpacity = Mathf.Clamp01(_targetOpacity);
    }

    protected virtual void Update()
    {
        if (_ehyalTransform != null)
        {
            UpdateOpacity();
        }
    }
}
