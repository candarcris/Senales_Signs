using UnityEngine;
using UnityEngine.Events;

namespace Signs
{
    public class WorldDialogueTrigger : MonoBehaviour
    {
        [Tooltip("De dónde saldrá el globo de texto (ej. un objeto vacío sobre la cabeza del NPC)")]
        public Transform worldSpaceAnchor;
        public WorldDialogueSequence worldialogueSequence;
        public DialogueManager dialogueManager;

        private GameObject globoActual; // Guarda la referencia al globo vivo

        public void TriggerDialogue()
        {
            if(globoActual != null)
            {
                return;
            }

            dialogueManager.StartWorldSpaceDialogue(worldialogueSequence, worldSpaceAnchor);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                TriggerDialogue();
            }
        }
    }
}
