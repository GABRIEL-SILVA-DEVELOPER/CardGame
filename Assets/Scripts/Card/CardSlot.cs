using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum SlotType { DECK, HAND, BAG, HERO }
    public SlotType type;
 
    [Header("Visual Feedback")]
    [SerializeField] private GameObject highlightObject;
    [SerializeField] private Canvas highlightCanvas;
    [SerializeField] private string UILayerName;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color combatColor;
    [SerializeField] private Color buffColor;
    [SerializeField] private Color equipWeaponColor;

    private Image highlightImage;
    private Card currentCard;

    private bool hasCard => currentCard != null;


    private void Start()
    {
        Card.OnAnyCardChangeParent += HandleGlobalCardChanged;
        Card.OnAnyCardDestroyed += HandleGlobalCardDestroyed;

        Card initialCard = GetComponentInChildren<Card>();
        if (initialCard != null) currentCard = initialCard;

        if (highlightObject == null) Debug.LogError("The visual highlight is not configured in CardSlot.cs");
        highlightImage = highlightObject.GetComponent<Image>();
        highlightObject.SetActive(false);
    }

    private void OnDisable()
    {
        Card.OnAnyCardChangeParent -= HandleGlobalCardChanged;
        Card.OnAnyCardDestroyed -= HandleGlobalCardDestroyed;
    }

    #region Event Handler

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        Card draggingCard = eventData.pointerDrag.GetComponent<Card>();

        if (draggingCard != null && IsValidMove(draggingCard))
        {
            Color targetColor = GetTargetHighlightColor(draggingCard);
            ShowHighlight(targetColor);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideHighlight();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        Card droppedCard = eventData.pointerDrag?.GetComponent<Card>();
        if (droppedCard != null && IsValidMove(droppedCard))
        {
            HandleCardDrop(droppedCard);
        }

        HideHighlight();
    }

    private void HandleGlobalCardChanged(Card card)
    {
        if (card == currentCard && card.GetParent() != this.transform)
        {
            currentCard = null;
        }
    }

    private void HandleGlobalCardDestroyed(Card card)
    {
        if (card == currentCard)
        {
            currentCard = null;
        }
    }

    #endregion


    private void HandleCardDrop(Card card)
    {
        if (hasCard && currentCard == null) currentCard = GetComponentInChildren<Card>();

        if (type == SlotType.BAG && currentCard == null)
        {
            if (card.Data.type == CardData.CardType.MONSTER) return;
            
            PlaceInSlot(card);
        }
        else if (type == SlotType.HAND && currentCard == null)
        {
            if (card.Data.type != CardData.CardType.WEAPON) return;

            PlaceInSlot(card);
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
            HeroSlot heroSlot = GetComponent<HeroSlot>();

            if (card.Data.type == CardData.CardType.MONSTER || card.Data.buffType == CardData.BuffType.HEAL)
            {
                card.GetCardVisual().RotateAndDecrease(transform, heroSlot.ConsumeCardEffect, card);
            }
        }
    }

    private void PlaceInSlot(Card card)
    {
        currentCard = card;
        card.UpdateParent(this.transform);
        card.MoveToParent();
    }

    private bool IsValidMove(Card dragCard)
    {
        CardData.CardType incomingType = dragCard.Data.type;
        CardData.BuffType incomingBuffType = dragCard.Data.buffType;

        if (type == SlotType.BAG)
            return IsStoreInBagSituation(incomingType) || IsWeaponBuffSituation(incomingType, incomingBuffType);

        if (type == SlotType.HAND)
            return IsEquipWeaponSituation(incomingType) || IsWeaponBuffSituation(incomingType, incomingBuffType);

        if (type == SlotType.DECK)
            return IsCombatSituation(incomingType) || IsWeaponBuffSituation(incomingType, incomingBuffType);

        if (type == SlotType.HERO)
            return IsHeroUnderAttackSituation(incomingType) || IsHeroUnderHealSituation(incomingType, incomingBuffType);

        return false;
    }

    #region Validations

    private bool IsCombatSituation(CardData.CardType incomingType)
    {
        return hasCard && currentCard.Data.type == CardData.CardType.MONSTER && 
            incomingType == CardData.CardType.WEAPON;
    }
    
    private bool IsWeaponBuffSituation(CardData.CardType incomingType, CardData.BuffType incomingBuffType)
    {
        return hasCard && currentCard.Data.type == CardData.CardType.WEAPON &&  
            incomingType == CardData.CardType.BUFF && incomingBuffType == CardData.BuffType.WEAPON_BUFF;
    }

    private bool IsEquipWeaponSituation(CardData.CardType incomingType)
    {
        return !hasCard && incomingType == CardData.CardType.WEAPON;
    }

    private bool IsStoreInBagSituation(CardData.CardType incomingType)
    {
        return !hasCard && incomingType != CardData.CardType.MONSTER;
    }
    
    private bool IsHeroUnderAttackSituation(CardData.CardType incomingType)
    {
        return incomingType == CardData.CardType.MONSTER;
    }

    private bool IsHeroUnderHealSituation(CardData.CardType incomingType, CardData.BuffType incomingBuffType)
    {
        return incomingType == CardData.CardType.BUFF && incomingBuffType == CardData.BuffType.HEAL;
    }

    #endregion

    private void ShowHighlight(Color targetColor)
    {
        highlightObject.SetActive(true);
        highlightImage.color = targetColor;
        
        if (highlightCanvas != null)
        {
            highlightCanvas.overrideSorting = true;
            highlightCanvas.sortingOrder = 99;
        }

        highlightImage.transform.DOKill();

        // Pop
        highlightObject.transform.localScale = Vector3.one * 0.5f;
        highlightObject.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void HideHighlight()
    {
        // Fade out
        highlightImage.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack)
            .OnComplete(() => highlightObject.SetActive(false))
            .SetUpdate(true);
    }

    private Color GetTargetHighlightColor(Card dragCard)
    {
        CardData.CardType incomingType = dragCard.Data.type;
        CardData.BuffType incomingBuffType = dragCard.Data.buffType;

        if (type == SlotType.BAG)
        {
            if (IsStoreInBagSituation(incomingType)) return normalColor;
            else if (IsWeaponBuffSituation(incomingType, incomingBuffType)) return buffColor;
        }
        else if (type == SlotType.HAND)
        {
            if (IsEquipWeaponSituation(incomingType)) return equipWeaponColor;
            else if (IsWeaponBuffSituation(incomingType, incomingBuffType)) return buffColor;
        }
        else if (type == SlotType.DECK)
        {
            if (IsCombatSituation(incomingType)) return combatColor;
            else if (IsWeaponBuffSituation(incomingType, incomingBuffType)) return buffColor;
        }
        else if (type == SlotType.HERO)
        {
            if (IsHeroUnderAttackSituation(incomingType)) return combatColor;
            else if (IsHeroUnderHealSituation(incomingType, incomingBuffType)) return buffColor;
        }

        return normalColor;
    }

    public void PlayImpactEffect()
    {
        // transform.DOKill();
        transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0f), 0.15f).SetUpdate(true);
    }

}