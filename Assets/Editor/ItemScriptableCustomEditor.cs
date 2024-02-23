using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemScriptableObject))]
public class ItemScriptableCustomEditor : Editor
{
    SerializedProperty _item, _name, _type, _icon, _restoreAm, _damageAm, _price, _desc;

    private void OnEnable()
    {
        _item = serializedObject.FindProperty("_item");
        _name = _item.FindPropertyRelative("_itemDisplayName");
        _type = _item.FindPropertyRelative("_itemType");
        _icon = _item.FindPropertyRelative("_itemIcon");
        _damageAm = _item.FindPropertyRelative("_itemDamage");
        _restoreAm = _item.FindPropertyRelative("_itemRestoreAmount");
        _price = _item.FindPropertyRelative("_itemPrice");
        _desc = _item.FindPropertyRelative("_itemDescription");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUILayout.BeginHorizontal();
        EditorGUILayout.ObjectField(_icon, GUIContent.none);
        var iconPreview = AssetPreview.GetAssetPreview(_icon.objectReferenceValue);
        EditorGUI.DrawTextureTransparent(GUILayoutUtility.GetRect(0, 100), iconPreview, ScaleMode.ScaleToFit);
        GUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(_name);
        EditorGUILayout.PropertyField(_type, GUIContent.none);

        var type = (ItemType)_type.enumValueIndex;
        switch (type)
        {
            case ItemType.Weapon:
                EditorGUILayout.PropertyField(_damageAm);
                break;
            case ItemType.potion:
                EditorGUILayout.PropertyField(_restoreAm);
                break;
        }

        EditorGUILayout.PropertyField(_price);
        EditorGUILayout.PropertyField(_desc);
        serializedObject.ApplyModifiedProperties();
    }
}
