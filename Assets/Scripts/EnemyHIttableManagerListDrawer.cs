
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnemyHittableObjects))]
public class EnemyHIttableManagerListDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var isInstantiableRect = new Rect(position.x, position.y, 50, position.height);

        var instantiableObjectRect = new Rect(position.x + 60, position.y, 200, position.height);

        var objectTagRect = new Rect(position.x + 200, position.y, 50, position.height);




    }
}
