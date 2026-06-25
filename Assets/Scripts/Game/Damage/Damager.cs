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

        /// <summary>
        /// Collision Projectile 프리팹을 생성할 확률
        /// </summary>
        [Range(0, 1)]
        public float chanceToSpawnCollisionPrefab = 1.0f;

        /// <summary>
        /// 공격했을 때 발생하는 파티클
        /// </summary>
        public ParticleSystem collisionParticles;

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
                return;
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
        /// 파티클 시스템을 생성하고 재생
        /// </summary>
        void OnCollisionEnter(Collision other)
        {
            if (collisionParticles == null || Random.value > chanceToSpawnCollisionPrefab)
            {
                return;
            }

            var pfx = PoolManager.Instance.GetObject(collisionParticles.gameObject).GetComponent<ParticleSystem>();

            pfx.transform.position = transform.position;
            pfx.Play();
        }
    }
}