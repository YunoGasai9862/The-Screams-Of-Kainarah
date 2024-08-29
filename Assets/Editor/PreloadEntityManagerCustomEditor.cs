
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

        SerializedProperty arraySize = serializedObject.FindProperty("preloadEntities");

        for(int i=0; i< arraySize.arraySize; i++)
        {
            SerializedProperty elements = serializedObject.FindProperty("preloadEntities").GetArrayElementAtIndex(i);

            SerializedProperty assetReference = elements.FindPropertyRelative("m_assetReference");
            SerializedProperty entityMB = elements.FindPropertyRelative("m_entityMB");
            SerializedProperty entitySO = elements.FindPropertyRelative("m_entitySO");
            SerializedProperty isMonoBehaviour = elements.FindPropertyRelative("m_isMonoBehavior");

            EditorGUILayout.PropertyField(assetReference);
            EditorGUILayout.PropertyField(entityMB);
            EditorGUILayout.PropertyField(entitySO);
            EditorGUILayout.PropertyField(isMonoBehaviour);

        }


        serializedObject.ApplyModifiedProperties();
    }
}
