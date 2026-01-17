using DG.Tweening;
using TMPro;
using UnityEngine;

public class PopupText : MonoBehaviour
{
    public enum PrefixType { PLUS, MINUS, NONE }

    [Header("Setup")]
    [SerializeField] private TextMeshProUGUI popupText;
    [SerializeField] private string VFXLayerName;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Canvas canvas;


    public void Setup(Vector3 position, int fontSize, PrefixType type, Color color, int valueText, bool isCritical)
    {
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingLayerName = VFXLayerName;
            canvas.sortingOrder = 999;  
        }

        transform.position = position;
        popupText.fontSize = fontSize;
        popupText.color = color;
        
        string prefix = "";
        if (type == PrefixType.MINUS)
        {
            prefix = "-";
        }
        else if (type == PrefixType.PLUS)
        {
            prefix = "+";
        }

        popupText.text = $"{prefix}{valueText}";

        PlayFadeAnimation(isCritical);
    }

    public void Setup(Vector3 position, int fontSize, Color color, string strText)
    {
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingLayerName = VFXLayerName;
            canvas.sortingOrder = 999;  
        }

        transform.position = position;
        popupText.fontSize = fontSize;
        popupText.color = color;
        
        popupText.text = strText;

        PlayFadeAnimation(false);
    }


    private void PlayFadeAnimation(bool isCritical)
    {
        transform.DOKill();

        Sequence popupSeq = DOTween.Sequence().SetUpdate(true);

        transform.localScale = Vector3.zero;
        float duration = 1.0f;

        if (isCritical)
        {
            // Shake
            popupSeq.Append(transform.DOShakePosition(duration * 0.5f, 20.0f, 50));

            // Scale
            popupSeq.Join(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));
        }
        else
        {
            // Scale
            popupSeq.Append(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));
        }

        float height = 1.3f;
        popupSeq.Join(transform.DOMoveY(transform.position.y + height, duration).SetEase(Ease.OutCubic));

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            popupSeq.Insert(duration * 0.5f, canvasGroup.DOFade(0, duration * 0.5f));
        }

        popupSeq.OnComplete(() => Destroy(gameObject));
    }
}