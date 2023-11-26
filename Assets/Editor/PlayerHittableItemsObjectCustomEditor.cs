
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

[CustomEditor(typeof(PlayerHittableItemsScriptableObject))]
public class PlayerHittableItemsObjectCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update(); 

        PlayerHittableItemsScriptableObject playerhittableItems = (PlayerHittableItemsScriptableObject)target;

        SerializedProperty arraySize = serializedObject.FindProperty("colliderItems");

        if(GUILayout.Button("Add Item"))
        {
            arraySize.arraySize++;
        }

        for (int i = 0; i < arraySize.arraySize; i++)
        {
            SerializedProperty elements = serializedObject.FindProperty("colliderItems").GetArrayElementAtIndex(i);

            SerializedProperty canHitPlayer = elements.FindPropertyRelative("canHitPlayer");
            SerializedProperty collider = elements.FindPropertyRelative("collider");
            SerializedProperty isItBasedOnAnimationName = elements.FindPropertyRelative("isItBasedOnAnimationName");
            SerializedProperty animationName = elements.FindPropertyRelative("animationName");

            EditorGUILayout.PropertyField(canHitPlayer);

            EditorGUI.BeginChangeCheck();

            if(canHitPlayer.boolValue)
            {
                EditorGUILayout.PropertyField(collider);
            }

            EditorGUILayout.PropertyField(isItBasedOnAnimationName);

            if (isItBasedOnAnimationName.boolValue)
            {
                EditorGUILayout.PropertyField(animationName);

            }

            EditorGUI.EndChangeCheck();

        }

        if (GUILayout.Button("Delete Item"))
        {
            if(arraySize.arraySize > 0)
            {
                arraySize.arraySize = arraySize.arraySize > 0 ? arraySize.arraySize - 1 : 0;
            }
        }


        serializedObject.ApplyModifiedProperties();
    }
}

