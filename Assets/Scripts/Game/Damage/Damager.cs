using System;
using Core.Health;
using Core.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ActionGameFramework.Health
{
    /// <summary>
    /// 데미지를 줄 수 있는 컴포넌트
    /// </summary>
    public class Damager : MonoBehaviour
    {
        /// <summary>
        /// 이 Damager가 주는 데미지
        /// </summary>
        public float damage;

        /// <summary>
        /// 이 Damager가 주는 데미지의 속성 (typeCalculations와 매칭)
        /// </summary>
        public DamageType damageType = DamageType.Normal;

        /// <summary>
        /// 데미지를 주었을 때 발생하는 이벤트
        /// </summary>
        public Action<Vector3> hasDamaged;

        [SerializeField, Tooltip("공격했을 때 발생하는 파티클")]
        ParticleSystem hitParticle;

        [SerializeField, Tooltip("공격 맞았을 때 효과음")]
        AudioClip hitSound;

        /// <summary>
        /// Damager의 alignment
        /// </summary>
        public SerializableIAlignmentProvider alignment;

        /// <summary>
        /// Damager의 alignment를 가져옴
        /// </summary>
        public IAlignmentProvider AlignmentProvider
        {
            get { return alignment != null ? alignment.GetInterface() : null; }
        }

        /// <summary>
        /// 데미지 값 설정
        /// </summary>
        /// <param name="damageAmount">
        /// 설정할 데미지 값
        /// 0보다 작은 값은 적용 안 됨
        /// </param>
        public void SetDamage(float damageAmount)
        {
            if (damageAmount < 0)
            {
                damage = 0;
            }
            damage = damageAmount;
        }

        /// <summary>
        /// 데미지가 성공적으로 적용되었을 때 이벤트 호출
        /// </summary>
        public void HasDamaged(Vector3 point, IAlignmentProvider otherAlignment)
        {
            if (hasDamaged != null)
            {
                hasDamaged(point);
            }
        }

        /// <summary>
        /// 공격이 적에게 명중했을 때
        /// </summary>
        void OnCollisionEnter(Collision other)
        {
            PlayHitEffects(transform.position);
        }

        /// <summary>
        /// 파티클과 효과음을 재생
        /// </summary>
        public void PlayHitEffects(Vector3 hitPosition)
        {
            PlayHitEffects(hitPosition, Vector3.up);
        }

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
}