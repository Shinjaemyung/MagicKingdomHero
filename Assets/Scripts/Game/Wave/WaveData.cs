using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 하나의 Wave에 대한 스폰 정보를 담는 데이터 에셋.
/// WaveManager가 List&lt;WaveData&gt;를 wave 순서대로 들고 있으며,
/// 인스펙터에서 Wave마다 이 에셋의 값(적 종류, 수량, 간격, 방향)을 조절해 밸런스를 잡는다.
/// 기존에 EnemySpawner가 개별적으로 들고 있던 WaveSpawnData 리스트를 대신한다.
/// </summary>
[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    [Tooltip("이 Wave에서 진행할 스폰 그룹 목록 (스폰 방향별로 나눠서 등록 가능)")]
    public List<SpawnInfo> spawnInfos;
}
