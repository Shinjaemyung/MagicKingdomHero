using Core.Health;
using UnityEngine;

namespace ActionGameFramework.Health
{
    /// <summary>
    /// 타깃으로 삼을 수 있는 객체
    /// </summary>
    public class Targetable : DamageableBehaviour
    {
        /// <summary>
        /// 타깃의 Transform
        /// </summary>
        public Transform targetTransform;

        /// <summary>
        /// The position of the object
        /// </summary>
        protected Vector3 _currentPosition, _previousPosition;

        /// <summary>
        /// The velocity of the rigidbody
        /// </summary>
        public virtual Vector3 Velocity { get; protected set; }

        /// <summary>
        /// The transform that objects target, which falls back to this object's transform if not set
        /// </summary>
        public Transform TargetableTransform
        {
            get
            {
                return targetTransform == null ? transform : targetTransform;
            }
        }

        /// <summary>
        /// Returns our targetable's transform position
        /// </summary>
        public override Vector3 Position
        {
            get { return TargetableTransform.position; }
        }

        /// <summary>
        /// Initialises any DamageableBehaviour logic
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            ResetPositionData();
        }

        /// <summary>
        /// Sets up the position data so velocity can be calculated
        /// </summary>
        protected void ResetPositionData()
        {
            _currentPosition = Position;
            _previousPosition = Position;
        }

        /// <summary>
        /// Calculates the velocity and updates the position
        /// </summary>
        void FixedUpdate()
        {
            _currentPosition = Position;
            Velocity = (_currentPosition - _previousPosition) / Time.fixedDeltaTime;
            _previousPosition = _currentPosition;
        }
    }
}