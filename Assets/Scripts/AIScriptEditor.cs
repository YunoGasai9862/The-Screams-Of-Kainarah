using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyWalkBetweenPathsAI))]
public class AIScriptEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EnemyWalkBetweenPathsAI enemyWalkBetweenPaths = (EnemyWalkBetweenPathsAI)target;

        SerializedProperty wayPoints = serializedObject.FindProperty("WayPoints");
        SerializedProperty singleTarget = serializedObject.FindProperty("target");

        if (enemyWalkBetweenPaths.multipleTargets)
            EditorGUILayout.PropertyField(wayPoints);
        else
            EditorGUILayout.PropertyField(singleTarget);

        serializedObject.ApplyModifiedProperties();

    }
}
