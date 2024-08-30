
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
            SerializedProperty isMonoBehaviour = element.FindPropertyRelative("m_isMonoBehavior");
            SerializedProperty entityMB = element.FindPropertyRelative("m_entityMB");
            SerializedProperty entitySO = element.FindPropertyRelative("m_entitySO");

            EditorGUILayout.PropertyField(assetReference);
            EditorGUILayout.PropertyField(isMonoBehaviour);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(GetEntityPreloadingType(isMonoBehaviour, entityMB, entitySO));
            EditorGUI.indentLevel--;

            GUILayout.Space(10);


        }

        if (GUILayout.Button("Delete Preload Entity"))
        {
            arraySize.arraySize--;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private SerializedProperty GetEntityPreloadingType(SerializedProperty isMonoBehavior, SerializedProperty entityMB, SerializedProperty entitySO)
    {
        return isMonoBehavior.boolValue ? entityMB : entitySO;
    }
}
