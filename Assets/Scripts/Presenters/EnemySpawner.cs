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
        private float _maxSpawnDelay;
        [SerializeField]
        private float _minSpawnDelay;

        private bool _isSpawning = false;

        private int _currentEnemyCount => Singleton<GameManager>.Instance.Enemies.Count;

        private void Awake()
        {
            if(_maxEnemiesAmount == 0)
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
            if (!_isSpawning && _currentEnemyCount < _maxEnemiesAmount)
            {
                _isSpawning = true;
                StartCoroutine(SpawnEnemiesOverTime());
            }
        }

        private IEnumerator SpawnEnemiesOverTime()
        {
            while(_currentEnemyCount < _maxEnemiesAmount)
            {
                float delay = GetSpawnDelay();
                yield return new WaitForSeconds(delay);
                SpawnEnemy();
                Debug.Log("Enemy spawned");
            }
            _isSpawning = false;
        }

        private float GetSpawnDelay()
        {
            float ratio = Mathf.Clamp01((float)_currentEnemyCount / _maxEnemiesAmount);
            float delay = Mathf.Lerp(_minSpawnDelay, _maxSpawnDelay, ratio);

            return Mathf.Clamp(delay, _minSpawnDelay, Mathf.Max(_minSpawnDelay, _maxSpawnDelay));
        }

        private void SpawnEnemy()
        {
            Vector3 randomPoint = FindValidSpawnPoint();
            if (randomPoint.y <= 0)
                Debug.Log(randomPoint.y);
            GameObject itemToSpawn = Instantiate(SpawnItems[Random.Range(0, SpawnItems.Length)], Transform);
            itemToSpawn.transform.position = randomPoint + SpawnOffset;
            Singleton<GameManager>.Instance.Enemies.Add(itemToSpawn);
            itemToSpawn.GetComponent<EnemyBasePresenter>().NavMeshAgent.enabled = true;
        }
    }
}
