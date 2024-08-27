using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CustomPropertyDrawer(typeof(PreloadEntity))]
public class PreloadEntityCustomEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty assetReferenceProperty = property.FindPropertyRelative("m_assetReference");

        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), assetReferenceProperty);


    }
}
