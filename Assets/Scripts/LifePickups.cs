using UnityEngine;
using UnityEngine.Tilemaps;

public class LifePickups : MonoBehaviour
{

    [SerializeField] Tilemap _ground;
    [SerializeField] GameObject LifePickup;
    void Start()
    {
        for (int x = _ground.cellBounds.xMin; x < _ground.cellBounds.xMax; x++)
        {
            for (int y = _ground.cellBounds.yMin; y < _ground.cellBounds.yMax; y++)
            {
                Vector3Int GroundPos = new Vector3Int(x, y, (int)_ground.transform.position.y);
                Vector3 localSpace = _ground.CellToWorld(GroundPos);

                if (_ground.HasTile(GroundPos))
                {
                    if (x % 2 == 0)
                    {
                        Vector2 LifeLocation = new Vector2(localSpace.x, localSpace.y + 2.5f);
                        Instantiate(LifePickup, LifeLocation, Quaternion.identity);
                    }

                }

            }
        }


    }


}
