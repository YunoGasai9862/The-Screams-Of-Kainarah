using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class CrystalGenerator : MonoBehaviour
{
    [SerializeField] Tilemap _tiles;
    private List<Vector3> _ledgePositions;
    void Start()
    {
        _ledgePositions= new List<Vector3>();
        for(int x=_tiles.cellBounds.xMin; x<_tiles.cellBounds.xMax; x++)
        {

            for(int y= _tiles.cellBounds.yMin; y<_tiles.cellBounds.yMax; y++)
            {
                Vector3Int LocationOnTile = new Vector3Int(x, y, (int)_tiles.transform.position.y);//i dont know why are we using the y Position
                //now convert the Tile world Pos to World Pos

                Vector3 WorldLocation=_tiles.WorldToLocal(LocationOnTile);
                if(_tiles.HasTile(LocationOnTile)) //if that tile exists, and is not a blank tile
                {
                    _ledgePositions.Add(WorldLocation);  //OMG this work because there are only 5 tiles so far!
                    Debug.Log(WorldLocation);
                }else
                {
                    //the tile is empty
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
