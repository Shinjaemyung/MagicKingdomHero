using Cinemachine;
using Core.Health;
using Core.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    /// <summary>특정 wave에서 이 스포너가 스폰할 적 종류와 수량</summary>
    [System.Serializable]
    public class WaveSpawnData
    {
        [Tooltip("이 wave에서 스폰할 Enemy 프리팹")]
        public GameObject enemyPrefab;

        [Tooltip("이 wave에서 스폰할 개수")]
        public int spawnCount;
    }

    [SerializeField, Tooltip("스폰 위치")]
    public Transform spawnPoint;

    [SerializeField, Tooltip("순서대로 이동할 웨이포인트 배열")]
    private Transform[] waypoints;

    [SerializeField, Tooltip("스폰 간격 (초)")]
    private float spawnInterval = 5f;

    [SerializeField, Tooltip("wave 순서대로 적용될 스폰 데이터 (리스트의 index = wave 번호)")]
    private List<WaveSpawnData> waveSpawnDataList;

    private float _timer;
    private int _remainingSpawnCount;
    private GameObject _currentEnemyPrefab;
    private bool _isSpawning;

    CinemachineImpulseSource _impulseSource;

    /// <summary>이 스포너가 이번 wave에서 스폰한 적이 전부 죽었을 때 발생</summary>
    public event Action<EnemySpawner> WaveCleared;

    private int _aliveCount;

    private void Update()
    {
        if (!_isSpawning)
            return;

        _timer += Time.deltaTime;
        if (_timer >= spawnInterval)
        {
            _timer = 0f;
            SpawnEnemy();

            _remainingSpawnCount--;
            if (_remainingSpawnCount <= 0)
            {
                _isSpawning = false;
            }
        }
    }

    /// <summary>
    /// 지정한 waveIndex의 스폰 데이터(적 종류, 수량)를 기준으로 스포너를 활성화
    /// 활성화 즉시 첫 번째 적이 스폰되고, 이후 spawnInterval마다 스폰되며,
    /// 지정된 수량을 모두 스폰하면 자동으로 멈춘다.
    /// </summary>
    /// <param name="waveIndex">waveSpawnDataList에서 사용할 wave 번호</param>
    public void StartWave(int waveIndex)
    {
        if (waveSpawnDataList == null || waveIndex < 0 || waveIndex >= waveSpawnDataList.Count)
        {
            Debug.LogWarning($"[EnemySpawner] {name} 에 wave {waveIndex}에 대한 스폰 데이터가 없습니다.");
            return;
        }

        var data = waveSpawnDataList[waveIndex];
        _currentEnemyPrefab = data.enemyPrefab;
        _remainingSpawnCount = data.spawnCount;
        _aliveCount = 0;
        _timer = spawnInterval; // 활성화 즉시 첫 적을 스폰
        _isSpawning = true;

        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _impulseSource.GenerateImpulseWithForce(0.5f);

        gameObject.SetActive(true);
    }

    private void SpawnEnemy()
    {
        if (_currentEnemyPrefab == null || spawnPoint == null || waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("[EnemySpawner] enemyPrefab / spawnPoint / waypoints 를 Inspector에서 연결해주세요.");
            return;
        }

        var enemyObject = PoolManager.Instance.GetObject(_currentEnemyPrefab);

        var poolable = enemyObject.GetComponent<EnemyPoolable>();
        if (poolable != null)
            poolable.Init(_currentEnemyPrefab);

        var enemy = enemyObject.GetComponent<Enemy>();
        enemy.Died += GamePlayManager.Instance.OnEnemyDied;
        enemy.Removed += OnSpawnedEnemyDied;

        _aliveCount++;

        var mover = enemyObject.GetComponent<EnemyMover>();
        if (mover != null)
        {
            mover.SetWaypoints(waypoints);
            mover.ActivateAt(spawnPoint.position);
        }
    }

    private void OnSpawnedEnemyDied(DamageableBehaviour damageable)
    {
        damageable.Removed -= OnSpawnedEnemyDied;

        _aliveCount--;
        if (_aliveCount < 0)
            _aliveCount = 0;

        CheckWaveCleared();
    }

    /// <summary>이번 wave 분량 스폰이 다 끝났고, 스폰한 적도 전부 죽었으면 WaveCleared 발생</summary>
    private void CheckWaveCleared()
    {
        if (!_isSpawning && _aliveCount <= 0)
        {
            WaveCleared?.Invoke(this);
        }
    }
}
