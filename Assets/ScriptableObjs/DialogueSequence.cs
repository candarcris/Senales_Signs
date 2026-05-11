using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Signs
{
    // 1. Creamos una estructura para representar UNA sola "diapositiva" o línea del diálogo
    [System.Serializable] // Súper importante para que Unity lo muestre en el Inspector
    public struct DialogueLine
    {
        public string nombrePj;
        [TextArea(3, 5)] // Hace que la caja de texto en el inspector sea más grande
        public string text;

        public Sprite portraitImage; // Opcional: Imagen de quien habla o icono a mostrar

        [Tooltip("Escribe una palabra clave si esta línea activa algo (ej: 'AbrirPuerta')")]
        public string actionName; // <-- ¡Esto es lo nuevo!
    }
    // 2. El ScriptableObject simplemente guarda una lista de esas líneas
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Scriptable Objects/Dialogue Sequence")]
    public class DialogueSequence : ScriptableObject
    {
        [Header("Contenido del Diálogo")]
        public DialogueLine[] lines; // Usamos un arreglo (o List<DialogueLine>) para la secuencia
    }
}
