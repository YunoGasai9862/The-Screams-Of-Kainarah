using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SceneData
{
    [Serializable]
    public class ObjectData
    {
        public Vector3 position;
        public Quaternion rotation;
        public string tag;
        public string name;
    }

    IList<ObjectData> objectsToPersist = new List<ObjectData>();
}
