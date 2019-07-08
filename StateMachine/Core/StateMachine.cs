using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wokarol.StateMachineSystem
{
    public class StateMachine
    {
        // Private variables
        private List<Transition> transitions = new List<Transition>();
        State initialState;

        // Properties
        public List<Transition> Transitions {
            get => transitions;
            set => transitions = value ?? throw new ArgumentNullException();
        }
        public readonly DropOutStack<State> History;
        public State CurrentState { get; private set; }
        public bool BeenTickedThrough { get; private set; } = false;

        // Constructors
        private StateMachine() { }
        public StateMachine(State initialState, int maxHistoryDepth = 10)
        {
            this.initialState = initialState;
            History = new DropOutStack<State>(maxHistoryDepth);
            ChangeState(this.initialState);
        }

        /// <summary>
        /// Changes to given state
        /// </summary>
        public void ChangeState(State nextState)
        {

            bool transitionToSameState = nextState == CurrentState;
            if (transitionToSameState && !CurrentState.CanTransitionToSelf) return; // Makes sure that transition will not occur if current state can't transition to themself

            if(CurrentState != null) {
                History.Push(CurrentState);
            }

            // Calls exit on current state, if one exist
            CurrentState?.Exit(transitionToSameState);
            CurrentState = nextState;

            // Calls enter on new state and add it to history
            if (nextState != null) {
                nextState.Enter(this, transitionToSameState);
            }
        }

        /// <summary>
        /// Ticks machine
        /// </summary>
        public void Tick(float delta)
        {
            BeenTickedThrough = true;

            var anyTransitionResult = StateUtils.CheckTransitions(transitions, CurrentState);

            if (CurrentState != null) {
                // There is current state
                var tickResult = CurrentState.Tick(delta);
                if(anyTransitionResult.next == null) {
                    // There is no "from any" transition
                    var stateTransitionResult = CurrentState.CheckTransitions();
                    if (stateTransitionResult.next != null) {
                        // Next transition exist
                        stateTransitionResult.action?.Invoke();
                        ChangeState(stateTransitionResult.next);
                    }
                }
            }

            if(anyTransitionResult.next != null) {
                // Next transition exist
                anyTransitionResult.action?.Invoke();
                ChangeState(anyTransitionResult.next);
            }
        }

        /// <summary>
        /// Forces check of Transitions asynchronously outside of Tick 
        /// </summary>
        public void ForceTransitionCheck()
        {

        }

        /// <summary>
        /// Adds transition to Transitions list (works exactly like adding transition manually)
        /// </summary>
        /// <param name="evaluator">Transition is active if this function returns true</param>
        /// <param name="nextState">State to transition to</param>
        /// <param name="onTransitionAction">Action that is called when transition is executed</param>
        public void AddTransitionFromAnyState(Func<State, bool> evaluator, State nextState, Action onTransitionAction = null)
        {
            Transitions.Add(new Transition(evaluator, nextState, onTransitionAction));
        }

        /// <summary>
        /// Changes state of StateMachine to intial state
        /// </summary>
        public void Restart()
        {
            ChangeState(initialState);
        }
    }

    public struct Transition
    {
        public readonly State NextState;
        public readonly Func<State, bool> Evaluator;
        public readonly Action OnTransitionAction;

        public Transition(Func<State, bool> evaluator, State nextState, Action onTransitionAction)
        {
            Evaluator = evaluator;
            NextState = nextState;
            OnTransitionAction = onTransitionAction;
        }
    }

    public static class StateUtils
    {
        public static TransitionResult CheckTransitions(List<Transition> transitions, State currentState)
        {
            foreach (var transition in transitions) {
                if (transition.Evaluator(currentState)) {
                    return new TransitionResult(transition.NextState, transition.OnTransitionAction);
                }
            }
            return TransitionResult.Empty;
        }

        public struct TransitionResult
        {
            public readonly State next;
            public readonly Action action;

            public TransitionResult(State next, Action action)
            {
                this.next = next;
                this.action = action;
            }

            public static TransitionResult Empty => new TransitionResult(null, null);
        }
    }
}
