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

    [Header("무한 웨이브 설정")]
    [SerializeField, Tooltip("이 wave 번호(0-based)부터 무한 웨이브 모드로 전환됩니다. -1이면 비활성화.")]
    private int infiniteWaveStartIndex = -1;

    [SerializeField, Tooltip("무한 웨이브 모드에서 항상 소환할 Enemy 프리팹 종류")]
    private List<GameObject> infiniteEnemyPrefabs = new();

    [SerializeField, Tooltip("무한 웨이브 시작 시 적의 최대 체력")]
    private float infiniteBaseMaxHealth = 100f;

    [SerializeField, Tooltip("무한 웨이브에서 wave가 1 증가할 때마다 늘어나는 최대 체력")]
    private float infiniteHealthIncreasePerWave = 20f;

    [SerializeField, Tooltip("무한 웨이브 시작 wave(=infiniteWaveStartIndex)에서 스폰 간격(초)")]
    private float infiniteBaseSpawnInterval = 3f;

    [SerializeField, Tooltip("무한 웨이브에서 wave가 1 증가할 때마다 줄어드는 스폰 간격(초)")]
    private float infiniteSpawnIntervalDecreasePerWave = 0.2f;

    [SerializeField, Tooltip("무한 웨이브 스폰 간격이 줄어들 수 있는 최소값(초)")]
    private float infiniteMinSpawnInterval = 0.1f;

    [SerializeField, Tooltip("무한 웨이브에서 몇 마리를 소환할 때마다 enemy 종류를 새로 랜덤하게 바꿀지")]
    private int infiniteEnemyChangeInterval = 10;

    private float _timer;
    private int _remainingSpawnCount;
    private GameObject _currentEnemyPrefab;
    private int _spawnCountSinceEnemyChange;

    private bool _isSpawning;

    private bool _isInfiniteMode;
    private float _currentSpawnInterval;
    private float? _currentMaxHealthOverride;

    CinemachineImpulseSource _impulseSource;

    /// <summary>이 스포너가 이번 wave에서 스폰한 적이 전부 죽었을 때 발생</summary>
    public event Action<EnemySpawner> WaveCleared;

    private int _aliveCount;

    private void Update()
    {
        if (!_isSpawning)
            return;

        _timer += Time.deltaTime;
        if (_timer >= _currentSpawnInterval)
        {
            _timer = 0f;
            SpawnEnemy();

            if (!_isInfiniteMode)
            {
                _remainingSpawnCount--;
                if (_remainingSpawnCount <= 0)
                {
                    _isSpawning = false;
                }
            }
            else
            {
                // 무한 웨이브에서 n마리를 소환할 때마다 소환되는 enemy 종류를 랜덤하게 변경
                _spawnCountSinceEnemyChange++;
                if (infiniteEnemyChangeInterval > 0 && _spawnCountSinceEnemyChange >= infiniteEnemyChangeInterval)
                {
                    _spawnCountSinceEnemyChange = 0;
                    _currentEnemyPrefab = GetRandomInfiniteEnemyPrefab();
                }
            }
        }
    }

    /// <summary>
    /// 지정한 waveIndex의 스폰 데이터(적 종류, 수량)를 기준으로 스포너를 활성화
    /// 활성화 즉시 첫 번째 적이 스폰되고, 이후 spawnInterval마다 스폰되며,
    /// 지정된 수량을 모두 스폰하면 자동으로 멈춘다.
    /// waveIndex가 infiniteWaveStartIndex 이상이면 무한 웨이브 모드로 전환
    /// </summary>
    /// <param name="waveIndex">waveSpawnDataList에서 사용할 wave 번호</param>
    public void StartWave(int waveIndex)
    {
        if (IsInfiniteWaveIndex(waveIndex))
        {
            StartInfiniteWave(waveIndex);
            return;
        }

        var data = waveSpawnDataList[waveIndex];
        _isInfiniteMode = false;
        _currentEnemyPrefab = data.enemyPrefab;
        _remainingSpawnCount = data.spawnCount;
        _currentMaxHealthOverride = null;
        _currentSpawnInterval = spawnInterval;
        _aliveCount = 0;
        _timer = _currentSpawnInterval; // 활성화 즉시 첫 적을 스폰
        _isSpawning = true;

        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _impulseSource.GenerateImpulseWithForce(0.5f);

        gameObject.SetActive(true);
    }

    /// <summary>
    /// WaveManager로부터 직접 전달받은 스폰 정보(적 데이터, 수량, 간격)로 스포너를 활성화한다.
    /// 일반 웨이브 전용 진입점이며, waveSpawnDataList나 waveIndex를 사용하지 않는다.
    /// 무한 웨이브 로직(StartInfiniteWave/AdvanceInfiniteWave)과는 별개로 동작한다.
    /// </summary>
    public void StartSpawn(SpawnInfo spawnInfo)
    {
        var enemyData = spawnInfo.enemyData;
        if (enemyData == null || enemyData.enemyPrefab == null)
        {
            Debug.LogWarning($"[EnemySpawner] {name} 에 전달된 EnemyData 또는 enemyPrefab이 비어있습니다.");
            return;
        }

        _isInfiniteMode = false;
        _currentEnemyPrefab = enemyData.enemyPrefab;
        _remainingSpawnCount = spawnInfo.count;
        _currentMaxHealthOverride = null;
        _currentSpawnInterval = spawnInfo.interval;
        _aliveCount = 0;
        _timer = _currentSpawnInterval; // 활성화 즉시 첫 적을 스폰
        _isSpawning = true;

        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _impulseSource.GenerateImpulseWithForce(0.5f);

        gameObject.SetActive(true);
    }


    /// <summary>waveIndex가 무한 웨이브 모드에 해당하는지 여부</summary>
    public bool IsInfiniteWaveIndex(int waveIndex)
    {
        return infiniteWaveStartIndex >= 0 && waveIndex >= infiniteWaveStartIndex;
    }

    /// <summary>
    /// 무한 웨이브 모드를 시작한다. infiniteEnemyPrefab만 무제한으로 소환하며,
    /// infiniteWaveStartIndex로부터 몇 단계 지났는지에 따라 최대 체력은 올라가고 스폰 간격은 짧아진다.
    /// </summary>
    private void StartInfiniteWave(int waveIndex)
    {
        if (infiniteEnemyPrefabs == null || infiniteEnemyPrefabs.Count == 0)
        {
            Debug.LogWarning($"[EnemySpawner] {name} 에 무한 웨이브용 infiniteEnemyPrefab이 설정되어 있지 않습니다.");
            return;
        }

        _isInfiniteMode = true;
        _currentEnemyPrefab = GetRandomInfiniteEnemyPrefab();
        _spawnCountSinceEnemyChange = 0;
        _remainingSpawnCount = 0; // 무한 웨이브에서는 사용하지 않음
        _aliveCount = 0;
        ApplyInfiniteWaveDifficulty(waveIndex);
        _timer = _currentSpawnInterval; // 활성화 즉시 첫 적을 스폰
        _isSpawning = true;
    }

    /// <summary>무한 웨이브 모드에 사용될 Enemy 프리팹 랜덤 반환</summary>
    public GameObject GetRandomInfiniteEnemyPrefab()
    {
        if (infiniteEnemyPrefabs == null || infiniteEnemyPrefabs.Count == 0)
        {
            return null;
        }

        int index = UnityEngine.Random.Range(0, infiniteEnemyPrefabs.Count);
        return infiniteEnemyPrefabs[index];
    }

    /// <summary>
    /// 이미 무한 웨이브 모드로 스폰 중일 때 wave가 한 단계 더 진행되면 호출한다.
    /// 스폰을 멈추거나 재시작하지 않고, 다음 스폰부터 적용될 최대 체력/스폰 간격만 갱신한다.
    /// </summary>
    public void AdvanceInfiniteWave(int waveIndex)
    {
        if (!_isInfiniteMode)
        {
            StartInfiniteWave(waveIndex);
            return;
        }

        ApplyInfiniteWaveDifficulty(waveIndex);
    }

    /// <summary>infiniteWaveStartIndex로부터 몇 wave 지났는지를 기준으로 체력/스폰 간격을 계산해 적용</summary>
    private void ApplyInfiniteWaveDifficulty(int waveIndex)
    {
        int stepsIntoInfinite = Mathf.Max(0, waveIndex - infiniteWaveStartIndex);

        _currentMaxHealthOverride = infiniteBaseMaxHealth + infiniteHealthIncreasePerWave * stepsIntoInfinite;
        _currentSpawnInterval = Mathf.Max(
            infiniteMinSpawnInterval,
            infiniteBaseSpawnInterval - infiniteSpawnIntervalDecreasePerWave * stepsIntoInfinite);
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
        if (_currentMaxHealthOverride.HasValue)
            enemy.SetMaxHealth(_currentMaxHealthOverride.Value);

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
