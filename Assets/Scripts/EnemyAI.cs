
using UnityEngine;
using Pathfinding;
using System.Threading.Tasks;
using System.Threading;

public class EnemyAI : MonoBehaviour
{
    public const float FORCEUPPERLIMIT = 400f;

    [Header("Pathfinding Variables")]
    public float updatePathSeconds;
    public float farDistance;
    public float closeDistance;
    public float nextWayPointDistance; //tells you how much to move until the next waypoint
    public float jumpCheckOffset;

    [Header("Custom Behavior")]
    public bool isFollowEnabled;
    public bool isJumpEnabled;
    public LayerMask layerMaskForGrounding;
    public float forceMagnitude;
    public float jumpHeight;
    public float jumpPower;

    [Header("Targets")]
    public bool multipleTargets;

    [HideInInspector]
    public Transform[] WayPoints;
    [HideInInspector]
    public Transform target;

    private Seeker _seeker;
    private Rigidbody2D _rb;
    private Path _path;
    private int _currentIndex = 0;
    private int _sign;
    private int _currentWayPointIndex = 0;
    private Transform _selectedTargetToMoveToward;
    private bool isJumping = false;
    private Collider2D _collider;
    private Vector2 _boundsValue;
    private CancellationToken _cancellationToken;
    private CancellationTokenSource _tokenSource;

    void Start()
    {
        _seeker = GetComponent<Seeker>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        InvokeRepeating("UpdatePath", 0f, updatePathSeconds);
        _tokenSource = new CancellationTokenSource();
        _cancellationToken = _tokenSource.Token;
    }

    private async void FixedUpdate()
    {
        if (multipleTargets ? await IsInVisibleDistanceMultipleTargets(_cancellationToken) : await IsInVisibleDistanceSingleTarget(_cancellationToken) && isFollowEnabled)
        {
            PathToFollow();
        }
    }

    private async void UpdatePath()
    {
        _boundsValue = _collider.bounds.center;
        if (multipleTargets ? await IsInVisibleDistanceMultipleTargets(_cancellationToken) : await IsInVisibleDistanceSingleTarget(_cancellationToken) &&
        isFollowEnabled && _seeker.IsDone()) //if one path is finished
        {
            _seeker.StartPath(_rb.position, _selectedTargetToMoveToward.position, OnPathComplete);
        }

    }

    private async Task<bool> IsInVisibleDistanceMultipleTargets(CancellationToken _token) //find the closest first
    {
        bool inDistance = false;

        if (_currentIndex >= WayPoints.Length - 1)
        {
            _sign = -1;
        }
        if (_currentIndex <= 0)
        {
            _sign = 1;
        }
        await Task.Delay(5);

        if (!_token.IsCancellationRequested)
        {
            if (Vector2.Distance(transform.position, WayPoints[_currentIndex].position) < farDistance && Vector2.Distance(transform.position, WayPoints[_currentIndex].position) > closeDistance)
            {
                _selectedTargetToMoveToward = WayPoints[_currentIndex].transform;
                inDistance = true;

            }

            if (Vector2.Distance(transform.position, WayPoints[_currentIndex].position) < closeDistance)
            {
                _currentIndex = _currentIndex + _sign;
            }
        }

        return inDistance;

    }

    private async Task<bool> IsInVisibleDistanceSingleTarget(CancellationToken _token) //find the closest first
    {

        bool inDistance = false;

        if (!_token.IsCancellationRequested)
        {
            inDistance = Vector2.Distance(transform.position, target.position) < farDistance;
            _selectedTargetToMoveToward = target;

            await Task.Delay(100);

        }
        return inDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;
            _currentWayPointIndex = 0;//resets (this index is for all the waypoints between the wayPoints i have specified)
        }
    }

    private async void PathToFollow()
    {
        if (_path == null)
        {
            return; //there's an error -> exit (nothing to follow)
        }

        if (_currentWayPointIndex >= _path.vectorPath.Count)
        {
            return; //crossed all waypoints so far
        }


        Vector3 direction = ((Vector2)_path.vectorPath[_currentWayPointIndex] - _rb.position).normalized;  //the waypoint index in the path selected for that true value
        Vector3 force = direction * forceMagnitude * Time.deltaTime * _rb.mass;

        if (await canJump(isJumpEnabled))
        {
            if (direction.y > jumpHeight && !isJumping) //if the direction of y is above, then jump
            {
                isJumping = true;
                var value = Vector2.up * forceMagnitude * Time.deltaTime * jumpPower;
                if (value.y < FORCEUPPERLIMIT)
                    _rb.AddForce(value * _rb.mass, ForceMode2D.Impulse);
            }
            isJumping = false;

        }

        _rb.AddForce(force, ForceMode2D.Force);

        if (_currentWayPointIndex < _path.vectorPath.Count)
        {
            float distance = Vector3.Distance(_rb.position, _path.vectorPath[_currentWayPointIndex]);

            if (distance < nextWayPointDistance)
            {
                _currentWayPointIndex++; //move to next path (current waypoint has been reached)
            }

        }
        Vector3 rotation = await flipCharacter(0, 0, 0);

        if (_rb.linearVelocity.x > .05f)
        {
            transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        }

        if (_rb.linearVelocity.x < -.05f)
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
        Debug.DrawRay(_boundsValue, -Vector3.up * jumpCheckOffset, Color.red);
        return Physics2D.Raycast(_boundsValue, -Vector3.up, jumpCheckOffset, layerMaskForGrounding) && isJumpEnabled;
    }

    private void OnDisable()
    {
        _tokenSource.Cancel();
    }
}
