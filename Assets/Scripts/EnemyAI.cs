
using UnityEngine;
using Pathfinding;
using GlobalAccessAndGameHelper;
using System.Threading.Tasks;

public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform[] targets;
    public float activateDistance = 50f; //seeking distance
    public float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f; //next node height for the enemy to jump
    public float jumpModifier = 0.3f;//how powerful the jump is for the enemy
    public float jumpCheckOffset = 0.1f; //collider check

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;

    [Header("Layer Mask")]
    public LayerMask layerMask;

    private Path path;
    private int currentWaypoint = 0;
    private Transform _actualTarget;
    bool isGrounded = false;
    Seeker seeker;
    Rigidbody2D rb;


    public async void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if (await isOneTargetOnly(targets))
        {
            _actualTarget = targets[0];
        }

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds); //it's going to keep repeating the script like a coroutine
    }

    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }
    }

    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone()) //the object is found which we are seeking
        {
            seeker.StartPath(rb.position, _actualTarget.position, OnPathComplete);
        }
    }

    private async Task<bool> isOneTargetOnly(Transform[] targets)
    {
        return await Task.FromResult(targets.Length < 2);
    }

    private void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        //Reached the end of path =>All the possible paths in vectorPath
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        //see if we collide with anything

        isGrounded = Physics2D.Raycast(rb.position, -Vector3.up, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);  //keeping modifying other stuff too

        //learn more about the script and modify!!!! (AFTER LIGHTNING IS DONE!!)

        //Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized; //direction from enemy to the currentWayPoint. Normalizes gives the magnitude 
        Vector2 force = direction * speed * Time.deltaTime;

        //Jump

        if (jumpEnabled && isGrounded)
        {
            if (direction.y > jumpNodeHeightRequirement)
            {
                rb.AddForce(Vector2.up * speed * jumpModifier);
            }
        }

        //Movement
        rb.AddForce(force, ForceMode2D.Force);  //makes the AI enemy move toward the target continuously

        //Next WayPoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;  //makes sure that the object keeps following the path
        }

        //Direction Graphics Handling
        if (directionLookEnabled)
        {
            if (rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            }
        }

    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, _actualTarget.transform.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            Debug.Log(p);
            path = p;
            currentWaypoint = 0;
        }
    }
}
