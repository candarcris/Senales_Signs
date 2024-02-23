using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ItemEditorWindow : EditorWindow
{
    public List<ItemScriptableObject> _items;
    ItemScriptableObject _currentItem;
    Vector2 scrollPosition;

    [MenuItem("Tools/ItemEditorWindow")]
    static void OpenWindow()
    {
        var window = GetWindow<ItemEditorWindow>("Editor de items");
        window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 500, 500);
    }

    private void OnEnable()
    {
        var guids = AssetDatabase.FindAssets("t:ItemScriptableObject");
        _items = new List<ItemScriptableObject>();
        foreach (var gui in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(gui); //pasar gui a un path
            var asset = AssetDatabase.LoadAssetAtPath<ItemScriptableObject>(path); //pasar el objeto a la ruta
            _items.Add(asset);
        }
    }

    private void OnGUI()
    {
        var scrollRect = new Rect(0, 0, 220, Screen.height);
        GUILayout.BeginArea(scrollRect);
        GUILayout.BeginScrollView(scrollPosition, false, true);
        foreach (var item in _items)
        {
            Texture iconTexture = null;
            if(item._item._itemIcon != null)
            {
                iconTexture = item._item._itemIcon.texture;
            }
            if (GUILayout.Button(new GUIContent(item._item._itemDisplayName, iconTexture), GUILayout.Width(200), GUILayout.Height(50))) //siempre que sea if es un boton
            {
                _currentItem = item;
                GUI.FocusControl(_currentItem._item._itemDescription);
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        if (_currentItem == null) return;

        var itemEditRect = new Rect();
        itemEditRect.position = new Vector2(scrollRect.xMax, scrollRect.yMin);
        itemEditRect.size = new Vector2(Screen.width - scrollRect.width - Screen.width / 3f, Screen.height - 200);

        GUILayout.BeginArea(itemEditRect);
        var so = new SerializedObject(_currentItem);
        GUI.enabled = false;
        EditorGUILayout.PropertyField(so.FindProperty("_item"));
        GUI.enabled = true;

        EditorGUI.BeginChangeCheck();   
        var price = EditorGUILayout.IntSlider("Prize:", _currentItem._item._itemPrice, 100, 1000);
        GUILayout.Label("Description:");
        var desc = EditorGUILayout.TextArea(_currentItem._item._itemDescription, EditorStyles.textArea);
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_currentItem, "Propiedad cambió");
            _currentItem._item._itemPrice = price;
            _currentItem._item._itemDescription = desc;
            hasUnsavedChanges = true;

            EditorUtility.SetDirty(_currentItem); // esto es para que sirva el ctrl + Z
        }

        GUILayout.EndArea();
    }
}
