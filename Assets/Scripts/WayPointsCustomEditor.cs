
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WayPoints))]
public class WayPointsCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //override how the inspector for WayPoints look like
        WayPoints wayPoints = (WayPoints)target;  //always have the target ready (target is the object we use for the editing, and we cast it to Waypoints, the nwe can use wayPoints to directly modify editor values)

        EditorGUILayout.LabelField("Custom Layout for WayPoints");
        //display the default one
        wayPoints.wayPoint= (Transform)EditorGUILayout.ObjectField("Way Point", wayPoints.wayPoint, typeof(Transform), true); //we had to assign it similar to what we were doing with bool values

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
