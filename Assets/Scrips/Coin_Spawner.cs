using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Coin_Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs;
    public float obstacleSpawnTime = 3f;
    public float obstacleSpeed = 1.5f;
    public float obstacleSpeedModifier = 3f;
    public float minY = -3f; // Minimum y position
    public float maxY = 3f;  // Maximum y position
    PlayerCollision pc;

    private float timeUntilObstacleSpawn;
    private void Start()
    {
        pc = PlayerCollision.Instance;
        pc.OnFirePickup += FireModifier;
    }
    private void Update(){
        if(GameManager.Instance.isPlaying){
            SpawnLoop();
        }
    }

    private void SpawnLoop(){
        timeUntilObstacleSpawn += Time.deltaTime;

        if (timeUntilObstacleSpawn >= obstacleSpawnTime) {
            Spawn();
            timeUntilObstacleSpawn = 0f;
        }
    }

    private void Spawn() {
        GameObject obstacleToSpawn = obstaclePrefabs[Random.Range(0,obstaclePrefabs.Length)];

        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(transform.position.x, randomY, transform.position.z);

        GameObject spawnedObstacle = Instantiate(obstacleToSpawn, spawnPos, Quaternion.identity);

        Rigidbody2D obstacleRB2d = spawnedObstacle.GetComponent<Rigidbody2D>();
        obstacleRB2d. velocity = Vector2.left * obstacleSpeed;
        StartCoroutine(DestroyObstacle(spawnedObstacle));

        StartCoroutine(DestroyObstacle(spawnedObstacle));
    }


    private IEnumerator DestroyObstacle(GameObject spawnedObstacle) {
        yield return new WaitForSeconds(15);
        Destroy(spawnedObstacle);
    }

    private void FireModifier() {
        StartCoroutine(FireModifierCoroutine());
    }

    private IEnumerator FireModifierCoroutine() {
        obstacleSpeed = obstacleSpeed * obstacleSpeedModifier;
        obstacleSpawnTime = obstacleSpawnTime / obstacleSpeedModifier;
        yield return new WaitForSeconds(5);
        obstacleSpeed = obstacleSpeed / obstacleSpeedModifier;
        obstacleSpawnTime = obstacleSpawnTime * obstacleSpeedModifier;
    }
    
}
