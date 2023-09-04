using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private static GameObject Player;

    private void Start()
    {
        Player = GameObject.FindWithTag("Player");

    }

    public static void TrackPlayer(Transform Follower, float xOffset, float yOffset, float zOffset, float CameraSpeed)
    {
        Vector3 newPosition= new(Player.transform.position.x + xOffset, Player.transform.position.y + yOffset, Follower.transform.position.z + zOffset);
        Follower.transform.position = Vector3.MoveTowards(Follower.transform.position, newPosition, CameraSpeed * Time.deltaTime);
    }

}
