using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WayPointsMovement : MonoBehaviour
{
    [SerializeField] Transform[] Waypoints;
    [SerializeField] float MovementSpeed;
    private int currentIndex = 0;


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

        transform.position = Vector2.MoveTowards(transform.position, Waypoints[currentIndex].position, Time.deltaTime * MovementSpeed);

    }

    public Task<bool> WayPointDistance()
    {
        return Task.FromResult(Vector2.Distance(transform.position, Waypoints[currentIndex].position) <.1f);
    
    }

    public Task<bool> isCurrentIndexValueWithinBounds()
    {
        return Task.FromResult(currentIndex >= Waypoints.Length);
    }
}
