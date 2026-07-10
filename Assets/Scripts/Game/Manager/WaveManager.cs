using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [SerializeField, Tooltip("Spawner 활성화 시 효과음")]
    private AudioClip spanwerActivateClip;

    [SerializeField]
    private EnemySpawner enemySpanwer_East;
    [SerializeField]
    private EnemySpawner enemySpanwer_West;
    [SerializeField]
    private EnemySpawner enemySpanwer_South;
    [SerializeField]
    private EnemySpawner enemySpanwer_North;

    [Header("일반 웨이브 설정")]
    [SerializeField, Tooltip("wave 순서대로 적용될 WaveData (리스트의 index = wave 번호)")]
    private List<WaveData> waveDataList;

    [SerializeField, Tooltip("카메라 이동이 완료되기까지 걸리는 시간(초)")]
    private float cameraMoveDuration = 1.5f;

    [SerializeField, Tooltip("카메라 이동이 끝난 후 Spawner가 활성화되기까지 걸리는 시간(초)")]
    private float activateSpawnerDelayAfterCameraMove = 0.5f;

    [Header("무한 웨이브 자동 진행 설정")]
    [SerializeField, Tooltip("무한 웨이브 모드 진입 후 자동으로 다음 wave로 넘어가기까지 걸리는 시간(초)")]
    private float infiniteWaveAutoAdvanceInterval = 10f;

    private Coroutine _infiniteWaveCoroutine;
    private readonly List<EnemySpawner> _activeInfiniteSpawners = new List<EnemySpawner>();


    private int currentWaveIndex;
    private int _pendingSpawnerCount;

    private AudioSource _audioSource;

    public event Action TutorialWaveCleared;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _audioSource = GetComponent<AudioSource>();
    }

    public void OnClickStartWave()
    {
        if (currentWaveIndex >= waveDataList.Count)
        {
            List<EnemySpawner> infiniteSpawners = new();
            infiniteSpawners.Add(enemySpanwer_East);
            infiniteSpawners.Add(enemySpanwer_West);
            infiniteSpawners.Add(enemySpanwer_South);
            infiniteSpawners.Add(enemySpanwer_North);
            StartInfiniteAutoProgress(infiniteSpawners);
        }
        else
        {
            StartSpawnerActivate();
        }
    }

    public void StartSpawnerActivate()
    {
        var waveData = GetCurrentWaveData();
        if (waveData == null || waveData.spawnInfos == null || waveData.spawnInfos.Count == 0)
        {
            Debug.LogWarning($"[WaveManager] wave {currentWaveIndex} 에 대한 WaveData가 없습니다.");
            return;
        }

        var cameraTargetSpawner = GetSpawner(waveData.spawnInfos[0].spawnPoint);
        CameraManager.Instance.MoveTowerPlacementCameraTo(cameraTargetSpawner.spawnPoint.position, cameraMoveDuration, OnCameraMoveCompleted);
    }

    void OnCameraMoveCompleted()
    {
        StartCoroutine(ActivateSpawnerAfterCameraMove());
    }

    IEnumerator ActivateSpawnerAfterCameraMove()
    {
        yield return new WaitForSeconds(activateSpawnerDelayAfterCameraMove);

        ActivateSpawner();
        CameraManager.Instance.UnlockTowerPlacementCamera();
    }

    void ActivateSpawner()
    {
        var waveData = GetCurrentWaveData();
        if (waveData == null || waveData.spawnInfos == null || waveData.spawnInfos.Count == 0)
        {
            Debug.LogWarning($"[WaveManager] wave {currentWaveIndex} 에 대한 WaveData가 없습니다.");
            return;
        }

        if (spanwerActivateClip != null)
            _audioSource.PlayOneShot(spanwerActivateClip);

        _pendingSpawnerCount = 0;

        foreach (var spawnInfo in waveData.spawnInfos)
        {
            var spawner = GetSpawner(spawnInfo.spawnPoint);
            if (spawner == null)
                continue;

            spawner.WaveCleared += OnSpawnerWaveCleared;
            spawner.StartSpawn(spawnInfo);
            _pendingSpawnerCount++;
        }

        if (_pendingSpawnerCount == 0)
        {
            Debug.LogWarning($"[WaveManager] wave {currentWaveIndex} 에서 유효한 스포너를 찾지 못했습니다.");
        }
    }

    /// <summary>스포너가 이번 wave에 스폰한 적을 모두 스폰하고, 그 적들이 전부 죽었을 때 호출됨</summary>
    void OnSpawnerWaveCleared(EnemySpawner spawner)
    {
        spawner.WaveCleared -= OnSpawnerWaveCleared;

        _pendingSpawnerCount--;
        if (_pendingSpawnerCount <= 0)
        {
            ClearTutorialWave();
        }
    }

    // 마지막 적 죽으면 실행
    void ClearTutorialWave()
    {
        currentWaveIndex++;
        TutorialWaveCleared?.Invoke();
    }

    /// <summary>현재 wave 번호에 해당하는 WaveData를 반환</summary>
    private WaveData GetCurrentWaveData()
    {
        if (waveDataList == null || currentWaveIndex < 0 || currentWaveIndex >= waveDataList.Count)
            return null;

        return waveDataList[currentWaveIndex];
    }

    /// <summary>지정한 스폰 방향에 매칭된 씬의 EnemySpawner를 반환한다.</summary>
    private EnemySpawner GetSpawner(SpawnPoint direction)
    {
        switch (direction)
        {
            case SpawnPoint.East:
                return enemySpanwer_East;
            case SpawnPoint.West:
                return enemySpanwer_West;
            case SpawnPoint.South:
                return enemySpanwer_South;
            case SpawnPoint.North:
                return enemySpanwer_North;
        }
        return null;
    }


    /// <summary>
    /// 무한 웨이브 모드에 진입하면 호출된다. 이후부터는 플레이어가 WaveStartButton을 클릭하지 않아도
    /// infiniteWaveAutoAdvanceInterval 마다 자동으로 wave 번호를 올리고, 해당 스포너의 난이도(체력/스폰 간격)를 갱신한다.
    /// </summary>
    void StartInfiniteAutoProgress(List<EnemySpawner> spawners)
    {
        if (_infiniteWaveCoroutine != null)
            StopCoroutine(_infiniteWaveCoroutine);

        _activeInfiniteSpawners.Clear();
        _activeInfiniteSpawners.AddRange(spawners);

        // 무한 웨이브 진입 즉시 각 스포너의 스폰을 시작한다
        foreach (var spawner in _activeInfiniteSpawners)
        {
            spawner.StartWave(currentWaveIndex);
        }

        _infiniteWaveCoroutine = StartCoroutine(InfiniteWaveAutoProgressRoutine());
    }

    IEnumerator InfiniteWaveAutoProgressRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(infiniteWaveAutoAdvanceInterval);

            currentWaveIndex++;
            foreach (var spawner in _activeInfiniteSpawners)
            {
                spawner.AdvanceInfiniteWave(currentWaveIndex);
            }
        }
    }

    private void OnDestroy()
    {
        if (_infiniteWaveCoroutine != null)
            StopCoroutine(_infiniteWaveCoroutine);
    }


}
