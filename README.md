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
standing.AddTransition(s => downPressed, ducking);
standing.AddTransition(s => bPressed, jumping);

jumping.AddTransition(s => downPressed, diving);

ducking.AddTransition(s => !downPressed, standing);

// Initialization
stateMachine = new StateMachine(standing);
```

>Rememeber, you will have to call
>```cs
>stateMachine.Tick(Time.deltaTime);
>```
>each frame
