
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] public PathController path;
    [SerializeField] public float speed = 2f;
    [SerializeField] public bool canMove = true;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float health = 50f;
    [SerializeField] private float radius = 0.35f;

    private static readonly List<Zombie> zombies = new List<Zombie>();

    private int waypointIndex;
    private float offset;
    private bool initialized;
    private Vector2 nextPosition;

    [SerializeField] private GameObject zombieDeathEffectPrefab;

    [SerializeField] private SpriteRenderer zombieSpriteRenderer;
    [SerializeField] private Color bottomLeftColor = Color.green;
    [SerializeField] private Color bottomRightColor = Color.green;
    [SerializeField] private Color topLeftColor = Color.green;
    [SerializeField] private Color topRightColor = Color.green;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.linearVelocity = Vector2.zero;
    }

    private void Start()
    {
        if (path == null)
            path = FindAnyObjectByType<PathController>();

        if (!initialized && path != null)
            Initialize(path);
    }

    public static Color RandomColor(Color bottomLeft, Color bottomRight, Color topLeft, Color topRight)
    {
        float x = UnityEngine.Random.value;
        float y = UnityEngine.Random.value;

        Color bottom = Color.Lerp(bottomLeft, bottomRight, x);
        Color top = Color.Lerp(topLeft, topRight, x);

        return Color.Lerp(bottom, top, y);
    }

    public void Initialize(PathController newPath)
    {
        if (newPath == null || newPath.waypoints == null || newPath.waypoints.Length < 2)
        {
            enabled = false;
            return;
        }

        zombieSpriteRenderer.color = RandomColor(bottomLeftColor, bottomRightColor, topLeftColor, topRightColor);

        path = newPath;
        waypointIndex = 1;

        float maxOffset = Mathf.Max(0f, path.offset / 2f - radius);

        for (int i = 0; i < 20; i++)
        {
            offset = Random.Range(-maxOffset, maxOffset);
            Vector3 position = path.GetOffsetPoint(0, offset);

            if (IsBlocked(position)) continue;

            transform.position = position;
            rb.position = position;
            nextPosition = position;

            initialized = true;
            enabled = true;
            zombies.Add(this);
            return;
        }
        
        Destroy(gameObject);
        
    }

    public void TakeDamage(float damage)
    {
        if (health <= 0f || damage <= 0f) return;

        health -= damage;

        if (health <= 0f)
        {
            var deathEffect = Instantiate(zombieDeathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(deathEffect, 1f);
            Destroy(gameObject);
        }
    }

    private bool IsBlocked(Vector2 position)
    {
        foreach (Zombie zombie in zombies)
        {
            if (zombie == null || zombie == this) continue;

            float distance = radius + zombie.radius;

            if ((zombie.nextPosition - position).sqrMagnitude < distance * distance)
                return true;
        }

        return false;
    }

    private void FixedUpdate()
    {
        if (!initialized || !canMove || waypointIndex >= path.waypoints.Length)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 target = path.GetOffsetPoint(waypointIndex, offset);

        if (Vector2.Distance(rb.position, target) < 0.05f)
        {
            waypointIndex++;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 next = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);

        if (IsBlocked(next))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        nextPosition = next;
        rb.MovePosition(next);
    }

    private void OnDestroy()
    {
        zombies.Remove(this);
    }
}
