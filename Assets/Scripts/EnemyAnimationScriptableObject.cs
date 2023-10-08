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
        //will select only one of them
        public bool selectIntValue;
        public bool selectBoolValue;
        public bool selectFloatValue;
        public bool selectStringValue;
        public typeValuePair<int> valueInt;
        public typeValuePair<string> valueString;
        public typeValuePair<float> valueFloat;
        public typeValuePair<bool> valueBool;

    }

    [Serializable]
    public class typeValuePair<T>
    {
        public T value;
    }

    public EnemyAnimationDetails[] eachAnimation;
}
