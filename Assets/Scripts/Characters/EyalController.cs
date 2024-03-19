using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyalController : MonoBehaviour
{
    public Transform[] _rutePoints;
    private float velocidad = 10f;
    private int indiceActual = 0;
    private bool _puedeMoverse = true;

    public void StartMovement()
    {
        StartCoroutine(MoveToNextPositionCoroutine());
    }

    private IEnumerator MoveToNextPositionCoroutine()
    {
        ResumeMovement();
        while (_puedeMoverse)
        {
            //_spriteRenderer.flipX = false;
            if (_rutePoints.Length == 0)
                yield break;

            // Obtiene los puntos de ruta actual y siguiente
            Vector3 startPoint = transform.position;
            Vector3 endPoint = _rutePoints[indiceActual].position;

            // Calcula el punto intermedio (para crear el efecto de arco)
            Vector3 middlePoint = (startPoint + endPoint) / 2f;
            middlePoint += Vector3.up * Mathf.Abs(endPoint.y - startPoint.y) * 0.5f;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * (velocidad / Vector3.Distance(startPoint, endPoint));

                // Interpola entre los puntos de inicio, medio y final para crear el arco
                Vector3 newPos = Vector3.Lerp(Vector3.Lerp(startPoint, middlePoint, t), Vector3.Lerp(middlePoint, endPoint, t), t);

                // Mueve el objeto hacia la nueva posición
                transform.position = newPos;

                yield return null;
            }

            // Si el objeto llega al punto de ruta actual, pasa al siguiente
            if (Vector3.Distance(transform.position, endPoint) < 0.1f)
            {
                // Detiene el movimiento
                _puedeMoverse = false;
                //_spriteRenderer.flipX = true;
                // Establece el índice al siguiente punto de ruta
                indiceActual = (indiceActual + 1) % _rutePoints.Length;
            }
        }
    }

    // Función para reanudar el movimiento
    private void ResumeMovement()
    {
        _puedeMoverse = true;
    }
}
