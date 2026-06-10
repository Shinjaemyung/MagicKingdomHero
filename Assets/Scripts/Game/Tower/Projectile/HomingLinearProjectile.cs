using ActionGameFramework.Health;
using ActionGameFramework.Helpers;
using Core.Health;
using UnityEngine;

namespace ActionGameFramework.Projectiles
{
    /// <summary>
    /// LinearProjectile를 기반으로 목표물을 추적하는 호밍 투사체
    /// </summary>
    public class HomingLinearProjectile : LinearProjectile
    {
        public int leadingPrecision = 2;

        /// <summary>
        /// 적의 경로 예측 여부
        /// </summary>
        public bool leadTarget;

        public Targetable _homingTarget;
        protected Vector3 targetPos;

        protected Vector3 _targetVelocity;

        float destroyTimerProgress = 0;
        float destroyTimer = 5;

        /// <summary>
        /// 발사 후 추적할 대상 설정
        /// </summary>
        /// <param name="target">추적할 대상</param>
        public override void Initialize(Targetable target)
        {
            base.Initialize(target);

            if (!target.IsDead)
            {
                _homingTarget = target;
            }
            else
            {
                targetPos = target.gameObject.transform.position;
            }
            
            destroyTimerProgress = 0;
        }

        protected virtual void FixedUpdate()
        {
            if (_homingTarget == null)
            {
                return;
            }

            _targetVelocity = _homingTarget.Velocity;
        }

        protected override void Update()
        {
            TryDestroySelf();

            if (!_fired)
            {
                return;
            }

            if (_homingTarget != null)
            {
                Quaternion aimDirection = Quaternion.LookRotation(GetHeading());

                _rigidbody.rotation = aimDirection;
                _rigidbody.linearVelocity = transform.forward * _rigidbody.linearVelocity.magnitude;
            }
            else
            {
                // 타겟이 죽으면 마지막 위치를 향해 날아감
                Vector3 heading = GetHeading();
                if (heading.sqrMagnitude > 0.01f)
                {
                    _rigidbody.rotation = Quaternion.LookRotation(heading);
                    _rigidbody.linearVelocity = transform.forward * _rigidbody.linearVelocity.magnitude;
                }
            }

            base.Update();
        }

        protected Vector3 GetHeading()
        {
            if (_homingTarget != null)
            {
                targetPos = _homingTarget.Position;
            }

            Vector3 heading;
            if (leadTarget)
            {
                heading = Ballistics.CalculateLinearLeadingTargetPoint(transform.position, targetPos,
                                                                       _targetVelocity, _rigidbody.linearVelocity.magnitude,
                                                                       acceleration,
                                                                       leadingPrecision) - transform.position;
            }
            else
            {
                heading = targetPos - transform.position;
            }

            return heading.normalized;
        }

        protected override void Fire(Vector3 firingVector)
        {
            /*
            if (_homingTarget == null)
            {
                Debug.LogError("목표물이 지정되지 않았습니다. 사격을 중단합니다.");
                return;
            }
            */
            if (_homingTarget != null)
            {
                _homingTarget.Removed += OnTargetDied;
            }

           base.Fire(firingVector);
        }

        void OnTargetDied(DamageableBehaviour targetable)
        {
            targetable.Removed -= OnTargetDied;
            _target = null;
            _homingTarget = null;
        }

        protected void TryDestroySelf()
        {
            if (_homingTarget != null)
                return;

            destroyTimerProgress += Time.deltaTime;

            if (Vector3.Distance(transform.position, targetPos) < 0.1f || destroyTimerProgress > destroyTimer)
            {
                Remove();
            }
        }

    }
}