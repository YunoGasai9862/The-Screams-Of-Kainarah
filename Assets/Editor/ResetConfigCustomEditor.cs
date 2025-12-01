using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResetConfig))]
public class ResetSystemCustomEditor: Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ResetConfig reset = (ResetConfig)target;

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

            GUILayout.Label("Main Key/Field Name");

            EditorGUILayout.PropertyField(key);

            GUILayout.Label("Value (Type, Old Value, New Value)");

            EditorGUILayout.PropertyField(value);
        }

        if (GUILayout.Button("Remove Reset Parameter"))
        {
            array.arraySize--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}