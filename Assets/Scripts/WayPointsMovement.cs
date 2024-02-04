using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using WayPointsObject;

public class WayPointsMovement : MonoBehaviour
{
    [SerializeField] WayPoints[] Waypoints;
    [SerializeField] float MovementSpeed;
    private int currentIndex = 0;
    private SpriteRenderer sr;
    private bool shouldTargetMove;
    public bool shouldMove { get => shouldTargetMove; set => shouldTargetMove = value; }

    private void Awake()
    {
        shouldMove = true;
    }
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    async void Update()
    {
        if (await WayPointDistance())
        { 
            currentIndex++;

            if(await IsCurrentIndexValueWithinBounds())
            {
                currentIndex = 0;
            }
        }

        if (shouldTargetMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, Waypoints[currentIndex].wayPoint.position, Time.deltaTime * MovementSpeed);

            Vector3 rotation = await CharacterFlip(currentIndex);

            transform.localRotation= Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        }

    }

    public Task<Vector3> CharacterFlip(int index)
    {

        Vector3 localRotation = Waypoints[index].leftWayPoint ? new Vector3(0, 0, 0) : new Vector3(0, 180, 0);

        return Task.FromResult(localRotation);
  
    }

    public Task<bool> WayPointDistance()
    {
        return Task.FromResult(Vector2.Distance(transform.position, Waypoints[currentIndex].wayPoint.position) <.1f);
    
    }

    public Task<bool> IsCurrentIndexValueWithinBounds()
    {
        return Task.FromResult(currentIndex >= Waypoints.Length);
    }
}
