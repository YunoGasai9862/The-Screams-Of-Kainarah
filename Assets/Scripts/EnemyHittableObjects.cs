using System;
using UnityEngine;

[Serializable]
public class EnemyHittableObjects : MonoBehaviour
{
    public bool IsinstantiableObject;
    public GameObject instantiateAfterAttack;
    public string ObjectTag;
}
