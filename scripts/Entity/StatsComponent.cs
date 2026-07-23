using System.Threading.Tasks;

public class StatsComponent
{
    //-------- Basic Stats ---------
    public int Health { get; private set; }
    public int MaxHealth { get; private set; }
    public int Stamina { get; private set; }
    public int MaxStamina { get; private set; }
    public int Speed { get; private set; }
    public int Strength { get; private set; }
    //-------- Driver Stats ---------



    // ------- Other ---------
    public float TickInterval { get; private set; } = 2.0f; // How often the stats are updated, in seconds

    // ------- Constructor ---------
    public StatsComponent(int health = 100, int stamina = 100, int speed = 400, int strength = 10)
    {
        MaxHealth = health;
        Health = MaxHealth;
        MaxStamina = stamina;
        Stamina = MaxStamina;
        Speed = speed;
        Strength = strength;
    }


    // ------- Methods ---------
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0)
        {
            Health = 0;
        }
    }

    public void Heal(int amount)
    {
        Health += amount;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    public void UseStamina(int amount)
    {
        Stamina -= amount;
        if (Stamina < 0)
        {
            Stamina = 0;
        }
    }

    public void RecoverStamina(int amount)
    {
        Stamina += amount;
        if (Stamina > MaxStamina)
        {
            Stamina = MaxStamina;
        }
    }

    public async Task Tick()
    {
        await Task.Delay((int)(TickInterval * 1000));
    }

    public async Task Poison(int times = 2, int damage = 1)
    {
        for (int i = 0; i < times; i++)
        {
            TakeDamage(damage);
            await Tick();
        }
    }

}