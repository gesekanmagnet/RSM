using UnityEngine;

namespace RSM
{
    public abstract class AIState<T> : ScriptableObject where T : MonoBehaviour
    {
        /// <summary>
        /// Function executed once state has started
        /// </summary>
        /// <param name="control">The controller as parameter control</param>
        public abstract void OnEnterState(T control);
        /// <summary>
        /// Function executed in Update loop
        /// </summary>
        /// <param name="control">The controller as parameter control</param>
        public abstract void OnUpdateState(T control);
        /// <summary>
        /// Function executed in FixedUpdate loop
        /// </summary>
        /// <param name="control">The controller as parameter control</param>
        public abstract void OnFixedUpdateState(T control);
        /// <summary>
        /// Function executed once state has ended
        /// </summary>
        /// <param name="control">The controller as parameter control</param>
        public abstract void OnExitState(T control);
    }
}