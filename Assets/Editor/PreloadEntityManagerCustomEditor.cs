
using Cinemachine.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

[CustomEditor(typeof(PreloaderManager))]
public class PreloadEntityManagerCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        PreloaderManager preloadManager = (PreloaderManager)target;

        GUILayout.Label("Reference Preloader For Preloading:");
        SerializedProperty preloader = serializedObject.FindProperty("preloader");
        EditorGUILayout.PropertyField(preloader);

        GUILayout.Label("Reference Gameload For Instantiation:");
        SerializedProperty gameload = serializedObject.FindProperty("gameLoad");
        EditorGUILayout.PropertyField(gameload);

        SerializedProperty arraySize = serializedObject.FindProperty("preloadEntities");

        GUILayout.Label("Array For Preloading Entities:");

        if (GUILayout.Button("Add Preload Entity"))
        {
            arraySize.arraySize++;
        }

        for(int i=0; i< arraySize.arraySize; i++)
        {
            SerializedProperty element = serializedObject.FindProperty("preloadEntities").GetArrayElementAtIndex(i);

            SerializedProperty assetReference = element.FindPropertyRelative("m_assetReference");
            SerializedProperty entityType = element.FindPropertyRelative("m_entityType");
            SerializedProperty entityMB = element.FindPropertyRelative("m_entityMB");
            SerializedProperty entitySO = element.FindPropertyRelative("m_entitySO");

            EditorGUILayout.PropertyField(assetReference);
            EditorGUILayout.PropertyField(entityType);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(GetEntityPreloadingType(entityType, entityMB, entitySO));
            EditorGUI.indentLevel--;

            GUILayout.Space(10);


        }

        if (GUILayout.Button("Delete Preload Entity"))
        {
            arraySize.arraySize--;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private SerializedProperty GetEntityPreloadingType(SerializedProperty entityType, SerializedProperty entityMB, SerializedProperty entitySO)
    {
        EntityType preloadEntityType = (EntityType)entityType.enumValueIndex;

        return preloadEntityType.Equals(EntityType.MonoBehavior) ? entityMB : entitySO;
    }
}
