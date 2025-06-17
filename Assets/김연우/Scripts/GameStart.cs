
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public SpawnManagerConfig spawnConfig;
    public GameObject playerPrefab;

    private void Start()
    {
        SpawnManager spawnManager = new SpawnManager(spawnConfig);
        spawnManager.Init();

        for (int i = 0; i < spawnConfig.SpawnPositions.Count; i++)
        {
            Vector3 pos = spawnConfig.SpawnPositions[i];
            Instantiate(playerPrefab, pos, Quaternion.identity);
        }
    }
}
