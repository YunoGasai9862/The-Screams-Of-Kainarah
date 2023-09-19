
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyHittableManager : MonoBehaviour
{

    [Header("Objects That Can be Treated As Hit (tag)")]
    [SerializeField] public EnemyHittableObjects _enemyHittableObjects;

    public Task<bool> isEntityAnAttackObject(Collider2D collider, EnemyHittableObjects objects )
    {
        for (int i = 0; i < 2; i++)
        {
          /*  if (collider.tag == tags[i].enemyHittableObjectsReference)
            {
                return Task.FromResult(true);
            }
          */
        }

        return Task.FromResult(false);
    }
}
