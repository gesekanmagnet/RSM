using UnityEngine;

namespace RSM
{
    public abstract class AIDecision : MonoBehaviour
    {
        /// <summary>
        /// Conditions needed for transitioning to another state
        /// </summary>
        /// <returns>Return the conditions for transition to other state have been met</returns>
        public abstract bool TrueCondition();
    }
}