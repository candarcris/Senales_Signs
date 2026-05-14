using TMPro;
using UnityEngine;

namespace Signs
{
    [System.Serializable]
    public struct WorldDialogueLine
    {
        [TextArea(2, 4)]
        public string text;
        public Sprite contextualImg;
        public GameObject extra3DModel; // Opcional 3D
        public bool showBg;
        public float activeTime;
        public TMP_FontAsset fuente;
        public Color fontColor;
    }

    [CreateAssetMenu(fileName = "WorldDialogueSequence", menuName = "Scriptable Objects/WorldDialogueSequence")]
    public class WorldDialogueSequence : ScriptableObject
    {
        public WorldDialogueLine[] lines;
    }
}
