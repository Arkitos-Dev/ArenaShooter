using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField]
    private SpawnData[] spawns;

    void Start()
    {
        SpawnItems(); // Initial spawn
    }

    public void SpawnItems()
    {
        foreach (SpawnData spawn in spawns)
        {
            Instantiate(spawn.itemPrefab, spawn.spawnPoint.position, Quaternion.identity, spawn.spawnPoint);
        }
    }

    public void ClearItems()
    {
        foreach (SpawnData spawn in spawns)
        {
            // Check if there's an item already at the spawn point and clear it
            if (spawn.spawnPoint.childCount > 0)
            {
                Destroy(spawn.spawnPoint.GetChild(0).gameObject);
            }
        }
    }

    // This method can be called by the game manager at the start of each round
    public void RespawnItems()
    {
        ClearItems();
        SpawnItems();
    }
}