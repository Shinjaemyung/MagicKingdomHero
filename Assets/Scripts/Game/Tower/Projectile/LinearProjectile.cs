using ActionGameFramework.Health;
using ActionGameFramework.Helpers;
using Core.Health;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace ActionGameFramework.Projectiles
{
    /// <summary>
    /// 가속도를 받아 직선으로 날아가는 투사체
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Damager))]
    public class LinearProjectile : Poolable, IProjectile
    {
        [SerializeField, Tooltip("투사체의 초기 발사 속도")]
        public float startSpeed;

        [SerializeField, Tooltip("투사체가 가속하는 속도")]
        public float acceleration;

        protected Rigidbody _rigidbody;
        protected Light _lightSourse;
        protected Collider _collider;

        [SerializeField, Tooltip("Projectile 파티클 시스템")]
        protected ParticleSystem projectilePS;

        [SerializeField, Tooltip("충돌 이후 투사체와 분리되어 제거되는 오브젝트")]
        protected GameObject[] Detached;

        /// <summary>
        /// Remove 코루틴
        /// </summary>
        private Coroutine removeCoroutine;

        /// <summary>
        /// 발사되었는지 여부
        /// </summary>
        protected bool _fired;

        /// <summary>
        /// 투사체가 발사될 때 호출되는 이벤트
        /// </summary>
        public event Action Fired;

        /// <summary>
        /// 이 투사체가 명중해야 하는 대상
        /// </summary>
        protected Targetable _target;

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _lightSourse = GetComponent<Light>();
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }

        public virtual void Initialize(Targetable target)
        {
            _target = target;
            _rigidbody.constraints = RigidbodyConstraints.None;

            projectilePS.Play();
            if (_lightSourse != null)
                _lightSourse.enabled = true;
            _collider.enabled = true;
        }

        public virtual void Initialize(Vector3 position)
        {
            _target = null;

            _rigidbody.constraints = RigidbodyConstraints.None;

            projectilePS.Play();
            if (_lightSourse != null)
                _lightSourse.enabled = true;
            _collider.enabled = true;
        }

        /// <summary>
        /// 투사체 상태 업데이트
        /// 발사된 상태라면 가속도 적용
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (!_fired)
            {
                return;
            }

            if (Math.Abs(acceleration) >= float.Epsilon)
            {
                // 투사체 시간이 지날수록 가속
                _rigidbody.linearVelocity += transform.forward * acceleration * Time.deltaTime;
            }
        }

        protected virtual void Update()
        {

        }

        /// <summary>
        /// 발사 위치에서 목표 위치를 향해 발사
        /// </summary>
        /// <param name="startPoint">발사 시작 위치</param>
        /// <param name="targetPoint">목표 위치</param>
        public virtual void FireAtPoint(Vector3 startPoint, Vector3 targetPoint)
        {
            transform.position = startPoint;

            Fire(Ballistics.CalculateLinearFireVector(startPoint, targetPoint, startSpeed));
        }

        /// <summary>
        /// 지정한 방향으로 투사체 발사
        /// </summary>
        /// <param name="startPoint">발사 시작 위치</param>
        /// <param name="fireVector">발사 방향 벡터</param>
        public virtual void FireInDirection(Vector3 startPoint, Vector3 fireVector)
        {
            transform.position = startPoint;

            // 발사 방향 계산을 위해 아주 작은 속도를 부여
            if (Math.Abs(startSpeed) < float.Epsilon)
            {
                startSpeed = 0.001f;
            }

            Fire(fireVector.normalized * startSpeed);
        }

        /// <summary>
        /// startSpeed 값을 무시하고 전달받은 속도로 투사체 발사.
        /// </summary>
        /// <param name="startPoint">발사 시작 위치</param>
        /// <param name="fireVelocity">발사 속도 벡터</param>
        public void FireAtVelocity(Vector3 startPoint, Vector3 fireVelocity)
        {
            transform.position = startPoint;

            startSpeed = fireVelocity.magnitude;

            Fire(fireVelocity);
        }

        /// <summary>
        /// 실제 투사체 발사 처리
        /// </summary>
        /// <param name="firingVector">발사 속도 벡터</param>
        protected virtual void Fire(Vector3 firingVector)
        {
            _fired = true;

            transform.rotation = Quaternion.LookRotation(firingVector);

            _rigidbody.linearVelocity = firingVector;

            Fired?.Invoke();
        }

        /// <summary>
        /// 타깃과 충돌 처리
        /// 지정된 타깃과 충돌했을 때만 제거
        /// </summary>
        void OnTriggerEnter(Collider other)
        {
            Targetable target = other.GetComponent<Targetable>();

            if (target == null || target != _target)
                return;
            
            // 투사체 정지
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            
            if (_lightSourse != null)
                _lightSourse.enabled = false;
            _collider.enabled = false;

            if (projectilePS)
            {
                projectilePS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            // 충돌 지점 계산
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 hitNormal = transform.position - hitPoint;
            hitNormal = hitNormal.sqrMagnitude > 0.0001f ? hitNormal.normalized : -transform.forward;

            // 충돌체의 피격 시 효과
            HitEffectPlayer hitEffectPlayer = GetComponent<HitEffectPlayer>();
            if (hitEffectPlayer != null) 
            {
                hitEffectPlayer.PlayHitEffects(hitPoint, hitNormal);
            }

            // 충돌 후 투사체의 이펙트가 자연스럽게 사라지도록 처리
            // 분리된 Detached 오브젝트에는 자동 삭제를 위한 AutoDestroying 스크립트가 필요
            foreach (var detachedPrefab in Detached)
            {
                if (detachedPrefab != null)
                {
                    ParticleSystem detachedPS = detachedPrefab.GetComponent<ParticleSystem>();
                    detachedPS.Stop();
                }
            }

            Remove();
        }

        /// <summary>
        /// 이 투사체를 풀로 반환
        /// </summary>
        protected virtual void Remove()
        {
            if (removeCoroutine != null)
            {
                StopCoroutine(removeCoroutine);
                removeCoroutine = null;
            }

            ReturnToPool();
        }
    }
}