using System;
using System.Collections.Generic;

namespace Wokarol.StateMachineSystem
{
    public abstract class State
    {
        // Private variables
        private List<Transition> transitions = new List<Transition>();

        // Properties
        public List<Transition> Transitions {
            get => transitions;
            set => transitions = value ?? throw new ArgumentNullException();
        }
        public virtual bool CanTransitionToSelf => true;
        public virtual bool HadFinished => true;
        public virtual string Name { get; protected set; } = "No name setted";
        public StateMachine StateMachine { get; private set; }

        // Events
        public event Action OnEnter;
        public event Action OnExit;

        // Exposed for StateMachine
        public void Enter(StateMachine stateMachine, bool transitioningToSelf)
        {
            StateMachine = stateMachine;
            OnEnter?.Invoke();
            EnterProcess(transitioningToSelf);
        }
        public void Exit(bool transitioningToSelf)
        {
            ExitProcess(transitioningToSelf);
            OnExit?.Invoke();
        }
        public State Tick(float delta)
        {
            return Process(delta);
        }
        public StateUtils.TransitionResult CheckTransitions()
        {
            return StateUtils.CheckTransitions(transitions, this);
        }

        // Abstract functions
        protected virtual void EnterProcess(bool transitioningToSelf) { }
        protected virtual void ExitProcess(bool transitioningToSelf) { }
        protected virtual State Process(float delta) { return null; }


        /// <summary>
        /// Adds transition to Transitions list (works exactly like adding transition manually)
        /// </summary>
        /// <param name="nextState">State to transition to</param>
        /// <param name="evaluator">Transition is active if this function returns true</param>
        /// <param name="onTransitionAction">Action that is called when transition is executed</param>
        public void AddTransition(State nextState, Func<State, bool> evaluator, Action onTransitionAction = null)
        {
            Transitions.Add(new Transition(evaluator, nextState, onTransitionAction));
        }
    }
}
