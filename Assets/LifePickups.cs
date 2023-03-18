using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LifePickups : MonoBehaviour
{

    [SerializeField] Tilemap _ground;
    [SerializeField] GameObject LifePickup;
    void Start()
    {
        for(int x=_ground.cellBounds.xMin; x<_ground.cellBounds.xMax; x++)
        {
            for(int y=_ground.cellBounds.yMin; y<_ground.cellBounds.yMax; y++)
            {

                Vector3Int GroundPos = new Vector3Int(x, y, (int)_ground.transform.position.y);
                 
            }
        }
        
        
    }

   
}
