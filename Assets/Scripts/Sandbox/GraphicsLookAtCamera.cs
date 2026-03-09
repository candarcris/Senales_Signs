using UnityEngine;

[ExecuteInEditMode] // Para verlo en la escena sin dar Play
public class GraphicsLookAtCamera : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            // Tomamos la rotación de la cámara principal (manejada por Cinemachine)
            Vector3 targetRotation = Camera.main.transform.rotation * Vector3.forward;
            Vector3 targetUp = Camera.main.transform.rotation * Vector3.up;

            // Forzamos al objeto a mirar en esa dirección
            transform.LookAt(transform.position + targetRotation, targetUp);
        }
    }
}
