using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HeroSlot : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool DrawRuptureRadius = false;
    [Header("Magnetic Settings")]
    [SerializeField] private float ruptureRadius = 200f;
    [Header("Visual Settings")]
    [SerializeField] private Image heroIcon;

    private CardVisual targetCard;
    private bool trackingVisualCard;


    private void Start()
    {
        Card.OnBeginDragGlobal += OnBeginDragGlobal;
        Card.OnEndDragGlobal += OnEndDragGlobal; 
    }

    private void OnDisable()
    {
        Card.OnBeginDragGlobal -= OnBeginDragGlobal;
        Card.OnEndDragGlobal -= OnEndDragGlobal;
    }

    #region Event Handler

    private void OnBeginDragGlobal(CardVisual cardOver)
    {
        trackingVisualCard = true;
        targetCard = cardOver;
    }

    private void OnEndDragGlobal()
    {
        trackingVisualCard = false;
        if (targetCard == null) return;

        if(!DOTween.IsTweening(targetCard.transform)) {
            targetCard.transform.DOLocalMove(Vector3.zero, 0.25f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => targetCard = null);
        }
    }

    #endregion


    private void Update()
    {
        if (!trackingVisualCard || targetCard == null || !IsValidMoment(targetCard.GetParentCard.Data)) return;

        ApplyMagneticEffect();
    }

    private bool IsValidMoment(CardData cardData)
    {
        bool correctType = cardData.type == CardData.CardType.BUFF && cardData.buffType == CardData.BuffType.HEAL;
        bool heroNeedLife = HeroManager.Instance.GetCurrentHealth < HeroManager.Instance.GetMaxHealth;

        return correctType && heroNeedLife;
    }

    private void ApplyMagneticEffect()
    {
        Vector2 slotPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 cardParentPos = new Vector2(targetCard.GetParentCard.transform.position.x, targetCard.GetParentCard.transform.position.y);

        float distance = Vector2.Distance(slotPos, cardParentPos);
        float intensity = Mathf.Clamp01(1 - (distance / ruptureRadius));

        Vector2 targetPos = Vector2.Lerp(cardParentPos, slotPos, intensity);

        if (intensity > 0)
        {
            targetCard.transform.position = Vector3.Lerp(targetCard.transform.position, targetPos, 100f * Time.deltaTime);
        }
    }


    public void ConsumeCardEffect(Card card)
    {
        bool isPositive = card.Data.buffType == CardData.BuffType.HEAL ? true : false;
        
        // Text popup
        Color popupColor = isPositive ? Color.green : Color.red;
        PopupText.PrefixType prefix = isPositive ? PopupText.PrefixType.PLUS : PopupText.PrefixType.MINUS;
        GameFeel.Instance.SpawnPopupText(transform.position, card.GetCardValue(), prefix, popupColor, 80, false);
        // Particles
        GameFeel.Instance.SpawnParticles(transform.position, isPositive);
        // Hit stop
        GameFeel.Instance.TriggerHitStop(0.05f);


        // Icon visual feedback 
        heroIcon.DOColor(isPositive ? Color.green : Color.red, 0.1f).SetLoops(2, LoopType.Yoyo);

        // Transform animations
        if (isPositive) PlayBuffAnim(card.GetCardValue());
        else PlayDebuffAnim(card.GetCardValue());
    }

    private void PlayBuffAnim(int value)
    {
        HeroManager.Instance.Heal(value);

        GameFeel.Instance.TriggerScreenShake(0.2f, Vector3.up);

        transform.DOKill();
        Sequence buffSeq = DOTween.Sequence().SetUpdate(true);
        // Scale up
        buffSeq.Append(transform.DOScale(Vector3.one * 1.3f, 0.05f).SetEase(Ease.OutQuad));
        // Reset
        buffSeq.Append(transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutElastic));
    }

    private void PlayDebuffAnim(int value)
    {
        HeroManager.Instance.ReciveDamage(value);

        GameFeel.Instance.TriggerScreenShake(0.3f, Vector3.down);

        transform.DOKill();
        Sequence debuffSeq = DOTween.Sequence().SetUpdate(true);
        Vector3 originalPos = transform.localPosition;
        // Squash
        debuffSeq.Append(transform.DOLocalMoveY(originalPos.y - 80f, 0.05f).SetEase(Ease.OutQuad));
        debuffSeq.Join(transform.DOScale(new Vector3(1.2f, 0.7f, 1.0f), 0.05f).SetEase(Ease.OutQuad));
        // Reset
        debuffSeq.Append(transform.DOLocalMoveY(originalPos.y, 0.15f).SetEase(Ease.OutBack));
        debuffSeq.Join(transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack));
    }


    // DEBUG ==================================================
    private void OnDrawGizmos()
    {
        if (!DrawRuptureRadius) return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, ruptureRadius);
    }

}