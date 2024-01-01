using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
public class ObjectGenerator : MonoBehaviour
{
    [HideInInspector]
    [SerializeField] Tilemap tiles;
    [HideInInspector]
    [SerializeField] GameObject prefabWithLocations;
    [SerializeField] GameObject itemToInstantiate;
    [SerializeField] Vector3 offsetForInstantiation;
    [SerializeField] bool shouldUseTiles;
    private int _numOfItems = 0;
    float zPos;
    async void Start()
    {
        if (shouldUseTiles)
           await PopulateTheSceneWithGameObjectsTilesV(_numOfItems, itemToInstantiate, tiles, offsetForInstantiation);
        else
           await PopulateTheSceneWithGameObjectsArrayV(itemToInstantiate, offsetForInstantiation, prefabWithLocations);
  
    }

    public async Task PopulateTheSceneWithGameObjectsTilesV(int incrementor, GameObject itemToInstantiate, Tilemap tiles, Vector3 offset)
    {
       var _ledgePositions = new List<Vector3>();

        for (int x = tiles.cellBounds.xMin; x < tiles.cellBounds.xMax; x++)
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
        await Task.CompletedTask;
    }

    public async Task PopulateTheSceneWithGameObjectsArrayV(GameObject itemToInstantiate, Vector3 offset, GameObject prefabWithLocations)
    {
        for (int x = 0; x < prefabWithLocations.transform.childCount; x++) 
        {
            Vector3 childObjectPos = prefabWithLocations.transform.GetChild(x).transform.position;
            Vector3 AdjustedPosition = new(childObjectPos.x + offset.x, childObjectPos.y + offset.y, childObjectPos.z + offset.z);  // x=-.5f y= 1.5f z=0.0f
            GameObject instantiatedObject = Instantiate(itemToInstantiate, AdjustedPosition, Quaternion.identity);
            instantiatedObject.name = itemToInstantiate.name.ToString() + $"{x}";        
        }
        await Task.CompletedTask;

    }
}
