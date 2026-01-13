using DG.Tweening;
using UnityEditor.SearchService;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { Instance = null; }
    [Header("Game Canvas")]
    [SerializeField] private Canvas canvas;
    [Header("Popup")]
    [SerializeField] private GameObject popupTextObject;
    [Header("VFX Settings")]
    [SerializeField] private GameObject cardGhostObject;


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
            TriggerHitStop(0.1f);

            int damage = weaponCard.GetCardValue();
            int monsterHealth = monsterCard.GetCardValue();

            SpawnPopupText(monsterCard.transform.position, damage);

            monsterHealth -= damage;

            if (monsterHealth <= 0)
            {
                monsterCard.SetCardValue(0);
                monsterCard.GetCardVisual().UpdateVisual();
                
                SpawnDeathGhost(monsterCard, dir);
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

    private void SpawnDeathGhost(Card deadCard, Vector3 direction)
    {
        if (cardGhostObject != null)
        {
            GameObject ghostObj = Instantiate(cardGhostObject, deadCard.transform.parent);
            ghostObj.transform.position = deadCard.transform.position;

            CardGhost ghostScript = ghostObj.GetComponent<CardGhost>();
            ghostScript.Setup(deadCard.Data.mainIcon, deadCard.Data.cardValueIcon, direction);
        }

        Destroy(deadCard.gameObject);
    }

    private void SpawnPopupText(Vector3 position, int value)
    {
        if (popupTextObject == null || canvas == null) return;
        
        Vector3 spawnPos = position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-0.5f, 0.5f), 0);
        GameObject popupObj = Instantiate(popupTextObject, canvas.transform);
        PopupText popup = popupObj.GetComponent<PopupText>();

        // TEST ================================================================
        bool isCrit = value >= 15; 
        Color color = isCrit ? Color.yellow : Color.red;
        int fontSize = isCrit ? 80 : 70;
        // TEST ================================================================

        popup.Setup(spawnPos, fontSize, PopupText.PrefixType.MINUS, color, value, isCrit);
    }

}