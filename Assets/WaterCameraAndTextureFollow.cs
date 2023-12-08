using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCameraAndTextureFollow : MonoBehaviour
{
    [SerializeField]
    public float WaterCamerSpeed;
    public float offsetX;
    void Update()
    {
        FollowPlayer.TrackPlayerX(transform, offsetX,transform.position.y, transform.position.z, WaterCamerSpeed);
    }
}
