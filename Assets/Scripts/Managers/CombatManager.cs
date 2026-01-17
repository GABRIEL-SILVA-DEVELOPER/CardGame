using DG.Tweening;
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


    public void StartCombat(Card weaponCard, Card monsterCard)
    {
        CardVisual cardVisual = weaponCard.GetCardVisual();

        cardVisual.PlayAttackAnimation(monsterCard.transform.position, (dir) =>
        {
            GameFeel.Instance.TriggerHitStop(0.1f);

            int damage = weaponCard.GetCardValue();
            int monsterHealth = monsterCard.GetCardValue();

            // TEST ==============================================
            bool isCrit = damage > 15 ? true : false;
            // TEST ==============================================
            
            GameFeel.Instance.SpawnPopupText
            (
                monsterCard.transform.position, 
                damage, 
                PopupText.PrefixType.MINUS, 
                Color.red, 
                70, 
                isCrit
            );

            monsterHealth -= damage;

            if (monsterHealth <= 0)
            {
                monsterCard.SetCardValue(0);
                monsterCard.GetCardVisual().UpdateVisual();
                
                GameFeel.Instance.SpawnDeathGhost(monsterCard, dir);
            }
            else
            {
                monsterCard.SetCardValue(monsterHealth);
                monsterCard.GetCardVisual().UpdateVisual();

                monsterCard.GetCardVisual().PlayKnockbackAnimation(dir);
            }

            Destroy(weaponCard.gameObject);
        });
    }

}