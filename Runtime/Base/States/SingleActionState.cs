using System;

namespace Wokarol.StateMachineSystem
{
    public class SingleActionState : State
    {
        Func<State, State> action;
        State actionResult;

        public SingleActionState(string name, Func<State, State> action)
        {
            Name = name;
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public SingleActionState(string name, Action<State> action)
            : this(name, state => { action(state); return null; }) { }

        protected override void EnterProcess(bool transitioningToSelf)
            => actionResult = action(this);

        protected override State Process(float delta) => actionResult;
    }
}
