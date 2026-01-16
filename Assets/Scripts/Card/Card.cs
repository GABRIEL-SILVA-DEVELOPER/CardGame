using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Card : MonoBehaviour , IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Events")]
    [HideInInspector] public static Action<CardVisual> OnBeginDragGlobal;
    [HideInInspector] public static Action OnEndDragGlobal;
    [HideInInspector] public static Action<Card> OnAnyCardChangeParent;
    [HideInInspector] public static Action<Card> OnAnyCardDestroyed;
    [HideInInspector] public UnityEvent<Card> PointerEnterEvent;
    [HideInInspector] public UnityEvent<Card> PointerExitEvent;
    [HideInInspector] public UnityEvent<Card> BeginDragEvent;
    [HideInInspector] public UnityEvent<Card> EndDragEvent;
    [Header("Card Data Settings")]
    [SerializeField] private CardData data;
    private int cardValue;
    [Header("Clamp Settings")]
    [SerializeField] private float horizontalPadding = 0.5f;
    [SerializeField] private float verticalPadding = 1.0f;
    [Header("Card Visual")]
    [SerializeField] private CardVisual cardVisualPrefab;
    private CardVisual visual;

    private Vector3 offset;
    private Image imageComponent;
    private Canvas canvas;

    private bool isHovering;
    private bool isDragging;


    private void Awake()
    {
        imageComponent = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();

        if (data != null) cardValue = data.cardValue;
    }

    private void Start()
    {
        visual = Instantiate(cardVisualPrefab, transform.parent);
        visual.Initialize(this);
    }

    private void OnDestroy()
    {
        if (visual != null)
        {
            Destroy(visual.gameObject);
        }

        OnAnyCardDestroyed?.Invoke(this);
    }


    #region Event Handler

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        isDragging = true;

        offset = transform.position - MousePosition();
        imageComponent.raycastTarget = false;

        BeginDragEvent?.Invoke(this);
        OnBeginDragGlobal?.Invoke(visual);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        transform.position = MousePosition() + offset;
        ClampPosition();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        isDragging = false;

        MoveToParent();
        imageComponent.raycastTarget = true;

        EndDragEvent?.Invoke(this);
        OnEndDragGlobal?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        PointerEnterEvent?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        PointerExitEvent?.Invoke(this);
    }

    #endregion


    #region Internal Methods

    private Vector3 MousePosition()
    {
        Vector3 mousePos = Mouse.GetMousePosition();
        mousePos.z = canvas.planeDistance;

        return mousePos;
    }

    private void ClampPosition()
    {
        Vector3 limits = new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z);
        Vector3 screenBounds = Camera.main.ScreenToWorldPoint(limits);

        Vector3 clampedPosition = transform.position;

        float minX = -screenBounds.x + horizontalPadding;
        float maxX = screenBounds.x  - horizontalPadding;
        float minY = -screenBounds.y + verticalPadding;
        float maxY = screenBounds.y  - verticalPadding;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        clampedPosition.z = 0;

        transform.position = clampedPosition;
    }

    #endregion

    #region Public Methods

    public void MoveToParent()
    {
        transform.localPosition = Vector3.zero;
        if (visual != null) visual.PrepareForImpact();
    }

    public void UpdateParent(Transform newParent)
    {
        transform.SetParent(newParent);
        if (visual != null) visual.UpdateParent(newParent);

        OnAnyCardChangeParent?.Invoke(this);
    }

    public CardVisual GetCardVisual()
    {
        return visual;
    }

    public int GetCardValue()
    {
        return cardValue;
    }

    public void SetCardValue(int newValue)
    {
        cardValue = newValue;
    }

    public Transform GetParent()
    {
        return transform.parent;
    }

    public CardData Data => data;

    public bool IsDragging => isDragging;

    public bool IsHovering => isHovering;

    #endregion

}