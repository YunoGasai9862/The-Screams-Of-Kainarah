using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialoguesAndOptions))]
public class DialoguesAndOptionsCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DialoguesAndOptions dialoguesAndOptions = (DialoguesAndOptions)target;

        SerializedProperty array = serializedObject.FindProperty("exchange");

        if(GUILayout.Button("Add Dialougue Exchange"))
        {
            array.arraySize++;
        }

        for(int i = 0; i< array.arraySize; i++)
        {
            SerializedProperty element = serializedObject.FindProperty("exchange").GetArrayElementAtIndex(i);

            SerializedProperty dialogues = element.FindPropertyRelative("dialogues");

            SerializedProperty dialogueOptions = element.FindPropertyRelative("dialogueOptions");

            EditorGUILayout.PropertyField(dialogues);

            EditorGUILayout.PropertyField(dialogueOptions);

            //continue with adding more labels/picking up and use it!!!

        }

        if (GUILayout.Button("Delete Dialougue Exchange"))
        {
            array.arraySize--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
