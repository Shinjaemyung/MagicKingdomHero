using UnityEngine;

/// <summary>DamageType에 대한 편의 확장 메서드</summary>
public static class DamageTypeExtensions
{
    /// <summary>이 DamageType이 등록한 아이콘을 가져온다. (DamageTypeIconDatabase 참조)</summary>
    public static Sprite GetIcon(this DamageType damageType)
    {
        var db = DamageTypeIconDatabase.Instance;
        return db != null ? db.GetIcon(damageType) : null;
    }
}
