using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private GameObject Player;
 
     private void Start()
    {
        Player = GameObject.FindWithTag("Player");
        

    }
    void Update()
    {
            transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + 15,transform.position.z);
         
       

    }
}
