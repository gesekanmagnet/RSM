using UnityEngine;

namespace RSM
{
    public class AIBrain<T> : MonoBehaviour, IBrain where T : MonoBehaviour
    {
        private State currentState;

        [Tooltip("All current active states")]
        [SerializeField] private State[] states;

        private T control;

        /// <summary>
        /// Current active state
        /// </summary>
        public AIState<T> CurrentState => currentState.active;
        /// <summary>
        /// All current active states
        /// </summary>
        public IState[] WrappedStates => states;

        protected float timeInState { get; set; }
        /// <summary>
        /// The amount of time in seconds that has elapsed
        /// since state was entered
        /// </summary>
        public float TimeInState => timeInState;
        protected bool stateFinished { get; set; }
        /// <summary>
        /// Indicates whether the current state has completed
        /// </summary>
        public bool StateFinished => stateFinished;

        protected void Awake()
        {
            control = GetComponent<T>();
            currentState = states[0];
            Initialize();
        }

        protected void Start()
        {
            currentState.active.OnEnterState(control);
            timeInState = 0;
        }

        protected void Update()
        {
            if(currentState == null)
            {
                Debug.LogError($"[{gameObject.name}] state is null. No state to execute");
                return;
            }

            timeInState += Time.deltaTime;

            currentState.active.OnUpdateState(control);
            currentState.TickTransition(this, control);
        }

        protected void FixedUpdate()
        {
            currentState.active.OnFixedUpdateState(control);
        }

        /// <summary>
        /// Switch the current state to the next determined state
        /// </summary>
        /// <param name="index">The next state index</param>
        protected void SwitchState(int index)
        {
            if (index == -1) return;
            if (currentState.Equals(states[index])) return;

            currentState.active.OnExitState(control);
            currentState = states[index];
            currentState.active.OnEnterState(control);

            timeInState = 0;
            stateFinished = false;
        }

        /// <summary>
        /// Get the state from the current list
        /// </summary>
        /// <typeparam name="U">Reference to the specified state</typeparam>
        /// <returns>Returns as specified state. If no state found, returns -1</returns>
        protected int GetState<U>() where U : AIState<T>
        {
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].active is U) return i;
            }

            Debug.Log($"State {typeof(U).Name} not found. Return null");
            return -1;
        }

        /// <summary>
        /// Set the current state to finish, useful for transition handling
        /// </summary>
        public void FinishTheCurrentState() => stateFinished = true;

        /// <summary>
        /// Executed in Awake function
        /// </summary>
        protected virtual void Initialize() { }
        
        [System.Serializable]
        public class State : IState
        {
            [Tooltip("State for the Brain")]
            [SerializeField] private AIState<T> state;
            [Tooltip("All active transition decisions")]
            [SerializeField] private Transition[] transitions;

            /// <summary>
            /// State for the Brain
            /// </summary>
            public AIState<T> active => state;

            public ScriptableObject Active => state;

            /// <summary>
            /// Update the transition condition per frame
            /// </summary>
            /// <param name="brain">Current used AI Brain</param>
            public void TickTransition(AIBrain<T> brain, T control)
            {
                if(transitions.Length == 0) return;

                foreach (var item in transitions)
                {
                    if(item.Decision == null) continue;

                    if(item.Decision.TrueCondition(control))
                    {
                        brain.SwitchState(item.NextStateIndex);
                        break;
                    }
                }
            }
        }

        [System.Serializable]
        public struct Transition : ITransition
        {
            [Tooltip("Condition for switch states")]
            [SerializeField] private AIDecision<T> decision;
            [Tooltip("The next state after condition is met")]
            [SerializeField] private int nextStateIndex;

            /// <summary>
            /// Condition for switch states
            /// </summary>
            public AIDecision<T> Decision => decision;
            /// <summary>
            /// The next state after condition is met
            /// </summary>
            public int NextStateIndex => nextStateIndex;
        }
    }

    public interface IBrain { IState[] WrappedStates { get; } }
    public interface IState { ScriptableObject Active { get; } }
    public interface ITransition { }

}