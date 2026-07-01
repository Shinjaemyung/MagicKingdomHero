using UnityEngine;

/// <summary>
/// Tower / VirtualTower 계층 안에서 "WizardHat" 찾아
/// DamageType에 맞는 머티리얼로 색상을 바꿔주는 공용 유틸리티
/// </summary>
public static class WizardHatMaterialApplier
{
    /// <summary>
    /// root 하위에서 WizardHat을 찾아 damageType에 대응하는 머티리얼을 적용
    /// </summary>
    public static void ApplyHatMaterial(Transform root, DamageType damageType)
    {
        if (root == null)
            return;

        var hat = root.GetComponentInChildren<WizardHat>();

        if (hat != null)
        {
            hat.ApplyMaterial(damageType);
        }
    }
}
