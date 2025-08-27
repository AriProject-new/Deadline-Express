using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }

    public void Setup(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
    }

    public void Damage(int amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }
    }
}