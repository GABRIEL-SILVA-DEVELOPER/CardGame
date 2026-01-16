using System;
using DG.Tweening;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float followSpeed = 40f;
    [Header("Rotation Settings")]
    [SerializeField] private float rotationAmount = 45f;
    [SerializeField] private float rotationSpeed  = 45f;
    private Vector3 movementDelta;
    private Vector3 rotationDelta;
    [Header("Tilt Settings")]
    [SerializeField] private float autoTiltAmount = 5f;
    [SerializeField] private float manualTiltAmount = 20f;
    [SerializeField] private float tiltSpeed = 20f;
    [SerializeField] private Transform tiltPivot;
    [Header("Visual Settings")]
    [SerializeField] private Image mainIcon;
    [SerializeField] private Image shadowMainIcon;
    [SerializeField] private Image valueIcon;
    [SerializeField] private Image valueIconShadow;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private GameObject shadowObject;
    [SerializeField] private Image flashOverlayImage;
    [Header("Canvas Settings")]
    [SerializeField] private int defaultSortingOrder  = 05;
    [SerializeField] private int draggingSortingOrder = 15;
    [Header("VFX")]
    [SerializeField] private ParticleSystem bloodParticles;

    private Canvas canvas;

    private Card parentCard;
    private Transform cardTransform;

    private bool waitingForImpact = false;
    private bool isInitialize;


    public void Initialize(Card card)
    {
        canvas = GetComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = defaultSortingOrder;

        if (shadowObject != null) shadowObject.SetActive(false);

        parentCard = card;
        cardTransform = parentCard.transform;

        parentCard.BeginDragEvent.AddListener(CardBeginDrag);
        parentCard.EndDragEvent.AddListener(CardEndDrag);
        parentCard.PointerEnterEvent.AddListener(CardPointerEnter);
        parentCard.PointerExitEvent.AddListener(CardPointerExit);

        UpdateVisual();

        isInitialize = true;
    }

    private void OnDestroy()
    {
        transform.DOKill();
        
        if (parentCard != null)
        {
            parentCard.BeginDragEvent.RemoveListener(CardBeginDrag);
            parentCard.EndDragEvent.RemoveListener(CardEndDrag);
            parentCard.PointerEnterEvent.RemoveListener(CardPointerEnter);
            parentCard.PointerExitEvent.RemoveListener(CardPointerExit);
        }
    }

    public void UpdateVisual()
    {
        if (parentCard == null) return;

        mainIcon.sprite = parentCard.Data.mainIcon;
        shadowMainIcon.sprite = parentCard.Data.mainIcon; 
        valueIcon.sprite = parentCard.Data.cardValueIcon;
        valueIconShadow.sprite = valueIcon.sprite;
        valueText.text = parentCard.GetCardValue().ToString();
    }

    #region Event Handler

    private void CardBeginDrag(Card card)
    {
        if (shadowObject != null) shadowObject.SetActive(true);
        canvas.sortingOrder = draggingSortingOrder;

        transform.DOKill();
        transform.DOScale(1.2f, 0.2f);
    }

    private void CardEndDrag(Card card)
    {
        if (shadowObject != null) shadowObject.SetActive(false);
        canvas.sortingOrder = defaultSortingOrder;

        transform.DOScale(Vector3.one * 1.0f, 0.2f);
    }

    private void CardPointerEnter(Card card)
    {
        if (parentCard.IsDragging) return;

        transform.DOKill();
        transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack);
    }

    private void CardPointerExit(Card card)
    {
        if (card.IsDragging) return;

        transform.DOKill();
        tiltPivot.DOKill();
        transform.DOScale(1.0f, 0.2f);
    }

    #endregion


    private void Update()
    {
        if (parentCard == null || !isInitialize) return;

        SmoothFollow();
        SmoothRotation();
        UpdateShadow();
        UpdateTilt();
    }

    public void UpdateParent(Transform newParent)
    {
        transform.SetParent(newParent);
    }

    #region Card Movements

    private void SmoothFollow()
    {
        transform.position = Vector3.Lerp(transform.position, cardTransform.position, followSpeed * Time.deltaTime);

        if (waitingForImpact)
        {
            float distance = Vector3.Distance(transform.position, cardTransform.position);

            if (distance < 1.5f)
            {
                ExecuteImpact();
            }
        }
    }

    private void SmoothRotation()
    {
        Vector3 diff = transform.position - cardTransform.position;

        float movementDeltaSpeed = 50.0f;
        movementDelta = Vector3.Lerp(movementDelta, diff, movementDeltaSpeed * Time.deltaTime);

        Vector3 movementRotation = movementDelta * rotationAmount;
        rotationDelta = Vector3.Lerp(rotationDelta, movementRotation, rotationSpeed * Time.deltaTime);

        transform.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Clamp(rotationDelta.x, -45.0f, 45.0f));
    }

    private void UpdateShadow()
    {
        if (parentCard.IsDragging && shadowObject != null)
        {
            float shadowOffset = 0.7f;
            shadowObject.transform.localPosition = 
                new Vector3(rotationDelta.x * shadowOffset, (rotationDelta.y - 20.0f) * shadowOffset, 0.0f);
        }
    }

    private void UpdateTilt()
    {
        float sine = Mathf.Sign(Time.time);
        float cosine = Mathf.Cos(Time.time);

        // Reduce the tilt effect if hovering
        float tiltEffectMultiplier = parentCard.IsHovering ? 0.1f : 1.0f;

        float tiltX = 0.0f;
        float tiltY = 0.0f;

        if (parentCard.IsHovering)
        {
            Vector3 mousePos = Mouse.GetMousePosition();
            Vector3 diff = mousePos - transform.position;

            tiltX = diff.y * manualTiltAmount;
            // Invert the diff.y so that when you hover the mouse over the top, the card tilts backward
            tiltY = -diff.x * manualTiltAmount;
        }

        float targetX = tiltX + (sine * autoTiltAmount * tiltEffectMultiplier);
        float targetY = tiltY + (cosine * autoTiltAmount * tiltEffectMultiplier);

        float lerpX = Mathf.LerpAngle(tiltPivot.localEulerAngles.x, targetX, tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(tiltPivot.localEulerAngles.y, targetY, tiltSpeed * Time.deltaTime);

        tiltPivot.localEulerAngles = new Vector3(lerpX, lerpY, 0);
    }

    #endregion

    private void ExecuteImpact()
    {
        waitingForImpact = false;

        transform.DOKill();
        transform.DOPunchScale(new Vector3(0.2f, -0.2f, 0f), 0.3f).SetUpdate(true);

        CardSlot slot = GetComponentInParent<CardSlot>();
        if (slot)
        {
            slot.PlayImpactEffect();
        }
    }

    public void PrepareForImpact()
    {
        waitingForImpact = true;
    }


    public void PlayAttackAnimation(Vector2 targetPos, Action<Vector3> onHitCallback)
    {
        isInitialize = false;

        Vector2 startPos = transform.position;
        Vector2 direction = (targetPos - startPos).normalized;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        Sequence combatSequence = DOTween.Sequence();

        float distance = 2.5f;

        // Antecipation
        combatSequence.Append(transform.DORotate(new Vector3(0, 0, targetAngle), 0.2f).SetEase(Ease.OutCubic));
        combatSequence.Join(transform.DOMove(startPos - (direction * distance), 0.2f));

        // Attack
        combatSequence.Insert(0.2f, transform.DOPunchScale(new Vector3(0.1f, 0.2f, 0), 0.1f));
        combatSequence.Append(transform.DOMove(targetPos, 0.15f).SetEase(Ease.InBack))
            .OnComplete(() => onHitCallback?.Invoke(direction));
    }

    public void PlayKnockbackAnimation(Vector3 attackDirection)
    {
        isInitialize = false;
        canvas.sortingOrder = draggingSortingOrder;

        TriggerFlash();
        PlayBloodVFX(attackDirection);

        Sequence damageSeq = DOTween.Sequence();

        // Change color
        damageSeq.Join(mainIcon.DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo));

        // Knockback
        float knockbackForce = 1f;
        Vector3 targetPos = transform.position + (attackDirection * knockbackForce);
        targetPos.z = 0f;

        damageSeq.Join(transform.DOMove(targetPos, 0.1f));

        // Shake rotation
        damageSeq.Join(transform.DOShakeRotation(0.3f, new Vector3(0, 0, 20f), 15));

        // Reset
        damageSeq.OnComplete(() =>
        {
            canvas.sortingOrder = defaultSortingOrder;

            isInitialize = true;
            canvas.sortingOrder = defaultSortingOrder;

            mainIcon.color = Color.white;
        });
    }
    
    private void TriggerFlash()
    {
        if (flashOverlayImage == null) return;

        flashOverlayImage.DOKill();
        flashOverlayImage.color = new Color(1f, 1f, 1f, 1f);
        flashOverlayImage.DOFade(0f, 0.2f).SetUpdate(true);
    }

    private void PlayBloodVFX(Vector3 lookDir)
    {
        if (bloodParticles == null) return;

        bloodParticles.transform.rotation = Quaternion.LookRotation(lookDir);
        bloodParticles.Play();
    }

}