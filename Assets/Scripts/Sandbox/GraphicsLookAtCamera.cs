using UnityEngine;

[ExecuteInEditMode] // Para verlo en la escena sin dar Play
public class GraphicsLookAtCamera : MonoBehaviour
{
    [Tooltip("Si es verdadero, el objeto solo rotará en el eje Y (útil para exploración 3D).")]
    public bool lockYAxis = true;

    void LateUpdate()
    {
        if (Camera.main != null)
        {
            if (lockYAxis)
            {
                // Billboard Cilíndrico (Solo Eje Y)
                Vector3 cameraPosition = Camera.main.transform.position;
                cameraPosition.y = transform.position.y; // Ignorar la altura de la cámara
                
                // Hacemos que mire hacia atrás (modo sprite) para invertir la rotación
                transform.LookAt(2 * transform.position - cameraPosition);
            }
            else
            {
                // Billboard Esférico (Manejado por Cinemachine, mira en todos los ejes)
                Vector3 targetRotation = Camera.main.transform.rotation * Vector3.forward;
                Vector3 targetUp = Camera.main.transform.rotation * Vector3.up;

                transform.LookAt(transform.position + targetRotation, targetUp);
            }
        }
    }
}
