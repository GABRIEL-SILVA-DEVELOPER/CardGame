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
            TriggerHitStop(0.11f);

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

                monsterCard.GetCardVisual().PlayKnockbackAnimation(dir);
            }

            Destroy(weaponCard.gameObject);
        });
    }

    private void TriggerHitStop(float duration)
    {
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.05f, 0.01f).SetUpdate(true);
        DOVirtual.DelayedCall(duration, () => Time.timeScale = 1f).SetUpdate(true);
    }

}