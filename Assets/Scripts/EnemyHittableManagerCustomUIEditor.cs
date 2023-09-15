
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyHittableObjects))]
public class EnemyHittableManagerCustomUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EnemyHittableObjects enemyHittableObject = (EnemyHittableObjects)target;

        EditorGUILayout.LabelField("Custom Editor For Enemy Hittable Ojbects");

        enemyHittableObject.IsinstantiableObject = EditorGUILayout.Toggle("Instantiate?", enemyHittableObject.IsinstantiableObject);
    }

}
