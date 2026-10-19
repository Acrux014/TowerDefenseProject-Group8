
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] public PathController path;
    [SerializeField] public float speed = 2f;
    [SerializeField] public bool canMove = true;
    [SerializeField] private Rigidbody2D rb;

    private int waypointIndex;
    private float offset;
    private bool initialized;

    private void Start()
    {
        if(path == null)
            path = FindAnyObjectByType<PathController>();

        if (!initialized && path != null) 
            Initialize(path);
    }

    public void Initialize(PathController newPath)
    {
        if (newPath == null || newPath.waypoints == null || newPath.waypoints.Length < 2)
        {
            enabled = false;
            return;
        }

        path = newPath;
        waypointIndex = 1;

        offset = Random.Range(-path.offset / 2f, path.offset / 2f);

        transform.position = path.GetOffsetPoint(0, offset);
        initialized = true;
        enabled = true;
    }

    private void Update()
    {
        /*if (!initialized || !canMove || waypointIndex >= path.waypoints.Length) return;

        Vector3 target = path.GetOffsetPoint(waypointIndex, offset);
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f) waypointIndex++;*/
    }

    private void FixedUpdate()
    {
        if (!initialized || !canMove || waypointIndex >= path.waypoints.Length)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            return;
        }

        Vector2 target = path.GetOffsetPoint(waypointIndex, offset);
        Vector2 direction = target - rb.position;

        if (direction.magnitude < 0.05f)
        {
            waypointIndex++;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = direction.normalized * Mathf.Min(speed, direction.magnitude / Time.fixedDeltaTime);
    }
}
