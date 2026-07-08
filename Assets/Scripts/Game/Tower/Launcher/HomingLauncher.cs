using ActionGameFramework.Health;
using ActionGameFramework.Helpers;
using ActionGameFramework.Projectiles;
using Core.Utilities;
using UnityEngine;
using static AttackUtility;

namespace TowerDefense.Towers.TowerLaunchers
{
    /// <summary>
    /// 호밍 투사체 런처
    /// </summary>
    public class HomingLauncher : Launcher
    {
        public ParticleSystem fireParticleSystem;

        /// <summary>
        /// 지정된 적에게 호밍 투사체 발사
        /// </summary>
        /// <param name="enemy">
        /// 공격할 적
        /// </param>
        /// <param name="attack">
        /// 공격에 사용될 투사체
        /// </param>
        /// <param name="firingPoint">
        /// 투사체가 발사되는 위치
        /// </param>
        public override void Launch(Targetable enemy, GameObject attack, Transform firingPoint, AttackContext attackContext)
        {
            var homingMissile = attack.GetComponent<HomingLinearProjectile>();
            if (homingMissile == null)
            {
                Debug.LogError("No HomingLinearProjectile attached to attack object");
                return;
            }
            Vector3 startingPoint = firingPoint.position;
            Vector3 targetPoint = Ballistics.CalculateLinearLeadingTargetPoint(
                startingPoint, enemy.Position,
                enemy.Velocity, homingMissile.startSpeed,
                homingMissile.acceleration);

            homingMissile.Initialize(enemy, attackContext);
            homingMissile.FireAtPoint(startingPoint, targetPoint);
            PlayParticles(fireParticleSystem, startingPoint, targetPoint);
        }

        /// <summary>
        /// 지정된 위치를 향해 호밍 투사체 발사
        /// </summary>
        /// <param name="enemy">
        /// 공격할 적
        /// </param>
        /// <param name="attack">
        /// 공격에 사용될 투사체
        /// </param>
        /// <param name="firingPoint">
        /// 투사체가 발사되는 위치
        /// </param>
        public override void LaunchToPosition(Vector3 position, GameObject attack, Transform firingPoint)
        {
            var homingMissile = attack.GetComponent<HomingLinearProjectile>();
            if (homingMissile == null)
            {
                Debug.LogError("No HomingLinearProjectile attached to attack object");
                return;
            }
            Vector3 startingPoint = firingPoint.position;
            Vector3 targetPoint = position;

            homingMissile.Initialize(position);
            homingMissile.FireAtPoint(startingPoint, targetPoint);
            PlayParticles(fireParticleSystem, startingPoint, targetPoint);
        }
    }
}