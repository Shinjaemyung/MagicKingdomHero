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

    private int currentWaveIndex;
    private int activatedSpawnerNum;

    private AudioSource _audioSource;

    public event Action WaveCleared;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _audioSource = GetComponent<AudioSource>();
    }

    public void StartWave()
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

    /// <summary>스포너가 이번 wave에 스폰한 적을 모두 스폰하고, 그 적들이 전부 죽었을 때 호출됨</summary>
    void OnSpawnerWaveCleared(EnemySpawner spawner)
    {
        spawner.WaveCleared -= OnSpawnerWaveCleared;
        ClearWave();
    }

    // 마지막 적 죽으면 실행
    void ClearWave()
    {
        currentWaveIndex++;
        WaveCleared?.Invoke();
    }

    void ActivateSpawner()
    {
        if (enemySpanwers == null || enemySpanwers.Count == 0)
        {
            Debug.LogWarning("[WaveManager] enemySpanwers 가 비어있습니다.");
            return;
        }

        var spawner = enemySpanwers[activatedSpawnerNum];
        spawner.WaveCleared += OnSpawnerWaveCleared;
        spawner.StartWave(currentWaveIndex);
        activatedSpawnerNum++;

        if (spanwerActivateClip != null)
            _audioSource.PlayOneShot(spanwerActivateClip);
    }

}
