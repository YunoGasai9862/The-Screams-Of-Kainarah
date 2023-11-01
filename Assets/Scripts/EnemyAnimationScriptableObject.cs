
namespace EnemyAnimation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "EnemyAnimationScriptableObjectEntity", menuName="Enemy Animation Object" )]
    public class EnemyAnimationScriptableObject : ScriptableObject
    {
    [Serializable]
    public class EnemyAnimationDetails
    {
        public string animationName;
        public bool selectIntValue;
        public bool selectBoolValue;
        public bool selectFloatValue;
        public bool selectStringValue;
        public int valueInt;
        public string valueString;
        public float valueFloat;
        public bool valueBool;

    }

    public EnemyAnimationDetails[] eachAnimation;
    }
}