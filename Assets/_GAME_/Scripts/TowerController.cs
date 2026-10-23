
using UnityEngine;

public class TowerController : MonoBehaviour
{
    public Transform turret;
    public Transform firePoint;
    public Projectile projectilePrefab;
    public LayerMask zombieLayers;

    public float range = 5f;
    public float fireRate = 1f;
    public float turnSpeed = 180f;

    private float nextShot;

    private void Update()
    {
        if (turret == null || firePoint == null || projectilePrefab == null || fireRate <= 0f) return;

        Zombie target = FindTarget();
        if (target == null) return;

        Vector2 direction = target.transform.position - turret.position;
        if (direction.sqrMagnitude < 0.0001f) return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle - 95f);

        turret.rotation = Quaternion.RotateTowards(turret.rotation, rotation, turnSpeed * Time.deltaTime);

        if (Quaternion.Angle(turret.rotation, rotation) > 5f || Time.time < nextShot) return;

        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        projectile.Fire(firePoint.up);

        nextShot = Time.time + 1f / fireRate;
    }

    private Zombie FindTarget()
    {
        Zombie closest = null;
        float distance = range * range;

        foreach (Collider2D hit in Physics2D.OverlapCircleAll(transform.position, range, zombieLayers))
        {
            Zombie zombie = hit.GetComponentInParent<Zombie>();
            if (zombie == null) continue;

            float current = ((Vector2)(zombie.transform.position - transform.position)).sqrMagnitude;

            if (current < distance)
            {
                distance = current;
                closest = zombie;
            }
        }

        return closest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
