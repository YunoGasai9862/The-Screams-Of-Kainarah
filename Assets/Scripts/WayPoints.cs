
namespace WayPointsObject
{
    using System;
    using UnityEngine;

    [Serializable]
    //implement custom editor
    public class WayPoints : Scene
    {
        [SerializeField] public Transform wayPoint;
        public bool leftWayPoint;
        public bool rightWayPoint;
    }
}
