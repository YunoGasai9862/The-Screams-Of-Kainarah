using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Serialization;
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
        public AbstractEntity entity=null;

        public float health;
        public ObjectData(string tag, string name,Vector3 pos, Quaternion rot, AbstractEntity entity)
        {
            this.tag = tag;
            this.name = name;
            this.position = pos;
            this.rotation = rot;
            this.entity= entity;
            this.health = entity.Health;
        }

        public ObjectData(string tag, string name, Vector3 pos, Quaternion rot)
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
    [Serializable]
    public class CompleteObject
    {
        public GameObject gameObject;
        public Transform transform;

        public CompleteObject(GameObject gameObject, Transform transform)
        {
            this.gameObject = gameObject;
            this.transform = transform;
        }

    }
    public void AddToObjectsToPersist(ObjectData data)
    {
        ObjectsToPersit.Add(data);
    }

    private IList<ObjectData> objectsToPersist = new List<ObjectData>();
}
