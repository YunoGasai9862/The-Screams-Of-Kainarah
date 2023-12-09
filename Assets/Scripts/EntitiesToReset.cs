using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EntitiesToReset", menuName ="Entities To Reset")]
public class EntitiesToReset : ScriptableObject
{
    [Serializable]
    public class EntityReset
    {
        public GameObject entity;
    }

    public EntityReset[] entitiesToReset;
}