using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGenerator : MonoBehaviour
{
    [SerializeField] GameObject smokeFlare;
    [SerializeField] int numberOfFlares;
    [SerializeField] int gapBetweenSmokeObjects;
    private Vector2 _flarePos;
    void Start()
    {
        for(int i=0; i<numberOfFlares; i++)
        {
            _flarePos = new Vector3(transform.position.x - (i * gapBetweenSmokeObjects), transform.position.y);
            GameObject smoke= Instantiate(smokeFlare, _flarePos, Quaternion.identity);
            smoke.name = $"{smokeFlare.name}{i}";
        }
        
    }

   
}
