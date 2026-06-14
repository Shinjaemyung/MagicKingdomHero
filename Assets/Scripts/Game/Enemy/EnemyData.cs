using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Tooltip("이 Enemy의 이름")]
    public string enemyName;

    [Tooltip("Hero와 충돌 시 주는 데미지")]
    public float attackDamage;

    [Tooltip("사망 시 지급하는 골드")]
    public int goldReward;
}
