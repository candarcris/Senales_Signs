using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EyalController))]
public class VisualToolsEditor : Editor
{
    private void OnSceneGUI()
    {
        EyalController eyalController = (EyalController)target;

        for(int i = 0; i < eyalController._rutePoints.Length; i++)
        {
            var rutePoint = eyalController._rutePoints[i];
            var nextRutePoint = eyalController._rutePoints[(i + 1) % eyalController._rutePoints.Length];

            if (rutePoint ==  null) { continue; }
            if (nextRutePoint ==  null) { continue; }

            Handles.color = Color.red;
            Handles.DrawDottedLine(rutePoint.position, nextRutePoint.position, 5f);

            EditorGUI.BeginChangeCheck();
            var newPos = Handles.PositionHandle(rutePoint.position, rutePoint.rotation);
            if(EditorGUI.EndChangeCheck() )
            {
                Undo.RecordObject(rutePoint, "Move rutePoint");
                rutePoint.position = newPos;
            }
        }
    }
}
