using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private GameObject Player;
    private float HalfHeight, HalfWidth;
    [SerializeField] float CameraMinSize;
    private void Start()
    {
        Player = GameObject.FindWithTag("Player");
      //  HalfHeight = (Camera.main.orthographicSize);  //gives you half the height
      //  HalfWidth = HalfHeight * Camera.main.aspect;  //gives you half of its width by multiplying camera's aspect ratio

    }
    void Update()
    {



        if (transform.position.x < Player.transform.position.x || transform.position.x > CameraMinSize)
                 transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + 15,transform.position.z);

    }
}
