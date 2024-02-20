using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Tools/ScriptableItems")]
public class ItemScriptableObject : ScriptableObject
{
    [SerializeField] private Item _item;
}

[System.Serializable]
public struct Item
{
    public string _displayName;
    public ItemType _itemType;
    public Sprite _itemIcon;
    public float _itemDamage;
    [Min(0)] public int _itemRestoreAmount; //Min(0) para que el restore no pueda ser negativo
}

public enum ItemType
{
    Weapon,
    potion
}
