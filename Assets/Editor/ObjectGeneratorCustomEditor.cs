using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(ObjectGenerator))]
public class ObjectGeneratorCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ObjectGenerator objectGenerator = (ObjectGenerator)target;

        SerializedProperty shouldUseTiles = serializedObject.FindProperty("shouldUseTiles");
        SerializedProperty tiles = serializedObject.FindProperty("tiles");
        SerializedProperty itemToInstantiate = serializedObject.FindProperty("itemToInstantiate");
        SerializedProperty offsetForInstantiation = serializedObject.FindProperty("offsetForInstantiation");
        SerializedProperty prefabWithLocations = serializedObject.FindProperty("prefabWithLocations");

        EditorGUILayout.PropertyField(itemToInstantiate);
        EditorGUILayout.PropertyField(offsetForInstantiation);
        EditorGUILayout.PropertyField(shouldUseTiles);

        EditorGUI.BeginChangeCheck();
        if(shouldUseTiles.boolValue)
        {
            EditorGUILayout.PropertyField(tiles);

        }else
        {
            EditorGUILayout.PropertyField(prefabWithLocations);
        }
        EditorGUI.EndChangeCheck();

        serializedObject.ApplyModifiedProperties();
    }

}
