using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPlayer : MonoBehaviour
{
   
    void Update()
    {
        FollowPlayer.TrackPlayer(transform, 0, 25, 0, 0f);
    }
}
