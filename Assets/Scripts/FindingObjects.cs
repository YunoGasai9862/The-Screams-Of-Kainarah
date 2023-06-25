using UnityEngine;

public class FindingObjects : MonoBehaviour
{

    public static bool FindObject(GameObject source, string tag)
    {
        RaycastHit2D ray;
        ray = Physics2D.Raycast(source.transform.position, Vector3.right, 10f);
        Debug.DrawRay(source.transform.position, Vector3.right * 10f, Color.blue);
        if (ray.collider != null && ray.collider.CompareTag(tag))
        {

            return true;
        }


        return false;
    }


}