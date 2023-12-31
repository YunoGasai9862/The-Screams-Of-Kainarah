using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class ObjectGenerator : MonoBehaviour
{
    [SerializeField] Tilemap _tiles;
    [SerializeField] GameObject itemToInstantiate;
    [SerializeField] Vector3 offsetForInstantiation;
    private int _numOfItems = 0;
    float zPos;
    void Start()
    {
        PopulateTheSceneWithGameObjects(_numOfItems, itemToInstantiate, _tiles, offsetForInstantiation);

    }

    public void PopulateTheSceneWithGameObjects(int incrementor, GameObject itemToInstantiate, Tilemap tiles, Vector3 offset)
    {
       var _ledgePositions = new List<Vector3>();

        for (int x = tiles.cellBounds.xMin; x < tiles.cellBounds.xMax; x++)  //I learned this new thing
        {
            for (int y = tiles.cellBounds.yMin; y < tiles.cellBounds.yMax; y++)
            {
                Vector3Int LocationOnTile = new(x, y, (int)zPos); //i dont know why are we using the y Position

                Vector3 localSpace = tiles.CellToWorld(LocationOnTile);

                if (tiles.HasTile(LocationOnTile))
                {
                    //fix this tomorrow for life pickups
                    _ledgePositions.Add(LocationOnTile);
                    Vector3 AdjustedPosition = new(localSpace.x + offset.x, localSpace.y + offset.y, localSpace.z + offset.z);  // x=-.5f y= 1.5f z=0.0f
                    GameObject instantiatedObject = Instantiate(itemToInstantiate, AdjustedPosition, Quaternion.identity);
                    instantiatedObject.name = itemToInstantiate.name.ToString() + $"{incrementor}";
                    incrementor++;
                }
            }
        }
    }
}
