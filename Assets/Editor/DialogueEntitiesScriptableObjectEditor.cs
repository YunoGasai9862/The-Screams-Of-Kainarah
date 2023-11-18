using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueEntityScriptableObject))]
public class DialogueEntitiesScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update(); //updates

        DialogueEntityScriptableObject dialogueEntityScriptableObject = (DialogueEntityScriptableObject)target;

        SerializedProperty arraySize = serializedObject.FindProperty("entities");

        if(GUILayout.Button("Add Entity"))
        {
            arraySize.arraySize++;
        }

        for (int i = 0; i < arraySize.arraySize; i++)
        {

            SerializedProperty eachEntity = serializedObject.FindProperty("entities").GetArrayElementAtIndex(i); //accessing from here

            SerializedProperty entityGameObject = eachEntity.FindPropertyRelative("entity");

            SerializedProperty entityShouldDialogueTrigger = eachEntity.FindPropertyRelative("shouldDialogueTrigger");

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(entityGameObject);

            EditorGUILayout.PropertyField(entityShouldDialogueTrigger);

            EditorGUI.EndChangeCheck();
        }

        if (GUILayout.Button("Delete Entity"))
        {
            arraySize.arraySize--;
        }

        serializedObject.ApplyModifiedProperties();

    }
}
