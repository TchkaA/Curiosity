using Godot;

public class CombatComponent
{

    private Player _player;  

    public CombatComponent(Player player)
    {
        _player = player;
    }

    public void PerformAttack()
    {
        _player.animation.PlayWeaponAnimation("base_attack");
    }

}