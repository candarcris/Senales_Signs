using UnityEngine;
using System.Collections;

public class ElementoOculto : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadAparicion = 2f;
    public bool seVuelveOcultar = true; // Si es false, se queda visible para siempre al descubrirlo

    public Collider miCollider;
    public Renderer miRenderer;

    [Header("Efecto Pop-up")]
    [SerializeField] private Color colorVisible;
    [SerializeField] private Color colorDestello = Color.white; // Color del flash inicial
    [SerializeField] private Vector3 escalaVisible;
    private Coroutine rutinaFading;

    void Start()
    {
        // Guardamos las propiedades originales
        colorVisible = miRenderer.material.color;
        escalaVisible = miRenderer.transform.localScale;

        // Estado inicial: Oculto e Intangible
        OcultarInstantaneo();
    }

    void OcultarInstantaneo()
    {
        miRenderer.enabled = false; // Lo ocultamos apagando el renderer
        miRenderer.transform.localScale = Vector3.zero; // Y lo encogemos
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

    // --- EFECTO POP-UP MÁGICO ---
    private IEnumerator TransicionVisibilidad(bool aparecer)
    {
        if (!aparecer)
        {
            yield return new WaitForSeconds(1f);
        }

        // Si aparece, encendemos renderer y compañía
        if (aparecer)
        {
            if (miCollider != null) miCollider.enabled = true;
            miRenderer.enabled = true;
        }

        Vector3 escalaInicial = miRenderer.transform.localScale;
        Vector3 escalaObjetivo = aparecer ? escalaVisible : Vector3.zero;
        
        // Al aparecer, empujamos el color hacia el destello para que se note instantáneo
        Color colorInicial = aparecer ? colorDestello : miRenderer.material.color;
        Color colorObjetivo = aparecer ? colorVisible : colorVisible; // Al desaparecer no importa el color final, solo la escala

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * velocidadAparicion;
            
            // Función curva simple (Ease-Out) para que la aparición se sienta más pulida
            float tSuavizado = 1f - Mathf.Pow(1f - t, 3f);

            miRenderer.transform.localScale = Vector3.Lerp(escalaInicial, escalaObjetivo, tSuavizado);
            miRenderer.material.color = Color.Lerp(colorInicial, colorObjetivo, t);
            
            yield return null;
        }

        // Aseguramos valores exactos al terminar
        miRenderer.transform.localScale = escalaObjetivo;
        miRenderer.material.color = colorObjetivo;

        // Si desaparece, lo ocultamos por completo
        if (!aparecer)
        {
            miRenderer.enabled = false;
            if (miCollider != null) miCollider.enabled = false;
        }
    }
}
