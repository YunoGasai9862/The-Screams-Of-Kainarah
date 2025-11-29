using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResetSystem))]
public class ResetSystemCustomEditor: Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ResetSystem reset = (ResetSystem)target;

        SerializedProperty state = serializedObject.FindProperty("state");

        SerializedProperty array = serializedObject.FindProperty("resetParameters");

        if (GUILayout.Button("Add Reset Parameter"))
        {
            array.arraySize++;
        }

        for (int i=0; i < array.arraySize; i++)
        {
            SerializedProperty element = array.GetArrayElementAtIndex(i);

            SerializedProperty key = element.FindPropertyRelative("m_key");

            SerializedProperty value = element.FindPropertyRelative("m_val");

            EditorGUILayout.PropertyField(key);

            EditorGUILayout.PropertyField(value);
        }

        if (GUILayout.Button("Remove Reset Parameter"))
        {
            array.arraySize--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}