using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { Instance = null; }


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }


    public void StartCombate(Card weaponCard, Card monsterCard)
    {
        int damage = weaponCard.GetCardValue();
        int monsterHealth = monsterCard.GetCardValue();

        monsterHealth -= damage;

        if (monsterHealth <= 0)
        {
            monsterCard.SetCardValue(0);
            monsterCard.GetCardVisual().UpdateVisual();

            Destroy(monsterCard.gameObject);
        }
        else
        {
            monsterCard.SetCardValue(monsterHealth);
            monsterCard.GetCardVisual().UpdateVisual();
        }

        Destroy(weaponCard.gameObject);
    }

}