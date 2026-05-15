
using Assets.Scripts.Polling.Configuration;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PollOrchestratorConfiguration))]
public class PollOrchestratorCustomEditor : Editor
{

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        PollOrchestratorConfiguration pollOrchestratorConfiguration = (PollOrchestratorConfiguration)target; //creating target

        SerializedProperty arraySize = serializedObject.FindProperty("orchestrators"); //gets the array

        if(GUILayout.Button("Add Orchestrator"))
        {
            arraySize.arraySize++;
        }

        for (int i = 0; i < arraySize.arraySize; i++)
        {
            SerializedProperty orchestratorElement = serializedObject.FindProperty("orchestrators").GetArrayElementAtIndex(i);

            SerializedProperty registryKey = orchestratorElement.FindPropertyRelative("registryKey");
            SerializedProperty pollingIntervalInSecondds = orchestratorElement.FindPropertyRelative("pollingIntervalInSecondds");

            EditorGUILayout.PropertyField(registryKey);
            EditorGUILayout.PropertyField(pollingIntervalInSecondds);

            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Remove Orchestrator"))
        {
            arraySize.arraySize = arraySize.arraySize > 0 ? arraySize.arraySize - 1 : 0;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
