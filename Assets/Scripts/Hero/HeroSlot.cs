using DG.Tweening;
using UnityEngine;

public class HeroSlot : MonoBehaviour
{
    [Header("Magnetic Settings")]
    [SerializeField] private float ruptureRadius = 200f;

    private CardVisual targetCard;
    private bool isMonitoring;


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

    private void OnBeginDragGlobal(CardVisual cardOver)
    {
        isMonitoring = true;
        targetCard = cardOver;
    }

    private void OnEndDragGlobal()
    {
        isMonitoring = false;
        if (targetCard == null) return;

        targetCard.transform.DOLocalMove(Vector3.zero, 0.25f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => targetCard = null);
    }


    private void Update()
    {
        if (!isMonitoring || targetCard == null) return;

        ApplyMagneticEffect();
    }


    private void ApplyMagneticEffect()
    {
        // TODO
    }

}