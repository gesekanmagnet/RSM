using UnityEngine;

namespace RSM
{
    public class AIBrain<T> : MonoBehaviour, IBrain where T : MonoBehaviour
    {
        private State<T> currentState;

        [Tooltip("All current active states")]
        [SerializeField] private State<T>[] states;

        private T control;

        /// <summary>
        /// Current active state
        /// </summary>
        public AIState<T> CurrentState => currentState.active;
        /// <summary>
        /// All current active states
        /// </summary>
        public IStateWrapper[] WrappedStates => states;

        private float timeInState;
        /// <summary>
        /// The amount of time in seconds that has elapsed
        /// since state was entered
        /// </summary>
        public float TimeInState => timeInState;

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
            currentState.TickTransition(this);
        }

        protected void FixedUpdate()
        {
            currentState.active.OnFixedUpdateState(control);
        }

        /// <summary>
        /// Switch the current state to the next determined state
        /// </summary>
        /// <param name="index">The next state index</param>
        public void SwitchState(int index)
        {
            if (index == -1) return;
            if (currentState.Equals(states[index])) return;

            currentState.active.OnExitState(control);
            currentState = states[index];
            currentState.active.OnEnterState(control);

            timeInState = 0;
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
        /// Executed in Awake function
        /// </summary>
        protected virtual void Initialize() { }
    }

    public interface IBrain
    {
        IStateWrapper[] WrappedStates { get; }
    }

    public interface IStateWrapper
    {
        ScriptableObject Active { get; }
    }

    [System.Serializable]
    public class State<T> : IStateWrapper where T : MonoBehaviour
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
        public void TickTransition(AIBrain<T> brain)
        {
            if(transitions.Length == 0) return;

            foreach (var item in transitions)
            {
                if(item.Decision.TrueCondition())
                {
                    brain.SwitchState(item.NextStateIndex);
                    break;
                }
            }
        }
    }

    [System.Serializable]
    public struct Transition
    {
        [Tooltip("Condition for switch states")]
        [SerializeField] private AIDecision decision;
        [Tooltip("The next state after condition is met")]
        [SerializeField] private int nextStateIndex;

        /// <summary>
        /// Condition for switch states
        /// </summary>
        public AIDecision Decision => decision;
        /// <summary>
        /// The next state after condition is met
        /// </summary>
        public int NextStateIndex => nextStateIndex;
    }
}