using System;
using UnityEngine;

namespace Core.Utilities
{
    /// <summary>
    /// A timer data model. Consumed/process by the TimedBehaviour
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// Event fired on elapsing
        /// </summary>
        readonly Action _callback;

        /// <summary>
        /// The time
        /// </summary>
        float _time, _currentTime;

        /// <summary>
        /// Normalized progress of the timer
        /// </summary>
        public float NormalizedProgress
        {
            get { return Mathf.Clamp(_currentTime / _time, 0f, 1f); }
        }

        /// <summary>
        /// Timer constructor
        /// </summary>
        /// <param name="newTime">the time that timer is counting</param>
        /// <param name="onElapsed">the event fired at the end of the timer elapsing</param>
        public Timer(float newTime, Action onElapsed = null)
        {
            SetTime(newTime);

            _currentTime = 0f;
            _callback += onElapsed;
        }

        /// <summary>
        /// Returns the result of AssessTime
        /// </summary>
        /// <param name="deltaTime">change in time between ticks</param>
        /// <returns>true if the timer has elapsed, false otherwise</returns>
        public virtual bool Tick(float deltaTime)
        {
            return AssessTime(deltaTime);
        }

        /// <summary>
        /// Checks if the time has elapsed and fires the tick event
        /// </summary>
        /// <param name="deltaTime">the change in time between assessments</param>
        /// <returns>true if the timer has elapsed, false otherwise</returns>
        protected bool AssessTime(float deltaTime)
        {
            _currentTime += deltaTime;
            if (_currentTime >= _time)
            {
                FireEvent();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Resets the current time to 0
        /// </summary>
        public void Reset()
        {
            _currentTime = 0;
        }

        /// <summary>
        /// Fires the associated timer event
        /// </summary>
        public void FireEvent()
        {
            _callback.Invoke();
        }

        /// <summary>
        /// Sets the elapsed time
        /// </summary>
        /// <param name="newTime">sets the time to a new value</param>
        public void SetTime(float newTime)
        {
            _time = newTime;

            if (newTime <= 0)
            {
                _time = 0.1f;
            }
        }
    }
}