using System;
using UnityEngine;

namespace TowerDefense.Affectors
{
    /// <summary>
    /// Animator가 붙어있는 GameObject(TowerMesh 등)에 부착되는 최소 책임 컴포넌트.
    /// 애니메이션 클립 이벤트 호출 담당.
    /// </summary>
    public class AnimationEventRelay : MonoBehaviour
    {
        /// <summary>
        /// 'OnAttackAnimationEvent' 애니메이션 이벤트가 호출되면 발생.
        /// </summary>
        public event Action OnAttack;

        /// <summary>
        /// 애니메이션 클립의 AnimationEvent가 호출하는 함수.
        /// </summary>
        public void OnAttackAnimationEvent()
        {
            OnAttack?.Invoke();
        }
    }
}
