using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float followSpeed = 40.0f;
    [Header("Rotation Settings")]
    [SerializeField] private float rotationAmount = 45.0f;
    [SerializeField] private float rotationSpeed  = 45.0f;
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
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private GameObject shadowObject; 
    [Header("Canvas Settings")]
    [SerializeField] private int defaultSortingOrder  = 05;
    [SerializeField] private int draggingSortingOrder = 15;

    private Canvas canvas;
    private Card parentCard;
    private Transform cardTransform;

    private bool isInitialize;


    public void Initialize(Card card)
    {
        // Internal references

        canvas = GetComponent<Canvas>();
        if (canvas == null) Debug.LogError("CardVisual.cs could not find the 'Canvas' component.");
        canvas.overrideSorting = true;
        canvas.sortingOrder = defaultSortingOrder;

        if (shadowObject != null) shadowObject.SetActive(false);

        // External references

        parentCard = card;
        cardTransform = parentCard.transform;

        parentCard.BeginDragEvent.AddListener(CardBeginDrag);
        parentCard.EndDragEvent.AddListener(CardEndDrag);
        parentCard.PointerEnterEvent.AddListener(CardPointerEnter);
        parentCard.PointerExitEvent.AddListener(CardPointerExit);

        UpdateVisualData();

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

    public void UpdateVisualData()
    {
        if (parentCard == null) return;

        mainIcon.sprite = parentCard.Data.mainIcon;
        shadowMainIcon.sprite = parentCard.Data.mainIcon; 
        valueIcon.sprite = parentCard.Data.cardValueIcon;
        valueText.text = parentCard.GetCardDataValue().ToString();
    }

    #region Event Handler

    private void CardBeginDrag(Card card)
    {
        if (shadowObject != null) shadowObject.SetActive(true);

        canvas.sortingOrder = draggingSortingOrder;
        transform.DOScale(1.2f, 0.2f);
    }

    private void CardEndDrag(Card card)
    {
        if (shadowObject != null) shadowObject.SetActive(false);

        canvas.sortingOrder = defaultSortingOrder;
        transform.DOScale(1.0f, 0.2f);
    }

    private void CardPointerEnter(Card card)
    {
        transform.DOScale(1.1f, 0.2f);
    }

    private void CardPointerExit(Card card)
    {
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


    private void SmoothFollow()
    {
        transform.position = Vector3.Lerp(transform.position, cardTransform.position, followSpeed * Time.deltaTime);
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

}