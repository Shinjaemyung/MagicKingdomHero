using ActionGameFramework.Health;
using System.Collections.Generic;
using UnityEngine;
using static AttackUtility;

namespace TowerDefense.Towers
{
    /// <summary>
    /// 타워의 다양한 발사 로직을 정의하는 인터페이스
    /// </summary>
    public interface ILauncher
    {
        /// <summary>
        /// 타워의 발사 로직을 구현하는 메서드
        /// </summary>
        /// <param name="enemy">
        /// 타워가 공격 대상으로 삼고 있는 적
        /// </param>
        /// <param name="attack">
        /// 적을 공격하는 데 사용되는 투사체 오브젝트
        /// </param>
        /// 발사 위치
        /// <param name="firingPoint"></param>
        void Launch(Targetable enemy, GameObject attack, Transform firingPoint, AttackContext attackContext);

        /// <summary>
        /// 타워의 발사 로직을 구현하는 메서드
        /// </summary>
        /// <param name="enemy">
        /// 타워가 공격 대상으로 삼고 있는 적
        /// </param>
        /// <param name="attack">
        /// 적을 공격하는 데 사용되는 투사체 오브젝트
        /// </param>
        /// <param name="firingPoints">
        /// 발사 위치 리스트
        /// </param>
        void Launch(Targetable enemy, GameObject attack, Transform[] firingPoints, AttackContext attackContext);

        /// <summary>
        /// 여러 적을 대상으로 하는 발사 로직을 구현하는 메서드
        /// </summary>
        /// <param name="enemies">
        /// 공격할 적들의 목록
        /// </param>
        /// <param name="attack">
        /// 적을 공격하는 데 사용되는 투사체 오브젝트
        /// </param>
        /// 발사 위치 리스트
        /// <param name="firingPoints"></param>
        void Launch(List<Targetable> enemies, GameObject attack, Transform[] firingPoints, AttackContext attackContext);


        /// <summary>
        /// 적이 없는 상태에서 발사 로직을 구현하는 메서드(발사 전 대상이 Remove 되었을 때 필요)
        /// </summary>
        /// <param name="position">
        /// 타워의 투사체가 날아갈 위치
        /// </param>
        /// <param name="attack">
        /// 위치를 향해 발사할 투사체 오브젝트
        /// </param>
        /// 발사 위치 리스트
        /// <param name="firingPoint"></param>
        void LaunchToPosition(Vector3 position, GameObject attack, Transform[] firingPoint);
    }
}