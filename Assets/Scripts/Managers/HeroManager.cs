using System;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    public static HeroManager Instance { get; private set; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { Instance = null; }


    public static Action<int, int> OnHealthChanged;

    public int GetMaxHealth => maxHealth;
    public int GetCurrentHealth => currentHealth;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);

        currentHealth = maxHealth;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }


    public void ReciveDamage(int amount)
    {
        currentHealth -= amount;
                
        if (currentHealth < 0)
        {
            currentHealth = 0;
            Die();
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Game over!");
    }

}