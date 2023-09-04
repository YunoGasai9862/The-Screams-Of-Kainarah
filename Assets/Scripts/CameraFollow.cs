using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private CameraShake _cameraShakeScript;

    [Header("Camera Follow Speed")]
    [SerializeField] float _cameraFollowSpeed;

    private void Awake()
    {
        _cameraShakeScript= GetComponent<CameraShake>();
    }

    void Update()
    {
        if(!_cameraShakeScript.isShaking)
        {
            FollowPlayer.TrackPlayer(transform, 0, 15, 0, _cameraFollowSpeed);
        }
    }
}
