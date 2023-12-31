using UnityEngine;
using UnityEngine.Tilemaps;

public class LifePickups : MonoBehaviour
{
    [SerializeField] Tilemap _ground;
    [SerializeField] GameObject lifePickup;
    [SerializeField] float zPos;
    private int _numberOfLifePickUps = 0;
    private string condition = "";
    void Start()
    {
        for (int x = _ground.cellBounds.xMin; x < _ground.cellBounds.xMax; x++)
        {
            for (int y = _ground.cellBounds.yMin; y < _ground.cellBounds.yMax; y++)
            {
                Vector3Int GroundPos = new Vector3Int(x, y, (int)zPos);
                Vector3 localSpace = _ground.CellToWorld(GroundPos);

                if (_ground.HasTile(GroundPos))
                {
                    if (x % 2 == 0)
                    {
                        Vector2 LifeLocation = new Vector2(localSpace.x + 2f, localSpace.y + 2f);
                        GameObject lifePU= Instantiate(lifePickup, LifeLocation, Quaternion.identity);
                        lifePU.name = lifePickup.name.ToString() + $"{_numberOfLifePickUps}";
                        _numberOfLifePickUps++;
                    }

                }

            }
        }


    }


}
