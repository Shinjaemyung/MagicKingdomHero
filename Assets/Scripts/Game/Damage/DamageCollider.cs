using Core.Health;
using UnityEngine;

namespace ActionGameFramework.Health
{
    /// <summary>
    /// DamageZone의 콜라이더 기반 구현체
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class DamageCollider : DamageZone
    {
        /// <summary>
        /// 충돌이 발생하면, 충돌한 오브젝트에 Damager가 있다면 데미지 처리
        /// </summary>
        /// <param name="c">충돌한 콜라이더</param>
        protected void OnTriggerEnter(Collider other)
        {
            var damager = other.gameObject.GetComponent<Damager>();
            if (damager == null)
            {
                return;
            }
            LazyLoad();

            float scaledDamage = ScaleDamage(damager.damage);
            Vector3 collisionPosition = other.transform.position;
            damageableBehaviour.TakeDamage(scaledDamage, collisionPosition, damager.AlignmentProvider, damager.damageType);

            DamageableBehaviour damageable = GetComponent<DamageableBehaviour>();
            damager.OnDamaged(damageable);
        }

        /// <summary>
        /// 여러 개의 충돌 지점(Contact)의 평균 위치 계산
        /// </summary>
        /// <param name="contacts">충돌 지점들</param>
        /// <returns>평균 충돌 위치</returns>
        protected Vector3 ConvertContactsToPosition(ContactPoint[] contacts)
        {
            Vector3 output = Vector3.zero;
            int length = contacts.Length;

            if (length == 0)
            {
                return output;
            }

            for (int i = 0; i < length; i++)
            {
                output += contacts[i].point;
            }

            output = output / length;
            return output;
        }
    }
}