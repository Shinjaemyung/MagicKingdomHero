using ActionGameFramework.Health;
using ActionGameFramework.Helpers;
using Core.Health;
using System;
using UnityEngine;

namespace ActionGameFramework.Projectiles
{
    /// <summary>
    /// 가속도를 받아 직선으로 날아가는 투사체
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class LinearProjectile : Poolable, IProjectile
    {
        /// <summary>
        /// 투사체가 가속하는 속도
        /// </summary>
        public float acceleration;

        /// <summary>
        /// 투사체의 초기 발사 속도
        /// </summary>
        public float startSpeed;

        /// <summary>
        /// 발사되었는지 여부
        /// </summary>
        protected bool _fired;

        protected Rigidbody _rigidbody;

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
        }

        public virtual void Initialize(Targetable target)
        {
            _target = target;
        }

        /// <summary>
        /// 투사체 상태 업데이트
        /// 발사된 상태라면 가속도 적용
        /// </summary>
        protected virtual void Update()
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
        void OnCollisionEnter(Collision other)
        {
            Targetable target = other.collider.GetComponent<Targetable>();

            if (target == null || target != _target)
                return;

            Remove();
        }

        /// <summary>
        /// 이 투사체를 풀로 반환
        /// </summary>
        protected void Remove()
        {
            ReturnToPool();
        }
    }
}