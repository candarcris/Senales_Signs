using Signs;
using System.Collections.Generic;
using UnityEngine;

namespace Signs
{
    public class DialogueManager : MonoBehaviour
    {
        public UIScreenSpaceDialogue screenSpaceUI;
        GameObject worldSpacePrefab;

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
            DisplayNextLine();
            isDialogueActive = true;
        }

        public void StartWorldSpaceDialogue(DialogueSequence sequence, Transform anchorPoint)
        {
            // A diferencia del Screen Space, aquí no congelamos al jugador ni usamos la Cola central.
            // TODO: Instancia el prefab de World Space en la posición del anchorPoint.
            Instantiate(worldSpacePrefab, anchorPoint);
            // TODO: Pásale el 'sequence' a ese objeto recién creado para que él mismo se gestione de forma independiente.
        }

        public void DisplayNextLine()
        {
            if (currentLines.Count == 0) { EndDialogue(); return; }
            DialogueLine dequeLine = currentLines.Dequeue();

            // ¡Magia aquí! Le decimos al Trigger que ejecute la acción si es que existe
            if (currentTrigger != null)
            {
                currentTrigger.ExecuteAction(dequeLine.actionName);
            }
            screenSpaceUI.ShowLine(dequeLine);
        }
        private void EndDialogue()
        {
            // TODO: Descongela al jugador, oculta el Canvas de pantalla.
            screenSpaceUI.gameObject.SetActive(false);
            isDialogueActive = false;
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
