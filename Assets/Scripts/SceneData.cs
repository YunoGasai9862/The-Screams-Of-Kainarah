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
        public string tag;
        public string name;
        public Vector3 position;
        public Quaternion rotation;
        public ObjectData(string tag, string name,Vector3 pos, Quaternion rot)
        {
            this.tag = tag;
            this.name = name;
            this.position = pos;
            this.rotation = rot;
        }

        public override string ToString()
        {
            return $"Pos: {this.position}, Rot: {this.rotation}, Tag: {this.tag}, Name: {this.name}";
        }

    }
    public void AddToObjectsToPersist(ObjectData data)
    {
        Debug.Log($"TESTING:  {data}");
        ObjectsToPersit.Add(data);
    }

    private IList<ObjectData> objectsToPersist = new List<ObjectData>();
}
