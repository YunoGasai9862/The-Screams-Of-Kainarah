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
    public LayerMask layerMaskForGrounding;
    public float forceMagnitude;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Path path;
    private int currentIndex=-1;
    private int sign;
    private int currentWayPointIndex = 0;
    private Transform selectedTargetToMoveToward;
    private bool isGrounded;
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
            seeker.StartPath(rb.position, WayPoints[currentIndex].position, OnPathComplete);
        }
     
    }

    private async Task<bool> IsInVisibleDistance() //find the closest first
    {
        if(currentIndex < WayPoints.Length-1 || currentIndex < 0)
        {
            sign = 1;
        }else
        {
            sign = -1;
        }
        currentIndex = currentIndex + sign;
        bool isInDistance = Vector2.Distance(transform.position, WayPoints[currentIndex].position) < activeDistance;
        if(isInDistance)
         selectedTargetToMoveToward= WayPoints[currentIndex].transform; 
        await Task.Delay(500);
        return isInDistance;

    }

    private void OnPathComplete(Path p) 
    {
        if(!p.error)
        {
            path = p;
            currentWayPointIndex = 0;//resets (this index is for all the waypoints between the wayPoints i have specified)
        }
    }

    private async void PathToFollow()
    {
        if(path==null)
        {
            return; //there's an error -> exit (nothing to follow)
        }

        if(currentWayPointIndex > path.vectorPath.Count)
        {
            return; //crossed all waypoints so far
        }

        isGrounded = Physics2D.Raycast(rb.position, -Vector3.up, 1f, layerMaskForGrounding);

        Vector3 direction = ((Vector2)path.vectorPath[currentWayPointIndex]- rb.position).normalized;  //the waypoint index in the path selected for that true value
        Vector3 force = direction * rb.mass * forceMagnitude;

    }
}
