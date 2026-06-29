using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Objects/TowerData")]
public class TowerData : ScriptableObject
{
    public string towerName;

    public int cost;
    //public int sell;

    public DamageType damageType;

    public Tower[] upgradeTowers;
}
