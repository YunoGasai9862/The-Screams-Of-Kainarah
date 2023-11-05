using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightFlickering))]
public class LightFlickerCustomUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); //calls the base

        LightFlickering lightFlicker = (LightFlickering)target;

        SerializedProperty subjectExist = serializedObject.FindProperty("anySubjectThatIsNotifyingTheLight");

        SerializedProperty subject = serializedObject.FindProperty("_subject");

        EditorGUI.BeginChangeCheck();

        if(subjectExist.boolValue)
        {
            EditorGUILayout.PropertyField(subject);
        }

        EditorGUI.EndChangeCheck();

        serializedObject.ApplyModifiedProperties();

    }

}
