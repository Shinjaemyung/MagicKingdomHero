using UnityEngine;

/// <summary>
/// 검을 휘두르는 순간, 검의 현재 위치/회전(=휘두르는 궤적)을 기준으로
/// Sword Slash VFX(파티클 프리팹)를 1회 재생한다.
/// HeroAttack의 타격 타이밍에 맞춰 PlaySlash()를 호출해서 사용한다.
/// </summary>
public class SwordSlashFX : MonoBehaviour
{
    [SerializeField, Tooltip("슬래시 이펙트가 생성될 기준점(검 끝 쪽 Transform). 비워두면 이 오브젝트 자신을 사용한다.")]
    private Transform vfxSpawnPoint;

    [SerializeField, Tooltip("재생할 Sword Slash VFX 프리팹")]
    private ParticleSystem slashVfxPrefab;

    [SerializeField, Tooltip("이펙트 크기 보정")]
    private float vfxScale = 1f;

    [SerializeField, Tooltip("기준점 회전에 더해줄 보정 회전값(도). 이펙트가 검 방향과 안 맞으면 조정한다.")]
    private Vector3 rotationOffsetEuler = Vector3.zero;

    /// <summary>검 궤적(현재 위치/회전)을 따라 슬래시 VFX를 1회 재생한다.</summary>
    public void PlaySlash()
    {
        if (slashVfxPrefab == null)
        {
            Debug.LogWarning($"[{nameof(SwordSlashFX)}] slashVfxPrefab이 설정되지 않았습니다.", this);
            return;
        }

        Transform origin = vfxSpawnPoint != null ? vfxSpawnPoint : transform;
        Quaternion rotation = origin.rotation * Quaternion.Euler(rotationOffsetEuler);

        ParticleSystem vfx = Instantiate(slashVfxPrefab, origin.position, rotation);
        if (!Mathf.Approximately(vfxScale, 1f))
            vfx.transform.localScale *= vfxScale;

        vfx.Play(false);
        Destroy(vfx.gameObject, GetMaxLifetime(vfx) + 0.2f);
    }

    private static float GetMaxLifetime(ParticleSystem root)
    {
        float max = 0f;
        foreach (var ps in root.GetComponentsInChildren<ParticleSystem>(false))
        {
            var main = ps.main;
            float duration = main.duration + main.startLifetime.constantMax;
            if (duration > max) max = duration;
        }
        return max;
    }
}
