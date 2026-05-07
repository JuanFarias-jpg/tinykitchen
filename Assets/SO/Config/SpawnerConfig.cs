using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerConfig", menuName = "Game/Spawner Config")]
public class SpawnerConfig : ScriptableObject
{
    public GameObject enemyPrefab;
    public int maxEnemies = 5;
    public float spawnInterval = 2f;
}