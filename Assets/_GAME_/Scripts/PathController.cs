using UnityEngine;

[ExecuteInEditMode]
public class PathController : MonoBehaviour
{
    [SerializeField] public Transform[] waypoints;
    [SerializeField] public float offset = 0f;



    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2)
            return;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null)
                return;

            if (i > 0 && GetDirection(i - 1) == Vector3.zero)
                return;
        }

        float halfWidth = Mathf.Max(0f, offset) / 2f;

       
        Gizmos.color = Color.green;

        for (int i = 0; i < waypoints.Length; i++)
        {
            Gizmos.DrawSphere(waypoints[i].position, 0.5f);

            if (i < waypoints.Length - 1)
                Gizmos.DrawLine(
                    waypoints[i].position,
                    waypoints[i + 1].position);
        }

        
        Gizmos.color = Color.cyan;

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(GetOffsetPoint(i, halfWidth), GetOffsetPoint(i + 1, halfWidth));
            Gizmos.DrawLine(GetOffsetPoint(i, -halfWidth), GetOffsetPoint(i + 1, -halfWidth));
        }
    }

    public Vector3 GetOffsetPoint(int index, float offset)
    {
        Vector3 point = waypoints[index].position;

        if (index == 0)
            return point + GetSide(0) * offset;

        if (index == waypoints.Length - 1)
            return point + GetSide(index - 1) * offset;

        Vector3 previousSide = GetSide(index - 1);
        Vector3 nextSide = GetSide(index);

        Vector3 combined = previousSide + nextSide;

        if (combined.sqrMagnitude < 0.0001f)
            return point + nextSide * offset;

        Vector3 miter = combined.normalized;
        float dot = Vector3.Dot(miter, nextSide);

        if (Mathf.Abs(dot) < 0.001f)
            return point + nextSide * offset;

        return point + miter * (offset / dot);
    }

    private Vector3 GetDirection(int index)
    {
        Vector3 direction = waypoints[index + 1].position - waypoints[index].position;

        direction.y = 0f;
        return direction.normalized;
    }

    private Vector3 GetSide(int index)
    {
        Vector3 direction = GetDirection(index);
        return new Vector3(-direction.z, 0f, direction.x);
    }


}
