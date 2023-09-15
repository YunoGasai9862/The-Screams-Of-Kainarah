using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyHittableManager : MonoBehaviour
{
 
    [Header("Objects That Can be Treated As Hit (tag)")]
    [SerializeField] List<EnemyHittableObjects> tags;
    public Task<bool> isEntityAnAttackObject(Collider2D collider, List<EnemyHittableObjects> tags)
    {
        for (int i = 0; i < tags.Count; i++)
        {
            if (collider.tag == tags[i].ObjectTag)
            {
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }
}
