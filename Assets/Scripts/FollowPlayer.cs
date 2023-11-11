using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private static GameObject Player;

    private void Start()
    {
        Player = GameObject.FindWithTag("Player");

    }

    public static void TrackPlayerX(Transform Follower, float xOffset, float yOffset, float zOffset, float speed)
    {
        Vector3 newPosition= new(Player.transform.position.x + xOffset, yOffset, zOffset);
        Follower.transform.position = Vector3.MoveTowards(Follower.transform.position, newPosition, speed * Time.deltaTime);
    }

    public static void TrackPlayerY(Transform Follower, float xOffset, float yOffset, float zOffset, float speed)
    {
        Vector3 newPosition = new(xOffset, Player.transform.position.y + yOffset, zOffset);
        Follower.transform.position = Vector3.MoveTowards(Follower.transform.position, newPosition, speed * Time.deltaTime);
    }

    public static void TrackPlayerZ(Transform Follower, float xOffset, float yOffset, float zOffset, float speed)
    {
        Vector3 newPosition = new(xOffset, yOffset, Follower.transform.position.z + zOffset);
        Follower.transform.position = Vector3.MoveTowards(Follower.transform.position, newPosition, speed * Time.deltaTime);
    }

    public static void TrackPlayer(Transform Follower, float xOffset, float yOffset, float zOffset, float speed)
    {
        Vector3 newPosition = new(Player.transform.position.x + xOffset, Player.transform.position.y + yOffset, Follower.transform.position.z + zOffset);
        Follower.transform.position = Vector3.MoveTowards(Follower.transform.position, newPosition, speed * Time.deltaTime);
    }

    public static void TrackPlayerX(Transform Follower, float distance, float xOffset, float yOffset, float zOffset, float speed)
    {
        if(Vector2.Distance(Follower.position, Player.transform.position) > distance)
        {
            Vector3 newPosition = new(Player.transform.position.x + xOffset, yOffset, zOffset);
            Follower.transform.position = Vector3.MoveTowards(Follower.transform.position, newPosition, speed * Time.deltaTime);
        }
    }


}
