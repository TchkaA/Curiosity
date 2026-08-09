// using Godot;
// using System;
// public partial class PlayerIdleState : IState
// {
//     private Player _player;
//     Vector2 input;
//     public void Enter()
//     {
//         _player.animationComponent.SetAnimation("idle");
//     }

//     public void Exit()
//     {
//     }

//     public PlayerIdleState(Player _player)
//     {
//         this._player = _player;
//     }

//     public void Update(double delta)
//     {
//         input = _player.inputComponent.MovementInput;
//         if (input != Vector2.Zero)
//         {
//             _player.stateMachine.ChangeState(_player.moveState);
//             return;
//         }
//     }
// }