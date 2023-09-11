using System;
using UnityEngine;

[Serializable]
//implement custom editor
public class WayPoints: MonoBehaviour
{
    [SerializeField] public Transform wayPoint;
    public bool leftWayPoint;
    public bool rightWayPoint;
}