using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingStone : MonoBehaviour
{
    public float forceMagnitude = 0.2f; // Magnitud de la fuerza
    public Vector3 forceDirection = Vector3.right; // Dirección de la fuerza (derecha por defecto)
    private Rigidbody rb;
    private Vector3 originalForce; // Almacenar la fuerza original


    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Obtén el componente Rigidbody del objeto
        originalForce = forceDirection * forceMagnitude; // Almacenar la fuerza original al inicio
    }

    private void OnDisable()
    {        
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(-rb.velocity, ForceMode.VelocityChange);
        }
    }

    private void Update()
    {
        // Calcula la fuerza en función de la dirección y la magnitud de la fuerza
        Vector3 force = forceDirection * forceMagnitude;

        // Aplica la fuerza al Rigidbody
        rb.AddForce(originalForce, ForceMode.Force);

        // Obtener la posición del objeto en la vista de la cámara
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        // Desactivar el objeto si está fuera de la vista de la cámara
        if (screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height)
        {
            gameObject.SetActive(false);
        }
    }
}
