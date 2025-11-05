using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent: MonoBehaviour
{
    [SerializeField]
    List<GameObject> iceTrailPrefabs;

    public void IceTrail()
    {
        iceTrailPrefabs.ForEach(prefab =>
        {
            InstantiateUtility iceTrail = new(prefab);
            iceTrail.InstantiateObject(transform.position, Quaternion.identity);
            iceTrail.SetObjectsParent(transform);
        });
    }
}