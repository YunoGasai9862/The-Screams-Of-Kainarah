using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class CrystalGenerator : MonoBehaviour
{
    [SerializeField] Tilemap _tiles;
    private List<Vector3> _ledgePositions;
    [SerializeField] GameObject Crystals;
    void Start()
    {
        _ledgePositions= new List<Vector3>();
        for(int x=_tiles.cellBounds.xMin; x<_tiles.cellBounds.xMax; x++)  //I learned this new thing
        {

            for(int y= _tiles.cellBounds.yMin; y<_tiles.cellBounds.yMax; y++)
            {
                Vector3Int LocationOnTile = new Vector3Int(x, y, (int)_tiles.transform.position.y); //i dont know why are we using the y Position
                                                                                             //now convert the Tile world Pos to World Pos
                Vector3 localSpace = _tiles.WorldToLocal(LocationOnTile);

                if(_tiles.HasTile(LocationOnTile))
                {
                    //has tile
                    _ledgePositions.Add(localSpace);
                    Vector3 AdjustedPosition = new Vector3(localSpace.x - 1f, localSpace.y + 1f, localSpace.z);
                    Instantiate(Crystals, AdjustedPosition, Quaternion.identity);
                }
               
            }
        }

     



    }


}
