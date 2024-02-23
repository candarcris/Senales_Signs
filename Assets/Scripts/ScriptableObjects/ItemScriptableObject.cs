using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.IO;
using UnityEditor; //Libreria para manipular archivos

[CreateAssetMenu(fileName = "Item", menuName = "Tools/ScriptableItems")]
public class ItemScriptableObject : ScriptableObject
{
    [SerializeField] public Item _item;
    private string path => Application.dataPath + "/itemPreset.json"; //El valor de path sera la ruta mas la carpeta indicada
    [ContextMenu("ToJson")]
    private void SerializeOnJson()
    {
        var json = JsonUtility.ToJson(this, true);
        File.WriteAllText(path, json); //Esta función crea el json indicado en el path nindicado
        AssetDatabase.Refresh(); //Para que aparezca el json al momento de crearlo
    }
    [ContextMenu("LoadJson")]
    private void LoadJson()
    {
        if (File.Exists(path))
        { 
            var json = File.ReadAllText(path);

            /// Va a pasar la información directamente al objeto sin crear una 
            /// instancia nueva, esto se hace porque el scriptable object no es un objeto del juego
            JsonUtility.FromJsonOverwrite(json, this); 
        }
    }
}

[System.Serializable]
public struct Item
{
    public string _itemDisplayName;
    public ItemType _itemType;
    public Sprite _itemIcon;
    public float _itemDamage;
    [Min(0)] public int _itemRestoreAmount; //Min(0) para que el restore no pueda ser negativo
    public int _itemPrice;
    public string _itemDescription;
}

public enum ItemType
{
    Weapon,
    potion
}
