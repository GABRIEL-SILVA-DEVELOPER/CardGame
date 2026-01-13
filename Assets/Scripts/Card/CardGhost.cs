using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardGhost : MonoBehaviour
{
    [Header("Visuals Settings")]
    [SerializeField] private Image mainIcon;
    [SerializeField] private Image shadowMainIcon;
    [SerializeField] private Image valueIcon;
    [SerializeField] private Image shadowValueIcon;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image flashOverlayImage;
    [SerializeField] private Color flashColor;

    public void Setup(Sprite mainIcon, Sprite valueIcon, Vector3 direction)
    {
        this.mainIcon.sprite = mainIcon;
        this.shadowMainIcon.sprite = mainIcon; 
        this.valueIcon.sprite = valueIcon;
        this.shadowValueIcon.sprite = valueIcon;
        this.valueText.text = "0";

        PlayAnimation(direction);
    }

    private void PlayAnimation(Vector3 direction)
    {
        TriggerFlash();

        Sequence deadSeq = DOTween.Sequence();

        deadSeq.Append(transform.DOMove(transform.position + (direction * 25f), 2.0f).SetEase(Ease.OutCubic));
        deadSeq.Join(transform.DORotate(new Vector3(0, 0, 720), 1.5f, RotateMode.FastBeyond360).SetEase(Ease.OutQuart));
        deadSeq.Join(transform.DOScale(Vector3.one * 5f, 0.5f));
        deadSeq.OnComplete(() => Destroy(gameObject));
    }

    private void TriggerFlash()
    {
        if (flashOverlayImage == null) return;

        flashOverlayImage.DOKill();
        flashOverlayImage.color = flashColor;
        flashOverlayImage.DOFade(0f, 0.2f).SetUpdate(true);
    }

}