using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Signs
{
    public class UIWorldSpaceDialogue : MonoBehaviour
    {
        [SerializeField] private Transform targetAnchor; // A quién debe perseguir
        [SerializeField] private Transform target3DObjectAnchor; // A quién debe perseguir

        public TMP_Text dialogoTxt;
        // Opcional: Si quieres mostrar una imagen encima del NPC (ej. un signo de exclamación)
        public Image dialogoImg;
        public Image backGroundImg;
        float activeTime;
        Color fontColor;
        TMP_FontAsset fuente;
        GameObject objeto3D;

        [Tooltip("Segundos que se queda cada frase en pantalla antes de pasar a la siguiente")]
        // El Manager llamará a este método justo después de instanciar el prefab
        public void Initialize(WorldDialogueSequence sequence, Transform anchor, Transform anchor3DObj)
        {
            targetAnchor = anchor;
            target3DObjectAnchor = anchor3DObj;
            // Iniciamos la rutina que leerá el diálogo sola
            StartCoroutine(ReproducirDialogoAutomatico(sequence));
        }

        private IEnumerator ReproducirDialogoAutomatico(WorldDialogueSequence sequence)
        {
            // TODO: Haz un foreach para recorrer cada 'line' dentro de 'sequence.lines'
            foreach(var line in sequence.lines)
            {
                // TODO: Cambia el texto (y la imagen si existe) por los de la línea.
                // (Puedes usar tu efecto de máquina de escribir aquí si quieres, o solo poner el texto de golpe).
                dialogoImg.enabled = line.contextualImg != null ? true : false;
                dialogoImg.sprite = dialogoImg.enabled ? line.contextualImg : null;
                //if (line.contextualImg != null) 
                //{ 
                //    dialogoImg.sprite = line.contextualImg;
                //}
                //else
                //{
                //    dialogoImg.enabled = false;
                //}
                backGroundImg.enabled = line.showBg;
                activeTime = line.activeTime > 0 ? line.activeTime : 0;
                fuente = line.fuente;
                fontColor = line.fontColor;
                dialogoTxt.color = fontColor;
                dialogoTxt.font = fuente;
                dialogoTxt.text = line.text;

                if (line.extra3DModel != null) 
                {
                    objeto3D = Instantiate(line.extra3DModel, target3DObjectAnchor.position, target3DObjectAnchor.rotation);
                }

                // TODO: Usa 'yield return new WaitForSeconds(tiempoPorFrase);' para esperar antes de pasar a la siguiente frase.
                yield return new WaitForSeconds(line.activeTime);
            }
            // TODO: Cuando el foreach termine (se acaben las frases), destruye este objeto para que no quede basura en la escena.
            if (objeto3D != null) { Destroy(objeto3D); }
            Destroy(gameObject);
        }

        private void Update()
        {
            if (targetAnchor != null)
            {
                transform.position = targetAnchor.position;
            }
            else
            {
                if (objeto3D != null) Destroy(objeto3D);
                Destroy(gameObject);
            }
        }
    }
}
