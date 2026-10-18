
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public PathController path;
    public float speed = 2f;
    public bool canMove = true;

    private int waypointIndex;
    private float offset;
    private bool initialized;

    private void Start()
    {
        if (!initialized && path != null) Initialize(path);
    }

    public void Initialize(PathController newPath)
    {
        if (newPath == null || newPath.waypoints == null || newPath.waypoints.Length < 2)
        {
            enabled = false;
            return;
        }

        path = newPath;
       
        offset = Random.Range(-path.offset / 2f, path.offset / 2f);

        transform.position = path.GetOffsetPoint(0, offset);
        initialized = true;
        enabled = true;
    }

    private void Update()
    {
        if (!initialized || !canMove || waypointIndex >= path.waypoints.Length) return;

        Vector3 target = path.GetOffsetPoint(waypointIndex, offset);
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f) waypointIndex++;
    }
}
