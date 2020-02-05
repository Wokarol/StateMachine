# StateMachine
It allows you to create generic State Machines using code

State Machine like this *[Image taken from http://www.gameprogrammingpatterns.com/state.html, I recommend you to read it]
![State Machine from gameprogrammingpatterns.com](http://www.gameprogrammingpatterns.com/images/state-flowchart.png)*
can be converted to code like that
```cs
// States
State standing = new Standing(this.characterController);
State jumping = new Jumping(this.characterController);
State ducking = new Ducking(this.characterController);
State diving = new Diving(this.characterController);

// Transitions
standing.AddTransition(ducking, s => downPressed);
standing.AddTransition(jumping, s => bPressed);

jumping.AddTransition(diving, s => downPressed);

ducking.AddTransition(standing, s => !downPressed);

// Initialization
stateMachine = new StateMachine(standing);
```
you can also move transitions to states themselves, then you just need to pass states to other states like that for example
```cs
// States
State standing = new Standing(this.characterController);
State jumping = new Jumping(this.characterController);
State ducking = new Ducking(this.characterController);
State diving = new Diving(this.characterController);

// State Injections
standing.SetStates(ducking, jumping);
jumping.SetStates(diving);
ducking.SetStates(standing);

// Initialization
stateMachine = new StateMachine(standing);
```

>Rememeber, you will have to call
>```cs
>stateMachine.Tick(Time.deltaTime);
>```
>each frame
