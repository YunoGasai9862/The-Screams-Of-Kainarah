
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyAnimationScriptableObject))]
public class EnemyAnimationScriptableObjectCustomEditor : Editor
{

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EnemyAnimationScriptableObject enemyAnimationScriptableObject = (EnemyAnimationScriptableObject)target; //creating target

        SerializedProperty arraySize = serializedObject.FindProperty("eachAnimation"); //gets the array

        if(GUILayout.Button("Add Element"))
        {
            arraySize.arraySize++;
        }

        for (int i = 0; i < arraySize.arraySize; i++)
        {
            SerializedProperty eachElement = serializedObject.FindProperty("eachAnimation").GetArrayElementAtIndex(i);

            SerializedProperty animationName = eachElement.FindPropertyRelative("animationName");

            SerializedProperty selectIntValue = eachElement.FindPropertyRelative("selectIntValue");
            SerializedProperty selectBoolValue = eachElement.FindPropertyRelative("selectBoolValue");
            SerializedProperty selectFloatValue = eachElement.FindPropertyRelative("selectFloatValue");
            SerializedProperty selectStringValue = eachElement.FindPropertyRelative("selectStringValue");

            SerializedProperty valueInt = eachElement.FindPropertyRelative("valueInt");
            SerializedProperty valueString = eachElement.FindPropertyRelative("valueString");
            SerializedProperty valueFloat = eachElement.FindPropertyRelative("valueFloat");
            SerializedProperty valueBool = eachElement.FindPropertyRelative("valueBool");

            EditorGUILayout.PropertyField(animationName);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(selectIntValue);

            if (selectIntValue.boolValue)
            {
                EditorGUILayout.PropertyField(valueInt);
                disableOtherBools(selectBoolValue, selectFloatValue, selectStringValue);

            }

            EditorGUILayout.PropertyField(selectBoolValue);

            if (selectBoolValue.boolValue)
            {
                EditorGUILayout.PropertyField(valueBool);
                disableOtherBools(selectIntValue, selectFloatValue, selectStringValue);

            }

            EditorGUILayout.PropertyField(selectFloatValue);

            if (selectFloatValue.boolValue)
            {
                EditorGUILayout.PropertyField(valueFloat);
                disableOtherBools(selectIntValue, selectBoolValue, selectStringValue);

            }

            EditorGUILayout.PropertyField(selectStringValue);

            if (selectStringValue.boolValue)
            {
                EditorGUILayout.PropertyField(valueString);
                disableOtherBools(selectIntValue, selectBoolValue, selectFloatValue);

            }

            EditorGUI.EndChangeCheck();

            EditorGUILayout.Space();


        }

        if (GUILayout.Button("Remove Element"))
        {
            arraySize.arraySize--;
        }

        serializedObject.ApplyModifiedProperties();

    }

    public void disableOtherBools(SerializedProperty first, SerializedProperty second, SerializedProperty third)
    {
        first.boolValue = false;
        second.boolValue = false;
        third.boolValue = false;
    }
}
