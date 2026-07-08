using Core.Utilities;
using UnityEngine;

/// <summary>
/// 파티클이나 효과음 등의 이펙트를 실행하는 컴포넌트
/// </summary>
public class HitEffectPlayer : MonoBehaviour
{
    [SerializeField, Tooltip("공격 맞았을 때 발생하는 파티클")]
    ParticleSystem hitParticle;

    [SerializeField, Tooltip("공격 맞았을 때 효과음")]
    AudioClip hitSound;

    /// <summary>
    /// 공격이 적에게 명중했을 때, 맞은 지점의 노멀(hitNormal) 방향에 맞춰
    /// 파티클과 효과음을 재생 (타격 방향에 따라 이펙트가 회전)
    /// </summary>
    public void PlayHitEffects(Vector3 hitPosition, Vector3 hitNormal)
    {
        Quaternion hitRotation = Quaternion.FromToRotation(Vector3.up, hitNormal);

        if (hitParticle != null)
        {
            var particleObj = PoolManager.Instance.GetObject(hitParticle.gameObject);
            particleObj.GetComponent<Poolable>().Init(hitParticle.gameObject);
            particleObj.GetComponent<PooledParticleSystem>().Play(hitPosition, hitRotation);
        }

        if (hitSound != null)
        {
            AudioManager.Instance.PlaySound(hitSound, hitPosition);
        }
    }
}
