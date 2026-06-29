using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DamageType별 아이콘을 보관하는 데이터베이스
/// Resources 폴더에 두고 어디서든 DamageTypeIconDatabase.Instance.GetIcon(type)로 조회한다.
/// </summary>
[CreateAssetMenu(fileName = "DamageTypeIconDatabase", menuName = "Game/Damage Type Icon Database")]
public class DamageTypeIconDatabase : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public DamageType damageType;
        public Sprite icon;
    }

    [SerializeField]
    private List<Entry> entries = new List<Entry>();

    private Dictionary<DamageType, Sprite> _lookup;

    private const string ResourcePath = "DamageTypeIconDatabase";

    private static DamageTypeIconDatabase _instance;

    /// <summary>Resources/DamageTypeIconDatabase 에셋을 로드해서 반환하는 싱글톤 접근자</summary>
    public static DamageTypeIconDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<DamageTypeIconDatabase>(ResourcePath);
                if (_instance == null)
                    Debug.LogError($"[DamageTypeIconDatabase] Resources/{ResourcePath}.asset 을 찾을 수 없습니다.");
            }
            return _instance;
        }
    }

    private void BuildLookupIfNeeded()
    {
        if (_lookup != null) return;

        _lookup = new Dictionary<DamageType, Sprite>();
        foreach (var entry in entries)
        {
            _lookup[entry.damageType] = entry.icon;
        }
    }

    /// <summary>해당 DamageType이 등록한 아이콘 Sprite를 반환</summary>
    public Sprite GetIcon(DamageType damageType)
    {
        BuildLookupIfNeeded();

        if (_lookup.TryGetValue(damageType, out var icon))
            return icon;

        Debug.LogWarning($"[DamageTypeIconDatabase] {damageType} 에 대한 아이콘이 등록되지 않았습니다.");
        return null;
    }
}
