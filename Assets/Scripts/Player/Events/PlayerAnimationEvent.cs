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
            InstantiatorController iceTrail = new(prefab);
            iceTrail.InstantiateGameObject(transform.position, Quaternion.identity);
            iceTrail.SetGameObjectParent(transform);
        });
    }
}