using System.Collections.Generic;
using UnityEngine;

namespace Signs
{
    [CreateAssetMenu(fileName = "PrimigenialLanguage", menuName = "Scriptable Objects/PrimigenialLanguage")]
    public class PrimigenialLanguage : ScriptableObject
    {
        [Tooltip("Lista de letras que Sagar ya ha descubierto")]
        public List<char> letrasDescubiertas = new List<char>();
        // Método para agregar una letra nueva cuando completas un mensaje
        public void DescubrirLetra(char nuevaLetra)
        {
            char letraMayuscula = char.ToUpper(nuevaLetra);
            if (!letrasDescubiertas.Contains(letraMayuscula))
            {
                letrasDescubiertas.Add(letraMayuscula);
            }
        }
        // Método para que el DialogueManager pregunte si entendemos esta letra
        public bool ConoceLetra(char letra)
        {
            return letrasDescubiertas.Contains(char.ToUpper(letra));
        }
        // Opcional: Un botón para borrar el progreso al probar el juego
        public void ResetearIdioma()
        {
            letrasDescubiertas.Clear();
        }
    }
}
