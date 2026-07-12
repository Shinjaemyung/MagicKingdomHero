using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    public Tower tower;

    [SerializeField, Tooltip("이 타워의 이름")]
    public string towerName;
    [SerializeField, Tooltip("이 타워의 데미지")]
    public float damage;
    [SerializeField, Tooltip("이 타워의 데미지 타입")]
    public DamageType damageType;
    [SerializeField, Tooltip("이 타워의 공격 범위")]
    public float attackRange;
    [SerializeField, Tooltip("이 타워의 공격 속도")]
    public float attackSpeed;
    [SerializeField, Tooltip("이 타워의 공격이 줄 상태 이상 효과들")]
    public List<StatusEffectData> statusEffects;
    [SerializeField, Tooltip("이 타워의 설치 및 업그레이드 비용")]
    public int cost;

    public TowerData[] upgradeTowers;
}
