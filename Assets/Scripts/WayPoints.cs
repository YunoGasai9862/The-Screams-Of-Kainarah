
namespace WayPointsObject
{
    using Assets.Scripts.Scene;
    using System;
    using UnityEngine;

    [Serializable]
    //implement custom editor
    public class WayPoints : MonoBehaviorScene
    {
        [SerializeField] public Transform wayPoint;
        public bool leftWayPoint;
        public bool rightWayPoint;
    }
}
