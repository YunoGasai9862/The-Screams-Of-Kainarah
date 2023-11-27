using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class CrystalGenerator : MonoBehaviour
{
    [SerializeField] Tilemap _tiles;
    private List<Vector3> _ledgePositions;
    [SerializeField] GameObject Crystals;
    [SerializeField] 
    float zPos;
    void Start()
    {
        _ledgePositions = new List<Vector3>();
        for (int x = _tiles.cellBounds.xMin; x < _tiles.cellBounds.xMax; x++)  //I learned this new thing
        {
            for (int y = _tiles.cellBounds.yMin; y < _tiles.cellBounds.yMax; y++)
            {

                Vector3Int LocationOnTile = new(x, y, (int)zPos); //i dont know why are we using the y Position

                Vector3 localSpace = _tiles.CellToWorld(LocationOnTile);

                if (_tiles.HasTile(LocationOnTile))
                {
                    //has tile
                    _ledgePositions.Add(LocationOnTile);
                    Vector3 AdjustedPosition = new(localSpace.x - .5f, localSpace.y + 1.5f, localSpace.z);
                    Instantiate(Crystals, AdjustedPosition, Quaternion.identity);
                }

            }
        }





    }


}
