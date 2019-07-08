using System;

namespace Wokarol.StateMachineSystem
{
    public class ActionState : State
    {
        Func<State, float, State> action;

        public ActionState(string name, Func<State, float, State> action)
        {
            Name = name;
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public ActionState(string name, Action<State, float> action)
            : this(name, (state, delta) => { action(state, delta); return null; }) { }

        protected override State Process(float delta) => action(this, delta);
    }
}
