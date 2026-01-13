using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Hearts sprites")]
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [Header("Animation Settings")]
    [SerializeField] private float bobAmount = 10f;
    [SerializeField] private float bobDuration = 1.0f;
    [SerializeField] private float delayBetweenHearts = 0.15f;


    private List<Image> hearts = new();


    private void Awake()
    {
        foreach(Transform child in transform)
        {
            Image img = child.GetComponentInChildren<Image>();
            if (img != null)
            {
                hearts.Add(img);
            }
        }
    }

    private void Start()
    {
        HeroManager.OnHealthChanged += UpdateHearts;

        foreach (var heart in hearts)
        {
            heart.material = new Material(heart.material);
        }

        StartFloatingAnimation();
    }

    private void OnDisable()
    {
        HeroManager.OnHealthChanged -= UpdateHearts;

        foreach(var heart in hearts)
        {
            heart.transform.parent.DOKill();
            heart.transform.DOKill();
        }
    }


    private void UpdateHearts(int currentHealth, int maxHealth)
    {
        string flashAmount = "_FlashAmount";
        string pulseID = "LastHeartPulse";
        int heartsToPulse = 3;

        DOTween.Kill(pulseID);

        for(int i = 0; i < hearts.Count; i++)
        {
            Material heartMat = hearts[i].material;

            if (currentHealth <= heartsToPulse && i <= heartsToPulse)
            {
                heartMat.DOFloat(1.0f, flashAmount, 0.4f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetId(pulseID);
            }
            else
            {
                heartMat.SetFloat(flashAmount, 0f);
            }


            if (i < currentHealth)
            {
                if (hearts[i].sprite != fullHeart)
                {
                    heartMat.SetFloat(flashAmount, 0f);

                    hearts[i].sprite = fullHeart;
                    hearts[i].color = Color.white;
                    hearts[i].transform.DOPunchScale(Vector3.one * 3.0f, 0.2f);
                }
            }
            else
            {
                if (hearts[i].sprite == fullHeart)
                {
                    //Color fadeIn/fadeOut
                    heartMat.DOKill();
                    heartMat.SetFloat(flashAmount, 1.0f);
                    heartMat.DOFloat(0f, flashAmount, 0.6f);

                    //Animation
                    float duration = 0.5f;
                    float randomPosX = Random.Range(-20f, 20f);
                    float randomPosY = Random.Range(-40f, 40f);
                    hearts[i].transform.DOPunchPosition(new Vector3(randomPosX, randomPosY, 0f), duration);
                    
                    float randomRotation = Random.Range(-20f, 20f);
                    hearts[i].transform.DOPunchRotation(new Vector3(0f, 0f, randomRotation), duration);

                    //Set sprite
                    hearts[i].sprite = emptyHeart;
                    // hearts[i].DOFade(0.1f, 0.2f);
                }
                else
                {
                    hearts[i].sprite = emptyHeart;
                    hearts[i].color = new Color(1, 1, 1, 0.5f);
                }
            }
        }
    }

    private void StartFloatingAnimation()
    {
        for(int i = 0; i < hearts.Count; i++)
        {
            Transform parentTransform = hearts[i].transform.parent;

            parentTransform.DOLocalMoveY(bobAmount, bobDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(i * delayBetweenHearts)
                .SetRelative(true);
        }
    }

}