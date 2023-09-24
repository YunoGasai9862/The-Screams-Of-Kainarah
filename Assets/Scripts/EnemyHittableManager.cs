
using System.Threading.Tasks;
using UnityEngine;

public class EnemyHittableManager : MonoBehaviour
{
    public static Task<bool> isEntityAnAttackObject(Collider2D collider, EnemyHittableObjects objects)
    {
        for (int i = 0; i < objects.elements.Length; i++)
        {
            var element = objects.elements[i];
            if (collider.tag == element.ObjectTag) //scriptable Object
            {
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }
}
