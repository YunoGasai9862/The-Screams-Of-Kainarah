
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyHittableObjects))]
public class EnemyHittableManagerCustomUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EnemyHittableObjects enemyHittableObject = (EnemyHittableObjects)target;

        for (int i = 0; i < enemyHittableObject.elements.Length; i++)
        {

            SerializedProperty eachElement = serializedObject.FindProperty("elements").GetArrayElementAtIndex(i); //at each index

            SerializedProperty isInstantiable = eachElement.FindPropertyRelative("IsinstantiableObject");

            SerializedProperty instantiateAfterAttack = eachElement.FindPropertyRelative("instantiateAfterAttack");

            EditorGUI.BeginChangeCheck(); //keeps track of changes

            EditorGUILayout.PropertyField(isInstantiable);

            if (isInstantiable.boolValue) //if True
            {
                EditorGUILayout.PropertyField(instantiateAfterAttack); //YAYA works~!!! Will add more tomorrow!
            }

            EditorGUI.EndChangeCheck();

            EditorGUILayout.Space();
        }

        serializedObject.ApplyModifiedProperties();


    }
}
