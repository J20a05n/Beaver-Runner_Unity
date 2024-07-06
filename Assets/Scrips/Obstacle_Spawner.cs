using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs;
    public float obstacleSpawnTime = 2f;
    [Range(0, 1)] public float obstacleSpawnTimeFactor = 0.5f;
    public float obstacleSpeed = 5f;
    [Range(0, 1)] public float obstacleSpeedFactor = 0.2f;
    public float _obstacleSpawnTime;
    public float _obstacleSpeed;

    private float timeUntilObstacleSpawn;
    private float timeAlive;
    GameManager gm;
    PlayerCollision pc;

    private void Update(){

        CalculateSpawnFactor();
        gm = GameManager.Instance;
        pc = PlayerCollision.Instance;
        timeAlive = gm.TimeAlive();
        if(GameManager.Instance.isPlaying) {
            SpawnLoop();
        }
    }

    private void CalculateSpawnFactor() {
        _obstacleSpawnTime = obstacleSpawnTime * Mathf.Pow(timeAlive, -obstacleSpawnTimeFactor);
        _obstacleSpeed = obstacleSpeed * Mathf.Pow(timeAlive, obstacleSpeedFactor);
    }

    private void SpawnLoop(){
        timeUntilObstacleSpawn += Time.deltaTime;

        if (timeUntilObstacleSpawn >= _obstacleSpawnTime) {
            Spawn();
            timeUntilObstacleSpawn = 0f;
        }
    }

    private void Spawn() {
        GameObject obstacleToSpawn = obstaclePrefabs[Random.Range(0,obstaclePrefabs.Length)];

        GameObject spawnedObstacle = Instantiate(obstacleToSpawn, transform.position, Quaternion.identity);

        Rigidbody2D obstacleRB2d = spawnedObstacle.GetComponent<Rigidbody2D>();
        obstacleRB2d. velocity = Vector2.left * _obstacleSpeed;
        StartCoroutine(DestroyObstacle(spawnedObstacle));
    }

    private IEnumerator DestroyObstacle(GameObject spawnedObstacle) {
        yield return new WaitForSeconds(5);
        Destroy(spawnedObstacle);
    }
    
}
