using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyHittableObjectsScriptableEntity", menuName = "Enemy Hittable Object")]
public class EnemyHittableObjects : ScriptableObject
{
    [Serializable]
    public class Elements
    {
        public bool IsinstantiableObject;
        public GameObject instantiateAfterAttack;
        public string ObjectTag;
    }

    public Elements[] elements;

}