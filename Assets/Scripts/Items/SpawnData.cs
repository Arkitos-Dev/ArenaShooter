using UnityEngine;

[System.Serializable] // This attribute allows the class to show up in the Unity inspector.
public class SpawnData
{
    public Transform spawnPoint;
    public GameObject itemPrefab;
}
