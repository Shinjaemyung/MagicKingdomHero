using System;
using Core.Health;
using Core.Utilities;
using UnityEngine;

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
        public Action<DamageableBehaviour> Damaged;

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
        public void OnDamaged(DamageableBehaviour damageable)
        {
            if (Damaged != null)
            {
                Damaged(damageable);
            }
        }
    }
}