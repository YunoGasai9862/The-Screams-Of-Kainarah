using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyAI))]
public class AIScriptEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EnemyAI enemyWalkBetweenPaths = (EnemyAI)target;

        SerializedProperty wayPoints = serializedObject.FindProperty("WayPoints");
        SerializedProperty singleTarget = serializedObject.FindProperty("target");

        if (enemyWalkBetweenPaths.multipleTargets)
            EditorGUILayout.PropertyField(wayPoints);
        else
            EditorGUILayout.PropertyField(singleTarget);

        serializedObject.ApplyModifiedProperties();

    }
}
