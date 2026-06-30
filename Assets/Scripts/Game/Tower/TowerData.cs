using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public DamageType damageType;

    public float damage;
    public float attackRange;
    public float attackSpeed;

    public int cost;

    public Tower[] upgradeTowers;
}
