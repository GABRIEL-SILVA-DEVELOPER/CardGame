using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour, IDropHandler
{
    public enum SlotType { DECK, HAND, BAG, HERO }
    public SlotType type;
 
    [Header("Visuals")]
    [SerializeField] private Image slotHighlighterImage;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color combatColor;
    [SerializeField] private Color equipmentColor;

    private Card currentCard;


    private void Start()
    {
        Card.OnAnyCardChangeParent += CheckCardChangeSlot;
        Card.OnAnyCardDestroyed += CheckCardDestroyed;

        Card initialCard = GetComponentInChildren<Card>();
        if (initialCard != null) currentCard = initialCard;
    }

    private void OnDisable()
    {
        Card.OnAnyCardChangeParent -= CheckCardChangeSlot;
        Card.OnAnyCardDestroyed -= CheckCardDestroyed;
    }

    #region Event Handler

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        Card droppedCard = eventData.pointerDrag?.GetComponent<Card>();

        if (droppedCard != null)
        {
            HandleCardDrop(droppedCard);
        }
    }

    private void CheckCardChangeSlot(Card card)
    {
        if (card == currentCard && card.GetParent() != this.transform)
        {
            currentCard = null;
        }
    }

    private void CheckCardDestroyed(Card card)
    {
        if (card == currentCard)
        {
            currentCard = null;
        }
    }

    #endregion


    private void HandleCardDrop(Card card)
    {
        if (HasCard() && currentCard == null)
        {
            currentCard = GetComponentInChildren<Card>();
        }

        if (type == SlotType.BAG && currentCard == null)
        {
            if (card.Data.type == CardData.CardType.MONSTER) return;
            
            PlaceInSlot(card);
            Debug.Log("Card on BAG!");
        }
        else if (type == SlotType.HAND && currentCard == null)
        {
            if (card.Data.type != CardData.CardType.WEAPON) return;

            PlaceInSlot(card);
            Debug.Log("Card on HAND!");
        }
        else if (type == SlotType.DECK && currentCard != null)
        {
            if (currentCard.Data.type == CardData.CardType.MONSTER && card.Data.type == CardData.CardType.WEAPON)
            {
                CombatManager.Instance.StartCombat(card, currentCard);
            }
        }
        else if (type == SlotType.HERO)
        {
            if (card.Data.type == CardData.CardType.MONSTER)
            {
                HeroManager.Instance.ReciveDamage(card.GetCardValue());
                Destroy(card.gameObject);
            }
            else if (card.Data.type == CardData.CardType.BUFF)
            {
                if (card.Data.buffType == CardData.BuffType.HEAL)
                {
                    HeroManager.Instance.Heal(card.GetCardValue());
                    Destroy(card.gameObject);
                }
            }
        }

    }

    private void PlaceInSlot(Card card)
    {
        currentCard = card;
        card.UpdateParent(this.transform);
        card.MoveToParent();
    }

    private bool HasCard()
    {
        return GetComponentInChildren<Card>() != null;
    }

}