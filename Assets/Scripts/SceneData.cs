using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SceneData
{
    protected IList<ObjectData> ObjectsToPersit { get => objectsToPersist; set => objectsToPersist = value; }
    [Serializable]
    public class ObjectData: SceneData
    {
        public Vector3 position;
        public Quaternion rotation;
        public string tag;
        public string name;
        public ObjectData(Vector3 pos, Quaternion rot, string tag, string name)
        {
            this.position = pos;
            this.rotation = rot;
            this.tag = tag;
            this.name = name;
        }
        public void AddToObjectsToPersist(ObjectData data)
        {
            ObjectsToPersit.Add(data);
        }
    }

   private IList<ObjectData> objectsToPersist = new List<ObjectData>();
}
