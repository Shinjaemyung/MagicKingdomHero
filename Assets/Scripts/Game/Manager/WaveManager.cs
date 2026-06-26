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

    [SerializeField, Tooltip("시작 후 첫 번째 Spawner가 활성화되기까지 걸리는 시간(초)")]
    private float firstSpawnerDelay;

    [SerializeField, Tooltip("시작 후 두 번째 Spawner가 활성화되기까지 걸리는 시간(초)")]
    private float secondSpawnerDelay;

    [SerializeField, Tooltip("시작 후 세 번째 Spawner가 활성화되기까지 걸리는 시간(초)")]
    private float thirdSpawnerDelay;

    [SerializeField, Tooltip("시작 후 마지막 Spawner가 활성화되기까지 걸리는 시간(초)")]
    private float lastSpawnerDelay;

    private int activatedSpawnerNum;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(ActivateSpawnerAfterDelay(firstSpawnerDelay));
        StartCoroutine(ActivateSpawnerAfterDelay(secondSpawnerDelay));
        StartCoroutine(ActivateSpawnerAfterDelay(thirdSpawnerDelay));
        StartCoroutine(ActivateSpawnerAfterDelay(lastSpawnerDelay));
    }

    private IEnumerator ActivateSpawnerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ActivateSpawner();
    }

    void ActivateSpawner()
    {
        if (enemySpanwers == null || enemySpanwers.Count == 0)
        {
            Debug.LogWarning("[WaveManager] enemySpanwers 가 비어있습니다.");
            return;
        }

        enemySpanwers[activatedSpawnerNum].ActivateSpawner();
        activatedSpawnerNum++;

        if (spanwerActivateClip != null)
            _audioSource.PlayOneShot(spanwerActivateClip);
    }

}
