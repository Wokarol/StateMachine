using UnityEngine;

namespace Wokarol.StateMachineSystem
{
    public class RandomSelectorState : State
    {
        private readonly State[] allStates;
        private State nextState;


        public RandomSelectorState(string name, params State[] allStates)
        {
            Name = name;
            this.allStates = allStates;
        }

        protected override void EnterProcess(bool transitionToSelf)
            => nextState = allStates[Random.Range(0, allStates.Length)];

        protected override State Process(float delta) => nextState;
    }
}
