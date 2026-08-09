// using Godot;
// using System;
// public partial class PlayerMoveState : IState
// {
//     public Player Player;
//     Vector2 input;
//     Vector2 velocity;
//     public void Enter()
//     {
//     }

//     public void Exit()
//     {
//     }

//     public PlayerMoveState(Player _player)
//     {
//         Player = _player;
//     }

//     public void Update(double delta)
//     {
//         Vector2 input = Player.inputComponent.MovementInput;
//         if (input != Vector2.Zero)
//         {
//             Player.movementComponent.Move(input);
//             Player.animationComponent.SetAnimation("move");
//         }
//         else
//         {
//             Player.stateMachine.ChangeState(Player.idleState);
//         }
//     }
// }