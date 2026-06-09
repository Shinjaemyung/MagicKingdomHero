using ActionGameFramework.Health;
using Core.Utilities;
using UnityEngine;

namespace TowerDefense.Towers.Projectiles
{
    /// <summary>
    /// 적에게 즉시 데미지를 입히는 Hitscan Attack
    /// </summary>
    [RequireComponent(typeof(Damager))]
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


        protected Damager _damager;

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
            _damager = GetComponent<Damager>();
            _timer = new Timer(delay, DealDamage);
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
        public void AttackEnemy(Vector3 origin, Targetable enemy)
        {
            _enemy = enemy;
            _origin = origin;
            _timer.Reset();
            _pauseTimer = false;
        }

        /// <summary>
        /// Hitscan 공격의 실제 공격 동작
        /// 공격할 적이 없으면 return
        /// </summary>
        protected void DealDamage()
        {
            if (_enemy == null)
            {
                ReturnToPool();
                return;
            }

            // 이펙트
            ParticleSystem pfxPrefab = _damager.collisionParticles;
            var attackEffect = PoolManager.Instance.GetObject(pfxPrefab.gameObject).GetComponent<ParticleSystem>();
            attackEffect.transform.position = _enemy.Position;
            attackEffect.Play();
            
            _enemy.TakeDamage(_damager.damage, _enemy.Position, _damager.AlignmentProvider);
            _pauseTimer = true;

            ReturnToPool();
        }
    }
}