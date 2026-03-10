using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    public Transform floatingPos;
    private Vector3 startPos; // Mejor guardar la posición (Vector3) que el Transform
    public float duration = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Guardamos la posición inicial exacta en el mundo
        startPos = transform.position;
        StartCoroutine(FloatingLoop());
    }

    IEnumerator FloatingLoop()
    {
        while (true) // Bucle infinito para que no deje de flotar
        {
            // Ir hacia arriba
            yield return StartCoroutine(MoveObject(startPos, floatingPos.position));

            // Volver a la base
            yield return StartCoroutine(MoveObject(floatingPos.position, startPos));
        }
    }

    IEnumerator MoveObject(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Opcional: Suavizar el movimiento (SmoothStep)
            t = t * t * (3f - 2f * t);

            transform.position = Vector3.Lerp(from, to, t);
            yield return null; // Espera al siguiente frame
        }
    }
}
