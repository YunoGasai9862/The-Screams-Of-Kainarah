
namespace EnemyHittable
{
    using System;
    using UnityEngine;

    [CreateAssetMenu(fileName = "EnemyHittableObjectsScriptableEntity", menuName = "Enemy Hittable Object")]
    public class EnemyHittableObjects : ScriptableObject
    {
        [Serializable]
        public class HittableObjects
        {
            public bool IsinstantiableObject;
            public GameObject instantiateAfterAttack;
            public string ObjectTag;
        }

        public HittableObjects[] elements; //better to create an array so i can store many Elements

    }

}
