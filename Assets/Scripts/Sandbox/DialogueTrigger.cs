using UnityEngine;
using UnityEngine.Events;

namespace Signs
{
    // 1. Creamos esta pequeña estructura para emparejar la Palabra Clave con el Evento
    [System.Serializable]
    public struct DialogueAction
    {
        public string actionName; // Aquí escribirás "DarLlave"
        public UnityEvent actionEvent; // Aquí arrastras la puerta para abrirla
    }

    public class DialogueTrigger : MonoBehaviour
    {
        public DialogueSequence dialogueSequence;
        
        public DialogueManager dialogueManager;

        public UnityEvent onDialogueStart; // 2. Declaramos el evento para verlo en el Inspector
        public UnityEvent onDialogueFinish; 

        [Header("Acciones del Diálogo")]
        public DialogueAction[] lineActions; // <-- Esto se verá HERMOSO y claro en el Inspector

        public void TriggerDialogue()
        {
            onDialogueStart?.Invoke(); // 3. Disparamos el evento (que congelará al jugador)
            dialogueManager.StartScreenSpaceDialogue(dialogueSequence, this);
        }

        // 2. Método que el Manager llamará cuando lea una palabra clave
        public void ExecuteAction(string actionName)
        {
            if (string.IsNullOrEmpty(actionName)) return;
            // Buscamos si tenemos esa palabra clave en nuestra lista
            foreach (DialogueAction action in lineActions)
            {
                if (action.actionName == actionName)
                {
                    action.actionEvent?.Invoke(); // Ejecutamos la puerta!
                }
            }
        }

        public void SwapDialogue(DialogueSequence newSequence)
        {
            dialogueSequence = newSequence;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                TriggerDialogue();
            }
        }

        void Update()
        {
        
        }
    }
}
