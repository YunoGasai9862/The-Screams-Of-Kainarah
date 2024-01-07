
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PickableItems))]
public class ScriptablePickableItemsCustomGUI : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update(); //updates

        PickableItems items = (PickableItems)target; //targets

        SerializedProperty arraySize = serializedObject.FindProperty("pickableEntities"); //for array size

        if (GUILayout.Button("Add Element"))
        { 
            arraySize.arraySize++;
        }

        for (int i = 0; i < items.pickableEntities.Length; i++)
        {
            SerializedProperty eachElement = serializedObject.FindProperty("pickableEntities").GetArrayElementAtIndex(i); //pick one

            SerializedProperty objectName = eachElement.FindPropertyRelative("objectName");

            SerializedProperty prefabToInstantiate = eachElement.FindPropertyRelative("prefabToInstantiate");

            SerializedProperty shouldBeDisabledAfterSomeTime = eachElement.FindPropertyRelative("shouldBeDisabledAfterSomeTime");

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(objectName);

            EditorGUILayout.PropertyField(prefabToInstantiate);

            EditorGUILayout.PropertyField(shouldBeDisabledAfterSomeTime);

            EditorGUI.EndChangeCheck();

            EditorGUILayout.Space();

        }

        if (GUILayout.Button("Delete Element"))
        {
            arraySize.arraySize--;

        }

        serializedObject.ApplyModifiedProperties();


    }
}
