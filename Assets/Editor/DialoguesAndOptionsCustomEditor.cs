using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialoguesAndOptions))]
public class DialoguesAndOptionsCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DialoguesAndOptions dialoguesAndOptions = (DialoguesAndOptions)target;
    }
}
