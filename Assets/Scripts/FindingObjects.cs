using UnityEngine;

public class FindingObjects : MonoBehaviour
{
    public static bool CastRayToFindObject(GameObject source, string tag, float rayCastLength)
    {
        RaycastHit2D ray;
        ray = Physics2D.Raycast(source.transform.position, Vector3.right, rayCastLength);
        Debug.DrawRay(source.transform.position, Vector3.right * rayCastLength, Color.blue);
        if (ray.collider != null && ray.collider.CompareTag(tag))
        {
            return true;
        }

        return false;
    }

}