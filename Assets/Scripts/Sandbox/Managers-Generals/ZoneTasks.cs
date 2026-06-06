using UnityEngine;
using UnityEngine.Events;

namespace Signs
{
    public class ZoneTasks : MonoBehaviour
    {
        [Header("Configuración de la Tarea")]
        [Tooltip("Descripción de lo que hay que hacer (ej: Matar enemigos, Activar 2 pilares)")]
        public string taskName;

        [Tooltip("¿Cuántas acciones se requieren para completar esta zona/tarea?")]
        public int requiredProgress = 1;
        private int currentProgress = 0;
        [Header("Eventos de Consecuencia")]
        [Tooltip("Lo que ocurre cuando se completa la tarea (abrir puerta, activar diálogo, etc)")]
        public UnityEvent OnTaskCompleted;
        // Cualquier objeto (enemigo, palanca) puede llamar a esta función cuando hace su parte
        public void AddProgress()
        {
            if (currentProgress >= requiredProgress) return; // Ya se completó
            currentProgress++;
            if (currentProgress >= requiredProgress)
            {
                CompleteTask();
            }
        }
        private void CompleteTask()
        {
            Debug.Log($"Tarea de Zona completada: {taskName}");
            OnTaskCompleted?.Invoke();
        }
    }
}
