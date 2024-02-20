using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RandomizerAttribute))]
public class RandomizerPropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 40;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(property.propertyType == SerializedPropertyType.Float)
        {
            var rectButton = position;
            rectButton.position += Vector2.up * 20;
            rectButton.size = new Vector2(rectButton.size.x, 20);
            position.size = new Vector2(rectButton.size.x, 20);
            var randomizer = attribute as RandomizerAttribute;
            if (GUI.Button(rectButton, "Randomizer"))
            {
                property.floatValue = UnityEngine.Random.Range(randomizer.min, randomizer.max);
            }
            EditorGUI.PropertyField(position, property, label);
        }
        else
        {
            Debug.LogError("Solo funciona para valores float");
        }
    }
}
