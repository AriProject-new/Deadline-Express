using NUnit.Framework;
using UnityEngine;

public class PlayerHealthTests
{
    [Test]
    public void Damage_ReducesHealth_ByCorrectAmount()
    {
        // ARRANGE
        // Create a new GameObject and add our PlayerHealth component to it.
        GameObject playerObject = new GameObject("Test Player");
        PlayerHealth playerHealth = playerObject.AddComponent<PlayerHealth>();
        playerHealth.Setup(maxHealth: 100);

        // ACT
        // Call the specific method we want to test.
        playerHealth.Damage(amount: 20);

        // ASSERT
        // Check if the result is what we expect.
        // We expect 100 - 20 = 80.
        Assert.AreEqual(80, playerHealth.CurrentHealth);
    }
    [Test]
    public void Damage_WithMoreDamageThanHealth_HealthBecomesZero()
    {
        // ARRANGE
        GameObject playerObject = new GameObject("Test Player");
        PlayerHealth playerHealth = playerObject.AddComponent<PlayerHealth>();
        playerHealth.Setup(maxHealth: 100);
        playerHealth.Damage(amount: 90); // Health is now 10

        // ACT
        // Deal lethal damage (25 damage to 10 health)
        playerHealth.Damage(amount: 25);

        // ASSERT
        // We expect health to be clamped at 0, not -15.
        Assert.AreEqual(0, playerHealth.CurrentHealth);
    }

    [Test]
    public void Damage_WithZeroAmount_HealthDoesNotChange()
    {
        // ARRANGE
        GameObject playerObject = new GameObject("Test Player");
        PlayerHealth playerHealth = playerObject.AddComponent<PlayerHealth>();
        playerHealth.Setup(maxHealth: 100);

        // ACT
        // Deal zero damage
        playerHealth.Damage(amount: 0);

        // ASSERT
        // We expect health to remain at its starting value.
        Assert.AreEqual(100, playerHealth.CurrentHealth);
    }
}