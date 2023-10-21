using Pathfinding;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyWalkBetweenPathsAI : MonoBehaviour
{

    [Header("Pathfinding Variables")]
    public float updatePathSeconds;
    public float farDistance;
    public float closeDistance;
    public Transform[] WayPoints;
    public float nextWayPointDistance; //tells you how much to move until the next waypoint
    public float jumpCheckOffset;


    [Header("Custom Behavior")]
    public bool isFollowEnabled;
    public bool isJumpEnabled;
    public LayerMask layerMaskForGrounding;
    public float forceMagnitude;
    public float jumpHeight;
    public float jumpPower;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Path path;
    private int currentIndex = 0;
    private int sign;
    private int currentWayPointIndex = 0;
    private Transform selectedTargetToMoveToward;
    private bool isJumping=false;
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, updatePathSeconds);
    }

    private async void FixedUpdate()
    {
        if (await IsInVisibleDistance() && isFollowEnabled)
        {
            PathToFollow();
        }
    }

    private async void UpdatePath()
    {
        if (await IsInVisibleDistance() &&
        isFollowEnabled && seeker.IsDone()) //if one path is finished
        {
            seeker.StartPath(rb.position, selectedTargetToMoveToward.position, OnPathComplete);
        }

    }

    private async Task<bool> IsInVisibleDistance() //find the closest first
    {
        bool inDistance = false;

        if (currentIndex >= WayPoints.Length - 1)
        {
            sign = -1;
        }
        if (currentIndex <= 0)
        {
            sign = 1;
        }
        await Task.Delay(5);

        if (Vector2.Distance(transform.position, WayPoints[currentIndex].position) < farDistance && Vector2.Distance(transform.position, WayPoints[currentIndex].position) > closeDistance)
        {
            selectedTargetToMoveToward = WayPoints[currentIndex].transform;
            inDistance = true;

        }

        if (Vector2.Distance(transform.position, WayPoints[currentIndex].position) < closeDistance)
        {
            currentIndex = currentIndex + sign;
        }

        return inDistance;


    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPointIndex = 0;//resets (this index is for all the waypoints between the wayPoints i have specified)
        }
    }

    private async void PathToFollow()
    {
        if (path == null)
        {
            return; //there's an error -> exit (nothing to follow)
        }

        if (currentWayPointIndex >= path.vectorPath.Count)
        {
            return; //crossed all waypoints so far
        }


        Vector3 direction = ((Vector2)path.vectorPath[currentWayPointIndex] - rb.position).normalized;  //the waypoint index in the path selected for that true value
        Vector3 force = direction * rb.mass * forceMagnitude * Time.deltaTime;


        if (await canJump(isJumpEnabled))
        {
            if (direction.y > jumpHeight && !isJumping) //if the direction of y is above, then jump
            {
                isJumping = true;

                rb.AddForce(Vector2.up * forceMagnitude * rb.mass * jumpPower);
            }

        }

        if(await canJump(isJumpEnabled) && isJumping)
        {
            await Task.Delay(1000);
            isJumping = false;
        }

        rb.AddForce(force, ForceMode2D.Force);

        if(currentWayPointIndex < path.vectorPath.Count)
        {
            float distance = Vector3.Distance(rb.position, path.vectorPath[currentWayPointIndex]);

            if (distance < nextWayPointDistance)
            {
                currentWayPointIndex++; //move to next path (current waypoint has been reached)
            }

        }
        Vector3 rotation = await flipCharacter(0, 0, 0);

        if (rb.velocity.x > .05f)
        {
            transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        }

        if(rb.velocity.x < -.05f)
        {
            transform.localRotation = Quaternion.Euler(rotation.x, rotation.y - 180, rotation.z);

        }

    }

    private Task<Vector3> flipCharacter(int valueX, int valueY, int intValueZ)
    {
        return Task.FromResult(new Vector3(valueX, valueY, intValueZ));
    }

    private async Task<bool> canJump(bool isJumpEnabled)
    {
        await Task.Delay(5);
        return Physics2D.BoxCast(transform.position, gameObject.GetComponent<Collider2D>().bounds.size, 0.0f, -Vector3.up, jumpCheckOffset, layerMaskForGrounding) && isJumpEnabled;
    }
}
