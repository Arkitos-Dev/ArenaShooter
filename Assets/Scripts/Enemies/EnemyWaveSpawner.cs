using System.Collections;
using UnityEngine;
using TMPro;

public class EnemyWaveSpawner : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public int initialSpawnCount = 3;
    public float timeBetweenWaves = 5f;

    [Header("UI Elements")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI enemiesLeftText;
    public TextMeshProUGUI totalEnemiesText;
    public TextMeshProUGUI newRoundNumber;
    public GameObject newRoundMessage;
    public GameObject Interface;
    public ItemSpawner itemSpawner;

    private int _currentWave = 0; // Starts from 0, representing the first wave
    private float waveCountdown;
    private bool isSpawning = false;
    private int activeEnemies = 0;
    private int totalEnemiesThisRound = 0;

    private Coroutine waveCoroutine;

    void Start()
    {
        waveCountdown = timeBetweenWaves;
        UpdateUI();
        newRoundMessage.SetActive(false);
    }

    void Update()
    {
        if (ShouldStartNewWave())
        {
            if (waveCoroutine == null)
            {
                waveCoroutine = StartCoroutine(PrepareNewRound());
            }
        }
        else if (!isSpawning)
        {
            waveCountdown -= Time.deltaTime;
        }
    }

    private bool ShouldStartNewWave()
    {
        return waveCountdown <= 0 && !isSpawning && activeEnemies == 0;
    }

    IEnumerator PrepareNewRound()
    {
        Interface.SetActive(false);
        newRoundNumber.text = (_currentWave + 1).ToString();
        newRoundMessage.SetActive(true);
        SlowDownTime();
        yield return new WaitForSecondsRealtime(3);
        RestoreTime();
        newRoundMessage.SetActive(false);
        Interface.SetActive(true);
        _currentWave++;
        itemSpawner.RespawnItems();
        waveCoroutine = StartCoroutine(SpawnWave(initialSpawnCount + _currentWave - 1));
    }


    private void SlowDownTime()
    {
        Time.timeScale = 0.5f;
    }

    private void RestoreTime()
    {
        Time.timeScale = 1.0f;
    }

    IEnumerator SpawnWave(int numberOfEnemies)
    {
        isSpawning = true;
        int enemiesToSpawn = Mathf.CeilToInt(numberOfEnemies * 1.5f);
        totalEnemiesThisRound = enemiesToSpawn;
        UpdateUI();

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
            if (i < enemiesToSpawn - 1)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        isSpawning = false;
        waveCoroutine = null;
        waveCountdown = timeBetweenWaves;
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject selectedEnemy = SelectEnemyBasedOnProbability();
            GameObject enemyInstance = Instantiate(selectedEnemy, spawnPoint.position, spawnPoint.rotation);
            BaseEnemy enemyScript = enemyInstance.GetComponent<BaseEnemy>();
            if (enemyScript != null)
            {
                enemyScript.ScaleStats(_currentWave);
                enemyScript.OnDeath += HandleEnemyDeath;
                activeEnemies++;
            }
            else
            {
                Debug.LogError("BaseEnemy script not found on spawned enemy prefab!");
            }
            UpdateUI();
        }
    }

    private void HandleEnemyDeath()
    {
        activeEnemies--;
        UpdateUI();
        CheckEndOfWave();
    }

    private void CheckEndOfWave()
    {
        if (activeEnemies <= 0 && !isSpawning)
        {
            waveCountdown = timeBetweenWaves;
        }
    }

    private GameObject SelectEnemyBasedOnProbability()
    {
        float chance = Random.Range(0f, 1f);
        return chance < 0.7f ? enemyPrefabs[0] : enemyPrefabs[1];
    }

    private void UpdateUI()
    {
        roundText.text = _currentWave.ToString();
        enemiesLeftText.text = $"Alive: {activeEnemies}";
        totalEnemiesText.text = $"Total: {totalEnemiesThisRound}";
    }
}
