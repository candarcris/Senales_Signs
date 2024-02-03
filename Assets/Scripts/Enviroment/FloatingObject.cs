using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    public float floatHeight = 0.5f; // Altura de la flotación
    public float floatSpeed = 1f; // Velocidad de la flotación

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void FixedUpdate()
    {
        // Calcula el desplazamiento vertical usando una función sinusoidal
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        // Aplica el desplazamiento a la posición del objeto
        transform.position = startPos + Vector3.up * yOffset;
    }
}
