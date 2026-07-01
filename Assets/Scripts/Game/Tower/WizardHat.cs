using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WizardHat 프리팹에 부착되어 DamageType에 어울리는 머티리얼로 색상을 바꿔주는 컴포넌트.
/// Assets/Materials 폴더의 DamageType별 전용 머티리얼(M_NormalWizardHat 등)을 인스펙터에서 매핑해 사용한다.
/// </summary>
public class WizardHat : MonoBehaviour
{
    [Serializable]
    public class DamageTypeMaterial
    {
        public DamageType damageType;
        public Material material;
    }

    [Tooltip("색상을 바꿀 대상 Renderer")]
    [SerializeField]
    private Renderer targetRenderer;

    [Tooltip("DamageType별로 적용할 머티리얼 매핑")]
    [SerializeField]
    private List<DamageTypeMaterial> materialsByDamageType = new();

    private Dictionary<DamageType, Material> _lookup;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();
    }

    /// <summary>DamageType에 어울리는 머티리얼을 찾아 Renderer에 적용</summary>
    public void ApplyMaterial(DamageType damageType)
    {
        if (targetRenderer == null)
        {
            Debug.LogWarning($"[WizardHat] {name}에서 Renderer를 찾을 수 없어 머티리얼을 적용할 수 없습니다.");
            return;
        }

        BuildLookupIfNeeded();

        if (_lookup.TryGetValue(damageType, out var material) && material != null)
        {
            targetRenderer.sharedMaterial = material;
        }
        else
        {
            Debug.LogWarning($"[WizardHat] {damageType}에 대응하는 머티리얼이 등록되어 있지 않습니다.");
        }
    }

    private void BuildLookupIfNeeded()
    {
        if (_lookup != null)
            return;

        _lookup = new Dictionary<DamageType, Material>();
        foreach (var entry in materialsByDamageType)
        {
            if (entry == null)
                continue;

            _lookup[entry.damageType] = entry.material;
        }
    }
}
