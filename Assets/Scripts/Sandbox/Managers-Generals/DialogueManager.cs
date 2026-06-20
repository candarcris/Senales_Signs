using Signs;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Signs
{
    public class DialogueManager : MonoBehaviour
    {
        public UIScreenSpaceDialogue screenSpaceUI;
        [SerializeField] GameObject worldSpacePrefab;

        private Queue<DialogueLine> currentLines = new Queue<DialogueLine>();
        private bool isDialogueActive = false;

        private DialogueTrigger currentTrigger; // Guardamos quién nos llamó
        private int currentLineIndex = 0; // Llevamos la cuenta de qué línea vamos

        public void StartScreenSpaceDialogue(DialogueSequence sequence, DialogueTrigger trigger)
        {
            currentTrigger = trigger;
            currentLineIndex = 0;
            // 1. Limpiamos la cola por si había algo antes
            currentLines.Clear();

            screenSpaceUI.gameObject.SetActive(true);

            // 2. TODO: Llena la cola (currentLines) con las líneas (lines) que vienen dentro del 'sequence'
            // Pista: puedes usar un foreach.
            foreach (DialogueLine line in sequence.lines)
            { 
                currentLines.Enqueue(line);
            }
            // 3. TODO: Congela al jugador. (Pista: Busca tu PlayerControllerSigns y pon puedeMoverse en false)

            // 4. TODO: Llama a un método (ej. DisplayNextLine()) para mostrar la primera frase
            isDialogueActive = true;
            DisplayNextLine();
        }

        public GameObject StartWorldSpaceDialogue(WorldDialogueSequence worldSequence, Transform anchorPoint, Transform anchor3Dobj)
        {
            // A diferencia del Screen Space, aquí no congelamos al jugador ni usamos la Cola central.
            // TODO: Instancia el prefab de World Space en la posición del anchorPoint.
            GameObject nuevoWorldUI = Instantiate(worldSpacePrefab, anchorPoint.position, Quaternion.identity, null);
            // TODO: Pásale el 'sequence' a ese objeto recién creado para que él mismo se gestione de forma independiente.
            nuevoWorldUI.TryGetComponent(out UIWorldSpaceDialogue worldSpaceUI);

            if(worldSpaceUI != null)
            {
                worldSpaceUI.Initialize(worldSequence, anchorPoint, anchor3Dobj);
            }

            return nuevoWorldUI;
        }

        public void DisplayNextLine()
        {
            if (currentLines.Count == 0 && screenSpaceUI.canNextLine)
            { 
                EndDialogue();
                return;
            }
            DialogueLine dequeLine = currentLines.Dequeue();

            // ¡Magia aquí! Le decimos al Trigger que ejecute la acción si es que existe
            if (currentTrigger != null && !string.IsNullOrEmpty(dequeLine.actionName))
            {
                currentTrigger.ExecuteAction(dequeLine.actionName);
                
            }
            screenSpaceUI.ShowLine(dequeLine);
        }
        private void EndDialogue()
        {
            isDialogueActive = false;

            // TODO: Descongela al jugador, oculta el Canvas de pantalla.
            screenSpaceUI.gameObject.SetActive(false);

            // Si tenemos un trigger guardado, le decimos que dispare su evento final
            if (currentTrigger != null)
            {
                currentTrigger.onDialogueFinish?.Invoke();

                // Lo limpiamos por seguridad, ya que el diálogo ya terminó
                currentTrigger = null;
            }
        }
        void Update()
        {
            if (!isDialogueActive) return; // Si no hay diálogo, no hacemos nada
            // Si presionamos Enter o la barra espaciadora
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // ¿La UI sigue escribiendo?
                if (screenSpaceUI.isTyping)
                {
                    // Forzamos a que termine de escribir de golpe
                    screenSpaceUI.CompletarLineaDeGolpe();
                }
                else
                {
                    // Si ya terminó, mostramos la siguiente línea
                    DisplayNextLine();
                }
            }
        }
    }
}
