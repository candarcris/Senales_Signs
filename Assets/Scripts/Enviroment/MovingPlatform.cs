using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 2f; // Velocidad de movimiento horizontal
    public float distance = 8f; // Distancia máxima de movimiento

    private float initialPositionX; // Posición inicial del objeto

    private void Start()
    {
        initialPositionX = transform.position.x; // Guardar la posición inicial
    }

    private void Update()
    {
        // Calcular el desplazamiento horizontal
        float displacement = Mathf.Sin(Time.time * speed) * distance;

        // Calcular la nueva posición del objeto
        Vector3 newPosition = new Vector3(initialPositionX + displacement, transform.position.y, transform.position.z);

        // Mover el objeto a la nueva posición
        transform.position = newPosition;
    }
}
