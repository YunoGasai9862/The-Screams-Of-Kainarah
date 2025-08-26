using UnityEngine;

public class FollowPlayer
{
    public static void TrackPlayerX(Transform follower, Transform toBeFollow, Vector3 offsetVector, float speed)
    {
        Vector3 newPosition= new(toBeFollow.position.x + offsetVector.x, offsetVector.y, offsetVector.z);
        follower.transform.position = Vector3.MoveTowards(follower.transform.position, newPosition, speed * Time.deltaTime);
    }

    public static void TrackPlayerY(Transform follower, Transform toBeFollow, Vector3 offsetVector, float speed)
    {
        Vector3 newPosition = new(offsetVector.x, toBeFollow.position.y + offsetVector.y, offsetVector.z);
        follower.transform.position = Vector3.MoveTowards(follower.transform.position, newPosition, speed * Time.deltaTime);
    }

    public static void TrackPlayerZ(Transform follower, Vector3 offsetVector, float speed)
    {
        Vector3 newPosition = new(offsetVector.x, offsetVector.y, follower.transform.position.z + offsetVector.z);
        follower.transform.position = Vector3.MoveTowards(follower.transform.position, newPosition, speed * Time.deltaTime);
    }

    public static void TrackPlayer(Transform follower, Transform toBeFollow, Vector3 offsetVector, float speed)
    {
        Vector3 newPosition = new(toBeFollow.position.x + offsetVector.x, toBeFollow.position.y + offsetVector.y, follower.transform.position.z + offsetVector.z);
        follower.transform.position = Vector3.MoveTowards(follower.transform.position, newPosition, speed * Time.deltaTime);
    }

    public static void TrackPlayerX(Transform follower, Transform toBeFollow, float distance, Vector3 offsetVector, float speed)
    {
        if(Vector2.Distance(follower.position, toBeFollow.position) > distance)
        {
            Vector3 newPosition = new(toBeFollow.position.x + offsetVector.x, offsetVector.y, offsetVector.z);
            follower.transform.position = Vector3.MoveTowards(follower.transform.position, newPosition, speed * Time.deltaTime);
        }
    }
}
