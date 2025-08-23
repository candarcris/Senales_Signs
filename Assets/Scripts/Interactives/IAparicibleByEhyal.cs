using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAparicibleByEhyal
{
    /// <summary>
    /// Actualiza la opacidad basada en la distancia del objetivo
    /// </summary>
    void UpdateOpacityBasedOnDistance(Transform transform, Vector3 offset, float multiplyValue);

    /// <summary>
    /// Obtiene la opacidad objetivo actual
    /// </summary>
    float GetTargetOpacity();

    /// <summary>
    /// Obtiene la opacidad actual
    /// </summary>
    float GetCurrentOpacity();

    /// <summary>
    /// Establece la opacidad objetivo
    /// </summary>
    void SetTargetOpacity(float opacity);

    /// <summary>
    /// Fuerza una opacidad específica
    /// </summary>
    void SetOpacity(float opacity);
}
