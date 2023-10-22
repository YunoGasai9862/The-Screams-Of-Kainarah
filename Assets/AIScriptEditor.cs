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
        SerializedProperty multipleObjects = serializedObject.FindProperty("Multiple Targets");

    }
}
