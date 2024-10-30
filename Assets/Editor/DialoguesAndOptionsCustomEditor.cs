using Amazon.Polly;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialoguesAndOptions))]
public class DialoguesAndOptionsCustomEditor : Editor
{
    private const int SINGLE_DIALOGUE_MAX_ARRAY_LENGTH = 1;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DialoguesAndOptions dialoguesAndOptions = (DialoguesAndOptions)target;

        VoiceOptions();

        SerializedProperty notifyEntityListenerEvent = serializedObject.FindProperty("notifyEntityListenerEvent");

        EditorGUILayout.PropertyField(notifyEntityListenerEvent);

        SerializedProperty array = serializedObject.FindProperty("exchange");

        if (GUILayout.Button("Add Dialougue Exchange"))
        {
            array.arraySize++;
        }

        for(int i = 0; i< array.arraySize; i++)
        {
            GUILayout.Label($"Dialogue: {i + 1}");

            SerializedProperty element = serializedObject.FindProperty("exchange").GetArrayElementAtIndex(i);

            SerializedProperty dialogueTriggeringEntity = element.FindPropertyRelative("_dialogueTriggeringEntity");

            SerializedProperty dialogues = element.FindPropertyRelative("_dialogues");     

            SerializedProperty dialogueOptions = element.FindPropertyRelative("_dialogueOptions");

            SerializedProperty multiDialoguesBool = dialogueOptions.FindPropertyRelative("_multipleDialogues");


            if(dialogues.arraySize > SINGLE_DIALOGUE_MAX_ARRAY_LENGTH)
            {
                multiDialoguesBool.boolValue = true;
            }
            else
            {
                multiDialoguesBool.boolValue = false;
            }

            EditorGUILayout.PropertyField(dialogueTriggeringEntity);

            EditorGUILayout.PropertyField(dialogueOptions);

            EditorGUILayout.PropertyField(dialogues);

            GUILayout.Space(30);

        }

        if (GUILayout.Button("Delete Dialougue Exchange"))
        {
            array.arraySize--;
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void VoiceOptions()
    {
        GUILayout.Label("Voice Options Available:");
        GUILayout.Label(
            "Aditi, Adriano, Amy, Andres, Aria,\n" +
            "Arlet, Arthur, Astrid, Ayanda, Bianca,\n" +
            "Brian, Burcu, Camila, Carla, Carmen,\n" +
            "Celine, Chantal, Conchita, Cristiano, Daniel,\n" +
            "Danielle, Dora, Elin, Emma, Enrique,\n" +
            "Ewa, Filiz, Gabrielle, Geraint, Giorgio,\n" +
            "Gregory, Gwyneth, Hala, Hannah, Hans,\n" +
            "Hiujin, Ida, Ines, Isabelle, Ivy,\n" +
            "Jacek, Jan, Joanna, Joey, Justin,\n" +
            "Kajal, Karl, Kazuha, Kendra, Kevin,\n" +
            "Kimberly, Laura, Lea, Liam, Lisa,\n" +
            "Liv, Lotte, Lucia, Lupe, Mads,\n" +
            "Maja, Marlene, Mathieu, Matthew, Maxim,\n" +
            "Mia, Miguel, Mizuki, Naja, Niamh,\n" +
            "Nicole, Ola, Olivia, Pedro, Penelope,\n" +
            "Raveena, Remi, Ricardo, Ruben, Russell,\n" +
            "Ruth, Salli, Seoyeon, Sergio, Sofie,\n" +
            "Stephen, Suvi, Takumi, Tatyana, Thiago,\n" +
            "Tomoko, Vicki, Vitoria, Zayd, Zeina,\n" +
            "Zhiyu"
        );

        GUILayout.Space(10);
    }
}
