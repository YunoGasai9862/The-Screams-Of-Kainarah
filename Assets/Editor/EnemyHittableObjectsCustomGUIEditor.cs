
using UnityEditor;
using UnityEngine;
using EnemyHitable;

[CustomEditor(typeof(EnemyHittableObjects))]
public class EnemyHittableManagerCustomUIEditor : Editor
{

    public override void OnInspectorGUI()
    {

        serializedObject.Update();

        EnemyHittableObjects enemyHittableObject = (EnemyHittableObjects)target;

        SerializedProperty arraySize = serializedObject.FindProperty("elements");

        if (GUILayout.Button("Add Element")) //adds a button
        {
            arraySize.arraySize++; //increases the array size by 1
        }

        for (int i = 0; i < enemyHittableObject.elements.Length; i++)
        {

            SerializedProperty eachElement = serializedObject.FindProperty("elements").GetArrayElementAtIndex(i); //at each index

            SerializedProperty isInstantiable = eachElement.FindPropertyRelative("IsinstantiableObject");

            SerializedProperty instantiateAfterAttack = eachElement.FindPropertyRelative("instantiateAfterAttack");

            SerializedProperty objectTag = eachElement.FindPropertyRelative("ObjectTag");


            EditorGUI.BeginChangeCheck(); //keeps track of changes

            EditorGUILayout.PropertyField(isInstantiable);

            if (isInstantiable.boolValue) //if True/exists
            {
                EditorGUILayout.PropertyField(instantiateAfterAttack); //YAYA works~!!! Will add more tomorrow!
            }

            EditorGUILayout.PropertyField(objectTag);

            EditorGUI.EndChangeCheck();

            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Delete Element")) //removes the Element
        {
            arraySize.arraySize--;
        }

        serializedObject.ApplyModifiedProperties();


    }
}
