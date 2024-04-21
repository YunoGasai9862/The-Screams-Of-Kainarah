using UnityEditor;

[CustomEditor(typeof(CustomLightProcessing))]
public class CustomLightProcessingUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); //calls the base

        CustomLightProcessing customLightProcessing = (CustomLightProcessing)target;

        SerializedProperty subjectExist = serializedObject.FindProperty("anySubjectThatIsNotifyingTheLight");

        SerializedProperty subject = serializedObject.FindProperty("_subject");

        EditorGUI.BeginChangeCheck();

        if (subjectExist.boolValue)
        {
            EditorGUILayout.PropertyField(subject);
        }

        EditorGUI.EndChangeCheck();

        serializedObject.ApplyModifiedProperties();

    }

}
