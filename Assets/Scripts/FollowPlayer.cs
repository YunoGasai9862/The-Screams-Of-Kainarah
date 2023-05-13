using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private static GameObject Player;
 
     private void Start()
    {
        Player = GameObject.FindWithTag("Player");
       
    }

    public static void TrackPlayer(Transform Follower, float xOffset, float yOffset, float zOffset)
    {
        Follower.transform.position = new Vector3(Player.transform.position.x + xOffset, Player.transform.position.y + yOffset, Follower.transform.position.z + zOffset);

    }

}
