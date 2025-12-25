using System.Collections;
using System.Collections.Generic;
using TrashSucker.Models;
using TrashSucker.Singleton;
using UnityEngine;

namespace TrashSucker.Presenters
{
    public class EnemySpawner: SpawnerBase
    {
        [SerializeField]
        private int _maxEnemiesAmount;
        [SerializeField]
        private float _spawnDelay;

        private bool _isSpawning = false;

        private void Awake()
        {
            _maxEnemiesAmount = SpawnCount;
            Singleton<GameManager>.Instance.Enemies = new List<GameObject>(_maxEnemiesAmount);
            EnemyBasePresenter[] objs = GameObject.FindObjectsByType<EnemyBasePresenter>(FindObjectsSortMode.None);
            foreach (EnemyBasePresenter obj in objs)
            {
                Singleton<GameManager>.Instance.Enemies.Add(obj.gameObject);
            }
        }

        private void Start()
        {
            if (Transform == null)
                Transform = this.transform;

            for (int i = 0; i < SpawnCount; i++)
            {
                SpawnEnemy();
            }
        }

        private void Update()
        {
            if (!_isSpawning && Singleton<GameManager>.Instance.Enemies.Count < _maxEnemiesAmount)
            {
                _isSpawning = true;
                StartCoroutine(SpawnEnemiesOverTime());
            }
        }

        private IEnumerator SpawnEnemiesOverTime()
        {
            while(Singleton<GameManager>.Instance.Enemies.Count < SpawnCount)
            {
                yield return new WaitForSeconds(_spawnDelay);
                SpawnEnemy();
                Debug.Log("Enemy spawned");
            }
            _isSpawning = false;
        }

        private void SpawnEnemy()
        {
            Vector3 randomPoint = FindValidSpawnPoint();
            if (randomPoint.y <= 0)
                Debug.Log(randomPoint.y);
            GameObject itemToSpawn = Instantiate(SpawnItems[Random.Range(0, SpawnItems.Length)], Transform);
            itemToSpawn.transform.position = randomPoint + SpawnOffset;
            Singleton<GameManager>.Instance.Enemies.Add(itemToSpawn);
        }
    }
}
