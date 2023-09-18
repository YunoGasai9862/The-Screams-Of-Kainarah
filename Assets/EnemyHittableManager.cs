
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyHittableManager : MonoBehaviour
{

    [Header("Objects That Can be Treated As Hit (tag)")]
    [SerializeField] public List<EnemyHittableObjects> objects_or_prefabs= new();

    public Task<bool> isEntityAnAttackObject(Collider2D collider, List<EnemyHittableObjects> tags)
    {
        for (int i = 0; i < objects_or_prefabs.Count; i++)
        {
            if (collider.tag == objects_or_prefabs[i].ObjectTag)
            {
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }
}
