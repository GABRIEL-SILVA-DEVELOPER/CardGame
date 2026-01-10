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
        int damage = weaponCard.GetCardDataValue();
        int monsterHealth = monsterCard.GetCardDataValue();

        int finalDamage = monsterHealth - damage;

        if (finalDamage <= 0)
        {
            monsterCard.SetCardDataValue(0);
            monsterCard.GetCardVisual().UpdateVisualData();

            Destroy(monsterCard.gameObject);
        }
        else
        {
            monsterCard.SetCardDataValue(finalDamage);
            monsterCard.GetCardVisual().UpdateVisualData();
        }

        Destroy(weaponCard.gameObject);
    }

}