using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SceneData
{
    public IList<ObjectData> ObjectsToPersit { get => objectsToPersist; set => objectsToPersist = value; }
    [Serializable]
    public class ObjectData
    {
        public Vector3 position;
        public Quaternion rotation;
        public string tag;
        public string name;
    }

   private IList<ObjectData> objectsToPersist = new List<ObjectData>();
}
