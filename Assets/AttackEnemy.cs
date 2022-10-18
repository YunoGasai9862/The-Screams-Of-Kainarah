using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemy : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Enemy;

    private float DaggerSpeed = 30f;

    void Update()
    {

        GetComponent<Rigidbody2D>().AddForce(Vector2.right * DaggerSpeed * Time.deltaTime);

    }
}
