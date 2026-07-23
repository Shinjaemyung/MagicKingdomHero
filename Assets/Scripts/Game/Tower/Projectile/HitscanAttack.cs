using ActionGameFramework.Health;
using Core.Utilities;
using UnityEngine;
using static AttackUtility;

namespace TowerDefense.Towers.Projectiles
{
    /// <summary>
    /// 적에게 즉시 데미지를 입히는 Hitscan Attack
    /// </summary>
    public class HitscanAttack : Poolable
    {
        /// <summary>
        /// 공격 실행 전 대기 시간
        /// </summary>
        public float delay;

        /// <summary>
        /// 공격 대기 타이머
        /// </summary>
        protected Timer _timer;

        /// <summary>
        /// 이 Hitscan이 공격할 적
        /// </summary>
        protected Targetable _enemy;

        protected AttackContext _attackContext;
        protected HitEffectPlayer _hitEffectPlayer;

        /// <summary>
        /// 공격이 발사되는 위치
        /// </summary>
        protected Vector3 _origin;

        /// <summary>
        /// Time.timeScale을 0으로 설정하지 않고도
        /// 타이머를 일시 정지할 수 있도록 하는 설정
        /// </summary>
        protected bool _pauseTimer;


        protected virtual void Awake()
        {
            _hitEffectPlayer = GetComponent<HitEffectPlayer>();
            _timer = new Timer(delay, ExecuteAttack);
        }

        /// <summary>
        /// Timer 정지 상태가 아니라면 업데이트
        /// </summary>
        protected virtual void Update()
        {
            if (!_pauseTimer)
            {
                _timer.Tick(Time.deltaTime);
            }
        }


        /// <summary>
        /// 공격 실행을 위한 초기 설정
        /// </summary>
        /// <param name="origin">
        /// 공격이 발사되는 위치
        /// </param>
        /// <param name="enemy">
        /// 공격할 적
        /// </param>
        public void Initialize(Vector3 origin, Targetable enemy, AttackContext context)
        {
            _enemy = enemy;
            _origin = origin;
            _timer.Reset();
            _pauseTimer = false;
            _attackContext = context;
        }

        /// <summary>
        /// Hitscan 공격의 실제 공격 동작
        /// 공격할 적이 없으면 return
        /// </summary>
        protected void ExecuteAttack()
        {
            if (_enemy == null || _enemy.IsDead)
            {
                ReturnToPool();
                return;
            }

            // 충돌 지점 계산
            Vector3 hitPosition = _enemy.Position;
            Vector3 hitNormal = (_origin - hitPosition);
            hitNormal = hitNormal.sqrMagnitude > 0.0001f ? hitNormal.normalized : Vector3.up;

            Collider enemyCollider = _enemy.GetComponentInChildren<Collider>();
            if (enemyCollider != null)
            {
                Vector3 closestPoint = enemyCollider.ClosestPoint(_origin);
                Vector3 diff = _origin - closestPoint;
                if (diff.sqrMagnitude > 0.0001f)
                {
                    hitPosition = closestPoint;
                    hitNormal = diff.normalized;
                }
            }

            // 이펙트
            _hitEffectPlayer.PlayHitEffects(hitPosition, hitNormal);

            // 데미지 처리
            Enemy enemy = _enemy.GetComponent<Enemy>();
            ApplyDamage(enemy, _attackContext);
            ApplyStatusEffects(enemy, _attackContext);
            _pauseTimer = true;

            ReturnToPool();
        }
    }
}