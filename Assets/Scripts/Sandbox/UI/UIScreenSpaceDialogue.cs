using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Signs
{
    public class UIScreenSpaceDialogue : MonoBehaviour
    {
        public TMP_Text nombreTxt;
        public TMP_Text dialogoTxt;
        public Image buttonImg;
        public Image illustrationImg;
        float velocidadEscritura = 0.05f;
        private Coroutine corrutinaEscritura;
        public TMP_FontAsset fuente;
        public Color fontColor;

        public bool isTyping = false;
        private string lineaActualCompleta = "";
        public bool canNextLine;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        public void SetActiveNextLineButton(bool state)
        {
            canNextLine = state;
        }

        public void ShowLine(DialogueLine line)
        {
            nombreTxt.text = line.nombrePj;
            buttonImg.sprite = line.buttonImage;
            illustrationImg.sprite = line.portraitImage;

            if (corrutinaEscritura != null)
            {
                StopCoroutine(corrutinaEscritura);
            }
            corrutinaEscritura = StartCoroutine(EscribirDialogo(line));
        }

        public IEnumerator EscribirDialogo(DialogueLine line)
        {
            isTyping = true; // Empieza a escribir
            lineaActualCompleta = line.text; // Guardamos el texto completo por si el jugador lo corta
            dialogoTxt.text = "";
            fuente = line.fuente;
            fontColor = line.fontColor;
            nombreTxt.color = fontColor;
            nombreTxt.font = fuente;
            dialogoTxt.color = fontColor;
            dialogoTxt.font = fuente;

            foreach (var letra in line.text)
            {
                dialogoTxt.text += letra;
                yield return new WaitForSeconds(velocidadEscritura);
            }
            isTyping = false; // Terminó de escribir naturalmente
        }

        // Nuevo método: Se llama si el jugador pulsa Enter ansiosamente
        public void CompletarLineaDeGolpe()
        {
            if (corrutinaEscritura != null) StopCoroutine(corrutinaEscritura);
            dialogoTxt.text = lineaActualCompleta; // Ponemos todo el texto
            isTyping = false;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
