using UnityEngine;

/// <summary>
/// WaveData 안에서 하나의 스폰 그룹을 정의.
/// 어떤 Enemy를, 어느 방향(SpawnPoint)의 스포너에서, 몇 마리, 몇 초 간격으로 생성할지를 담는다.
/// 하나의 WaveData는 여러 SpawnInfo를 가질 수 있어, 한 Wave 안에서 여러 방향에서
/// 동시에 서로 다른 적을 스폰하는 구성도 가능하다.
/// </summary>
[System.Serializable]
public class SpawnInfo
{
    [Tooltip("생성할 Enemy 데이터")]
    public EnemyData enemy;

    [Tooltip("적을 생성할 스폰 방향 (해당 방향을 가진 EnemySpawner에서 생성됨)")]
    public SpawnPoint spawnPoint;

    [Tooltip("생성할 적의 수")]
    public int count;

    [Tooltip("생성 간격 (초)")]
    public float interval;
}
