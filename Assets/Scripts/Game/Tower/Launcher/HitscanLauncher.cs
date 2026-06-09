using ActionGameFramework.Health;
using TowerDefense.Towers.Projectiles;
using UnityEngine;

namespace TowerDefense.Towers.TowerLaunchers
{
    /// <summary>
    /// An implementation of the tower launcher for hitscan attacks
    /// </summary>
    public class HitscanLauncher : Launcher
    {
        /// <summary>
        /// 발사 효과를 표시하기 위한 파티클 시스템
        /// </summary>
        public ParticleSystem fireParticleSystem;


        /// <summary>
        /// 히트스캔 공격을 초기화하고 대상에게 공격 수행
        /// 공격 오브젝트에 HitscanAttack.cs가 없으면 return
        /// </summary>
        /// <param name="enemy">
        /// 이 타워가 공격 대상으로 삼고 있는 적
        /// </param>
        /// <param name="attack">
        /// 적에게 데미지를 주는 projectile 오브젝트
        /// </param>
        /// 공격이 발사되는 위치
        /// <param name="firingPoint"></param>
        public override void Launch(Targetable enemy, GameObject attack, Transform firingPoint)
        {
            var hitscanAttack = attack.GetComponent<HitscanAttack>();
            if (hitscanAttack == null)
            {
                return;
            }
            hitscanAttack.transform.position = firingPoint.position;
            hitscanAttack.AttackEnemy(firingPoint.position, enemy);
            PlayParticles(fireParticleSystem, firingPoint.position, enemy.Position);
        }
    }
}