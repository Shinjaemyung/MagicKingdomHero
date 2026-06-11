using System.Collections.Generic;
//using ActionGameFramework.Audio;
using ActionGameFramework.Health;
using Core.Health;
using TowerDefense.Targetting;
using TowerDefense.Towers;
using TowerDefense.Towers.Projectiles;
using UnityEngine;

namespace TowerDefense.Affectors
{
    /// <summary>
    /// 투사체 발사 공격을 처리하는 공통 클래스
    /// 
    /// 이 스크립트를 추가하기 전에 같은 GameObject에 ILauncher 구현체 추가 필수
    /// </summary>
    [RequireComponent(typeof(ILauncher))]
    public class AttackAffector : Affector, ITowerRadiusProvider
    {
        /// <summary>
        /// 공격에 사용할 투사체
        /// </summary>
        public GameObject projectile;

        /// <summary>
        /// 투사체를 발사할 위치 리스트
        /// </summary>
        public Transform[] projectilePoints;

        /// <summary>
        /// 타워가 적을 탐색할 기준 중심점
        /// </summary>
        public Transform epicenter;

        /// <summary>
        /// 다중 공격(범위 공격) 여부 설정
        /// </summary>
        public bool isMultiAttack;

        /// <summary>
        /// 초당 공격 횟수(Fires Per Second)
        /// </summary>
        public float fireRate;

        /// <summary>
        /// 공격 시 재생할 오디오 소스
        /// </summary>
        //public RandomAudioSource randomAudioSource;

        public Targetter towerTargetter;

        /// <summary>
        /// 공격 범위 시각화 색상
        /// </summary>
        public Color radiusEffectColor;

        /// <summary>
        /// bool 값을 반환하는 조건식 델리게이트
        /// </summary>
        public delegate bool Filter();

        public Filter searchCondition;
        public Filter fireCondition;

        protected ILauncher _launcher;

        /// <summary>
        /// 다음 공격 가능까지 남은 시간
        /// </summary>
        protected float _fireTimer;

        /// <summary>
        /// 현재 추적 중인 적
        /// </summary>
        protected Targetable _trackingEnemy;

        /// <summary>
        /// 공격할 적
        /// </summary>
        protected Targetable _attackTarget;

        /// <summary>
        /// 공격할 적의 위치
        /// </summary>
        protected Vector3 _attackTargetPos;

        /// <summary>
        /// 애니메이션 시작 시점에 저장한 _attackTarget의 SpawnId.
        /// FireProjectile() 시점에 비교해서 그 사이에 pool 반환→재스폰됐는지 검증.
        /// </summary>
        private int _attackTargetSpawnId = -1;

        [Tooltip("공격 애니메이션을 재생할 Animator")]
        public Animator attackAnimator;

        /// <summary>Attack 파라미터 해시</summary>
        private static readonly int AttackHash = Animator.StringToHash("Attack");

        /// <summary>
        /// Targetter의 탐색 주기
        /// </summary>
        public float SearchRate
        {
            get { return towerTargetter.searchRate; }
            set { towerTargetter.searchRate = value; }
        }

        /// <summary>
        /// 현재 추적중인 대상
        /// </summary>
        public Targetable TrackingEnemy
        {
            get { return _trackingEnemy; }
        }

        /// <summary>
        /// 공격 범위
        /// </summary>
        public float EffectRadius
        {
            get { return towerTargetter.EffectRadius; }
        }

        public Color EffectColor
        {
            get { return radiusEffectColor; }
        }

        public Targetter Targetter
        {
            get { return towerTargetter; }
        }

        private void OnEnable()
        {
            Tower tower = GetComponentInParent<Tower>();
            epicenter = tower.GetComponent<Transform>();
        }

        /// <summary>
        /// AttackAffector 초기화
        /// </summary>
        public override void Initialize()
        {
            Initialize(-1);
        }

        /// <summary>
        /// 레이어 마스크로 AttackAffector 초기화
        /// </summary>
        public override void Initialize(LayerMask mask)
        {
            base.Initialize(mask);
            SetUpTimers();

            towerTargetter.ResetTargetter();
            //towerTargetter.alignment = affectorAlignment;
            towerTargetter.AcquiredTarget += OnAcquiredTarget;
            towerTargetter.LostTarget += OnLostTarget;
        }

        void OnLostTarget()
        {
            _trackingEnemy = null;
        }

        void OnAcquiredTarget(Targetable acquiredTarget)
        {
            _trackingEnemy = acquiredTarget;
        }

        public Damager DamagerProjectile
        {
            get { return projectile == null ? null : projectile.GetComponent<Damager>(); }
        }

        /// <summary>
        /// 투사체의 총 데미지
        /// </summary>
        public float GetProjectileDamage()
        {
            var splash = projectile.GetComponent<SplashDamager>();
            float splashDamage = splash != null ? splash.damage : 0;
            return DamagerProjectile.damage + splashDamage;
        }

        /// <summary>
        /// 공격 타이머 초기화
        /// </summary>
        protected virtual void SetUpTimers()
        {
            _fireTimer = 1 / fireRate;
            _launcher = GetComponent<ILauncher>();
        }

        /// <summary>
        /// 공격 타이머 갱신
        /// </summary>
        protected virtual void Update()
        {
            _fireTimer -= Time.deltaTime;
            if (_trackingEnemy != null && _fireTimer <= 0.0f)
            {
                OnFireAnimation();
                _fireTimer = 1 / fireRate;
            }
        }

        /// <summary>
        /// 공격 주기마다 호출. 애니메이터가 있으면 애니메이션만 재생하고
        /// 실제 발사는 Animation Event(OnAttackAnimationEvent)에서 처리.
        /// 애니메이터가 없으면 즉시 발사.
        /// </summary>
        protected virtual void OnFireAnimation()
        {
            if (fireCondition != null && !fireCondition())
                return;

            _attackTarget = _trackingEnemy;

            // 애니메이션 시작 시점의 SpawnId를 저장.
            // 애니메이션 재생 도중 _attackTarget이 pool 반환→재스폰되면
            // SpawnId가 바뀌므로 FireProjectile()에서 발사를 취소할 수 있다.
            var poolable = _attackTarget.GetComponent<Poolable>();
            _attackTargetSpawnId = poolable.SpawnId;
            _attackTargetPos = _attackTarget.gameObject.transform.position;

            if (attackAnimator != null)
            {
                // fireRate에 맞게 애니메이션 속도 조절 (클립 길이 직접 계산)
                var clips = attackAnimator.runtimeAnimatorController.animationClips;
                float clipLength = clips.Length > 0 ? clips[0].length : 1f;
                attackAnimator.speed = clipLength * fireRate;
                attackAnimator.ResetTrigger(AttackHash);
                attackAnimator.SetTrigger(AttackHash);
            }
            else
            {
                FireProjectile();
            }
        }

        /// <summary>
        /// Animation Event에서 호출. 실제 투사체 발사.
        /// </summary>
        public void OnAttackAnimationEvent()
        {
            FireProjectile();
        }

        /// <summary>
        /// 공격 시 공통으로 수행되는 로직
        /// </summary>
        protected virtual void FireProjectile()
        {
            if (_attackTarget == null)
                return;

            // 애니메이션 시작 이후 _attackTarget이 pool 반환→재스폰됐는지 검증
            // pool 오브젝트는 동일 인스턴스를 재사용하므로 SpawnId로 구분
            var poolable = _attackTarget.GetComponent<Poolable>();
            if (poolable != null && poolable.SpawnId != _attackTargetSpawnId)
            {
                _launcher.LaunchAtPosition(_attackTargetPos, DamagerProjectile.gameObject, projectilePoints);
            }
            else
            {
                if (isMultiAttack)
                {
                    //List<Targetable> enemies = towerTargetter.GetAllTargets();
                    //_launcher.Launch(enemies, projectile, projectilePoints);
                }
                else
                {
                    _launcher.Launch(_attackTarget, DamagerProjectile.gameObject, projectilePoints);
                }
            }

            // 오디오 나중에
            /*
            if (randomAudioSource != null)
            {
                randomAudioSource.PlayRandomClip();
            }
            */
        }

        /// <summary>
        ///대상 간 거리를 비교하기 위한 함수
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        protected virtual int ByDistance(Targetable first, Targetable second)
        {
            float firstSqrMagnitude = Vector3.SqrMagnitude(first.Position - epicenter.position);
            float secondSqrMagnitude = Vector3.SqrMagnitude(second.Position - epicenter.position);
            return firstSqrMagnitude.CompareTo(secondSqrMagnitude);
        }

        void OnDestroy()
        {
            towerTargetter.AcquiredTarget -= OnAcquiredTarget;
            towerTargetter.LostTarget -= OnLostTarget;
        }

        /*
#if UNITY_EDITOR
        /// <summary>
        /// 탐색 범위 시각화
        /// </summary>
        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(epicenter.position, towerTargetter.effectRadius);
        }
#endif
        */
    }
}
