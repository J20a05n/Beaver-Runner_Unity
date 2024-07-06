using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud_Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs;
    public float obstacleSpawnTime = 3f;
    public float obstacleSpeed = 1.5f;
    public float minY = -3f; // Minimum y position
    public float maxY = 3f;  // Maximum y position

    private float timeUntilObstacleSpawn;
    bool isOnFire;
    PlayerCollision pc;

    private void Start()
    {
        pc = PlayerCollision.Instance;
    }
    private void Update(){
        isOnFire  = pc.IsOnFire();
        if(GameManager.Instance.isPlaying && !isOnFire){
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

        if (Random.value > 0.5f) {
            Vector3 scale = spawnedObstacle.transform.localScale;
            scale.x = -1; // Mirror on the y-axis
            spawnedObstacle.transform.localScale = scale;
        }

        StartCoroutine(MoveObstacle(spawnedObstacle));
        StartCoroutine(DestroyObstacle(spawnedObstacle));
    }

    private IEnumerator MoveObstacle(GameObject obstacle) {
        while (obstacle != null) {
            obstacle.transform.Translate(Vector3.left * obstacleSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator DestroyObstacle(GameObject spawnedObstacle) {
        yield return new WaitForSeconds(15);
        Destroy(spawnedObstacle);
    }
    
}
