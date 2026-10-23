
using System.Collections;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] public GameObject zombiePrefab;
    [SerializeField] public PathController path;

    [SerializeField] public float spawnInterval = 1f;
    [SerializeField] public int numberOfSpawners = 1;

    private bool isSpawning;

    private void Update()
    {
        if (!isSpawning) SpawnWave();
    }

    public void SpawnWave()
    {
        if (isSpawning || zombiePrefab == null || path == null || path.waypoints == null || path.waypoints.Length < 2) return;
        if (!zombiePrefab.TryGetComponent<Zombie>(out _)) return;
        for(int i = 0; i < numberOfSpawners; i++)
        {
            StartCoroutine(Spawn());
        }
        
    }

    private IEnumerator Spawn()
    {
        isSpawning = true;

        GameObject zombie = Instantiate(zombiePrefab, path.waypoints[0].position, Quaternion.identity);
        zombie.GetComponent<Zombie>().Initialize(path);

        yield return new WaitForSeconds(Mathf.Max(0.01f, spawnInterval));

        isSpawning = false;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isSpawning = false;
    }
}
