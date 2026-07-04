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

    [SerializeField, Tooltip("순서대로 활성화 될 Spanwer 리스트")]
    private List<EnemySpawner> enemySpanwers;

    [SerializeField, Tooltip("카메라 이동이 끝난 후 Spawner가 활성화되기까지 걸리는 시간(초)")]
    private float activateSpawnerDelayAfterCameraMove = 0.5f;

    [Header("무한 웨이브 자동 진행 설정")]
    [SerializeField, Tooltip("무한 웨이브 모드 진입 후 자동으로 다음 wave로 넘어가기까지 걸리는 시간(초)")]
    private float infiniteWaveAutoAdvanceInterval = 10f;

    private Coroutine _infiniteWaveCoroutine;
    private readonly List<EnemySpawner> _activeInfiniteSpawners = new List<EnemySpawner>();


    private int currentWaveIndex;
    private int activatedSpawnerNum;

    private AudioSource _audioSource;

    public event Action TutorialWaveCleared;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _audioSource = GetComponent<AudioSource>();
    }

public void OnClickWaveStart()
    {
        if (currentWaveIndex >= enemySpanwers.Count) 
        {
            List<EnemySpawner> infiniteSpawners = new List<EnemySpawner>();
            foreach (EnemySpawner spawner in enemySpanwers)
            {
                if (spawner.IsInfiniteWaveIndex(currentWaveIndex))
                {
                    infiniteSpawners.Add(spawner);
                }
            }

            if (infiniteSpawners.Count > 0)
            {
                // 무한 웨이브 모드 시작 (대상 스포너 전체를 하나의 타이머로 함께 진행)
                StartInfiniteAutoProgress(infiniteSpawners);
            }
        }
        else
        {
            StartSpawnerActivate();
        }
    }

    public void StartSpawnerActivate()
    {
        var spawnPointPos = enemySpanwers[currentWaveIndex].spawnPoint.position;
        CameraManager.Instance.MoveTowerPlacementCameraTo(spawnPointPos, 1.5f, OnCameraMoveCompleted);
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
        if (enemySpanwers == null || enemySpanwers.Count == 0)
        {
            Debug.LogWarning("[WaveManager] enemySpanwers 가 비어있습니다.");
            return;
        }

        if (spanwerActivateClip != null)
            _audioSource.PlayOneShot(spanwerActivateClip);

        var spawner = enemySpanwers[activatedSpawnerNum];
        spawner.WaveCleared += OnSpawnerWaveCleared;
        spawner.StartWave(currentWaveIndex);
        activatedSpawnerNum++;
    }

    /// <summary>스포너가 이번 wave에 스폰한 적을 모두 스폰하고, 그 적들이 전부 죽었을 때 호출됨</summary>
    void OnSpawnerWaveCleared(EnemySpawner spawner)
    {
        spawner.WaveCleared -= OnSpawnerWaveCleared;
        ClearTutorialWave();
    }

    // 마지막 적 죽으면 실행
    void ClearTutorialWave()
    {
        currentWaveIndex++;
        TutorialWaveCleared?.Invoke();
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
