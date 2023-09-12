using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class WayPointsMovement : MonoBehaviour
{
    [SerializeField] WayPoints[] Waypoints;
    [SerializeField] float MovementSpeed;
    private int currentIndex = 0;
    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    async void Update()
    {
        if (await WayPointDistance())
        { 
            currentIndex++;

            if(await isCurrentIndexValueWithinBounds())
            {
                currentIndex = 0;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, Waypoints[currentIndex].wayPoint.position, Time.deltaTime * MovementSpeed);

        sr.flipX = await CharacterFlip(currentIndex);

    }

    public Task<bool> CharacterFlip(int index)
    {
        bool characterFlip = Waypoints[index].leftWayPoint ? false : true;

        return Task.FromResult(characterFlip);
  
    }

    public Task<bool> WayPointDistance()
    {
        return Task.FromResult(Vector2.Distance(transform.position, Waypoints[currentIndex].wayPoint.position) <.1f);
    
    }

    public Task<bool> isCurrentIndexValueWithinBounds()
    {
        return Task.FromResult(currentIndex >= Waypoints.Length);
    }
}
