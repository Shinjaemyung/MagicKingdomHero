using Core.Health;
using UnityEngine;

namespace TowerDefense.Affectors
{
    /// <summary>
    /// 모든 Affector의 공통 기능과 데이터를 제공하는 기본 클래스
    /// </summary>
    public abstract class Affector : MonoBehaviour
    {
        /// <summary>
        /// UI에 표시할 간단한 설명
        /// </summary>
        public string description;

        /// <summary>
        /// 적 체크에 사용할 물리 레이어 마스크
        /// </summary>
        public LayerMask EnemyMask { get; protected set; }

        /// <summary>
        /// Initializes the effect with search data
        /// </summary>
        /// <param name="affectorAlignment">
        /// The alignment of the effect for search purposes
        /// </param>
        /// <param name="mask">
        /// The physics layer of to search for
        /// </param>
        public virtual void Initialize(LayerMask mask)
        {
            EnemyMask = mask;
        }

        /// <summary>
        /// Initializes the effect with search data
        /// </summary>
        /// <param name="affectorAlignment">
        /// The alignment of the effect for search purposes
        /// </param>
        public virtual void Initialize()
        {
            Initialize(-1);
        }
    }
}