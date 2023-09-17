
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

        EditorGUI.BeginDisabledGroup(!enemyHittableObject.IsinstantiableObject); //open the disabling scope (it works!!)

        enemyHittableObject.instantiateAfterAttack = (GameObject)EditorGUILayout.ObjectField("Instantiable Object", enemyHittableObject.instantiateAfterAttack, typeof(GameObject), true); //te field which should be visible

        EditorGUI.EndDisabledGroup();//end the disabling scope

        enemyHittableObject.ObjectTag = EditorGUILayout.TextField("Object Tag", enemyHittableObject.ObjectTag);
    }

}
