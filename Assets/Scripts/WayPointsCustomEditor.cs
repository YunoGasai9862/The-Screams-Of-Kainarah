
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WayPoints))]
public class WayPointsCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //override how the inspector for WayPoints look like
        WayPoints wayPoints = (WayPoints)target;

        EditorGUILayout.LabelField("Custom Layout for WayPoints");
        //display the default one
        EditorGUILayout.ObjectField("WayPoint", wayPoints.wayPoint, typeof(Transform), true);

        EditorGUILayout.LabelField("Select One: ");

        EditorGUI.indentLevel++; //indents by one

        wayPoints.leftWayPoint = EditorGUILayout.Toggle("Left WayPoint", wayPoints.leftWayPoint);
        if (wayPoints.leftWayPoint)
            wayPoints.rightWayPoint = false;
        wayPoints.rightWayPoint = EditorGUILayout.Toggle("Right WayPoint", wayPoints.rightWayPoint);
        if (wayPoints.rightWayPoint)
            wayPoints.leftWayPoint = false;


        EditorGUI.indentLevel--; 


    }
}
