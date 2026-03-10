using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class ElementoOculto : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadAparicion = 2f;
    public bool seVuelveOcultar = true; // Si es false, se queda visible para siempre al descubrirlo

    [SerializeField] private Collider miCollider;
    [SerializeField] private Renderer miRenderer;
    [SerializeField] private Color colorTransparente;
    [SerializeField] private Color colorVisible;
    private Coroutine rutinaFading;

    void Start()
    {
        miRenderer = GetComponent<Renderer>();

        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            if (!col.isTrigger)
            {
                miCollider = col;
                break; // Lo encontramos y dejamos de buscar
            }
        }

        // Guardamos el color original del material
        colorVisible = miRenderer.material.color;
        // Creamos la versión invisible (Alpha = 0)
        colorTransparente = new Color(colorVisible.r, colorVisible.g, colorVisible.b, 0f);

        // Estado inicial: Oculto e Intangible
        OcultarInstantaneo();
    }

    void OcultarInstantaneo()
    {
        miRenderer.material.color = colorTransparente;
        if (miCollider != null) miCollider.enabled = false; // No se puede chocar/pisar
    }

    // --- DETECCIÓN DEL AURA DE EHYAL ---
    private void OnTriggerEnter(Collider other)
    {
        // Si el objeto que entró tiene el Layer "AuraDivina"
        if (other.gameObject.layer == LayerMask.NameToLayer("AuraDivina"))
        {
            if (rutinaFading != null) StopCoroutine(rutinaFading);
            rutinaFading = StartCoroutine(TransicionVisibilidad(true));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (seVuelveOcultar && other.gameObject.layer == LayerMask.NameToLayer("AuraDivina"))
        {
            if (rutinaFading != null) StopCoroutine(rutinaFading);
            rutinaFading = StartCoroutine(TransicionVisibilidad(false));
        }
    }

    // --- EFECTO VISUAL SUAVE ---
    private IEnumerator TransicionVisibilidad(bool aparecer)
    {
        // Si aparece, activamos la colisión inmediatamente para que el jugador no caiga
        if (aparecer) miCollider.enabled = true;

        Color colorObjetivo = aparecer ? colorVisible : colorTransparente;
        Color colorActual = miRenderer.material.color;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * velocidadAparicion;
            miRenderer.material.color = Color.Lerp(colorActual, colorObjetivo, t);
            yield return null;
        }

        // Si desaparece, quitamos la colisión al terminar de desvanecerse
        if (!aparecer && miCollider != null) miCollider.enabled = false;
    }
}
