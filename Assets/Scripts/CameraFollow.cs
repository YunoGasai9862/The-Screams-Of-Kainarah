using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
  
    void Update()
    {
        FollowPlayer.TrackPlayer(transform, 0, 15, 0);
    }
}
