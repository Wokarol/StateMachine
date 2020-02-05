using System;

namespace Wokarol.StateMachineSystem
{
    public class ParalelState : State
    {
        State[] states;

        public ParalelState(string name, params State[] states)
        {
            this.Name = name;
            this.states = states ?? throw new ArgumentNullException(nameof(states));
        }

        public override bool CanTransitionToSelf
        {
            get
            {
                bool allCan = true;
                for (int i = 0; i < states.Length; i++)
                {
                    if (!states[i].CanTransitionToSelf)
                    {
                        allCan = false;
                    }
                }
                return allCan;
            }
        }
        public override bool HadFinished
        {
            get
            {
                bool allFinished = true;
                for (int i = 0; i < states.Length; i++)
                {
                    if (!states[i].HadFinished)
                    {
                        allFinished = false;
                    }
                }
                return allFinished;
            }
        }

        protected override void EnterProcess(bool transitioningToSelf)
        {
            for (int i = 0; i < states.Length; i++)
            {
                states[i].Enter(StateMachine, transitioningToSelf);
            }
        }

        protected override void ExitProcess(bool transitioningToSelf)
        {
            for (int i = 0; i < states.Length; i++)
            {
                states[i].Exit(transitioningToSelf);
            }
        }

        protected override State Process(float delta)
        {
            for (int i = 0; i < states.Length; i++)
            {
                states[i].Tick(delta);
            }
            return null;
        }
    }
}
