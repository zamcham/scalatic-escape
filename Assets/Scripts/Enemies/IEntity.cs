public interface IEntity
{
    public float health { get; set; }
    public bool hasDied { get; set; }

    public void AddHealth(float amount)
    {
        if (!hasDied)
        {
            health += amount;

            if (health <= 0)
            {
                OnDeath();
            }
        }        
    }

    public void SetHealth(float health)
    {
        this.health = health;

        if (!hasDied && health <= 0)
        {
            OnDeath();
        }
    }

    public void Kill()
    {
        if (!hasDied)
        {
            health = 0;
            OnDeath();
        }        
    }

    public void OnDeath();
}