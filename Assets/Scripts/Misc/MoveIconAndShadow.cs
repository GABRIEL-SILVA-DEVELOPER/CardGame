using UnityEngine;
using UnityEngine.UI;

public class MoveIconAndShadow : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image shadowImage;
    [Header("Bob animation")]
    [SerializeField] private float bobIntensity = 15f;
    [SerializeField] private float minBobSpeed = 2.0f;
    [SerializeField] private float maxBobSpeed = 3.0f;
    [SerializeField] private float minShadowScale = 0.5f;
    [SerializeField] private float minShadowAlpha = 0.2f;

    private float bobSpeed;
    private Vector3 initialIconPos;
    private Vector3 initialShadowScale;


    private void Awake()
    {
        if (iconImage == null) Debug.LogError("Slot without icon image -> MoveIconAndShadow.cs");

        shadowImage.sprite = iconImage.sprite;
        shadowImage.color  = new Color(0, 0, 0, 0.5f);
 
        initialIconPos = iconImage.transform.localPosition;
        initialShadowScale = shadowImage.transform.localScale;

        bobSpeed = Random.Range(minBobSpeed, maxBobSpeed);
    }


    private void Update()
    {
        PlayBobAnim();
    }

    private void PlayBobAnim()
    {
        float sinValue = Mathf.Sin(Time.time * bobSpeed);

        float newY = initialIconPos.y + (sinValue * bobIntensity);
        iconImage.transform.localPosition = new Vector3(iconImage.transform.localPosition.x, newY, iconImage.transform.localPosition.z);

        float scaleMultiplier = Mathf.Lerp(1.0f, minShadowScale, (sinValue + 1f) / 2f);
        shadowImage.transform.localScale = initialShadowScale * scaleMultiplier;

        Color shadowColor = shadowImage.color;
        shadowColor.a = Mathf.Lerp(0.5f, minShadowAlpha, (sinValue + 1f) / 2f);
        shadowImage.color = shadowColor;
    }

}