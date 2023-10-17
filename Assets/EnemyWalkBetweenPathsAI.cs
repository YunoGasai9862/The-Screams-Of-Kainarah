using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyWalkBetweenPathsAI : MonoBehaviour
{

    [Header("Pathfinding Variables")]
    public float updatePathSeconds;
    public float activeDistance;
    public Transform[] WayPoints;


    [Header("Custom Behavior")]
    public bool isFollowEnabled;
    public bool isJumpEnabled;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Path path;
    private int currentIndex=0;
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, updatePathSeconds);
    }

    private async void FixedUpdate()
    {
        if(await IsInVisibleDistance() && isFollowEnabled)
        {
            PathToFollow();
        }
    }

    private async void UpdatePath()
    { 
       if(await IsInVisibleDistance() && isFollowEnabled && seeker.IsDone()) //if one path is finished
        {
          //  seeker.StartPath(rb.position, )
        }
     
    }

    private async Task<bool> IsInVisibleDistance() //find the closest first
    {
        int sign;
        if(currentIndex < WayPoints.Length)
        {
            sign = 1;
        }else
        {
            sign = -1;
        }

        bool isInDistance = Vector2.Distance(transform.position, WayPoints[currentIndex].position) < activeDistance;
        currentIndex = currentIndex + sign;
        await Task.Delay(500);
        return isInDistance;

    }

    private async void PathToFollow()
    {

    }
}
