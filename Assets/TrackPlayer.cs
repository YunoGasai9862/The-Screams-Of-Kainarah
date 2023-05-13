using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPlayer : MonoBehaviour
{
   
    void Update()
    {
        FollowPlayer.TrackPlayer(this.transform, 0, 25, 0);
    }
}
