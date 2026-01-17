using DG.Tweening;
using UnityEngine;

public class GameFeel : MonoBehaviour
{
    public static GameFeel Instance { get; private set; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { Instance = null; }


    [Header("Game Canvas")]
    [SerializeField] private Canvas canvas;
    [Header("Popup")]
    [SerializeField] private GameObject popupTextObject;
    [Header("VFX Settings")]
    [SerializeField] private GameObject cardGhostVFX;
    [SerializeField] private GameObject healVFX;
    [SerializeField] private GameObject bloodVFX;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }


    public void TriggerHitStop(float duration)
    {
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.05f, 0.01f).SetUpdate(true);
        DOVirtual.DelayedCall(duration, () => Time.timeScale = 1f).SetUpdate(true);
    }

    public void SpawnPopupText(Vector3 position, int value, PopupText.PrefixType prefixType, 
        Color color, int fontSize, bool isCrit)
    {
        if (popupTextObject == null || canvas == null) return;
        
        Vector3 spawnPos = position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-0.5f, 0.5f), 0);
        GameObject popupObj = Instantiate(popupTextObject, canvas.transform);
        PopupText popup = popupObj.GetComponent<PopupText>();

        color = isCrit ? Color.yellow : color;
        fontSize = isCrit ? fontSize + 10 : fontSize;

        popup.Setup(spawnPos, fontSize, prefixType, color, value, isCrit);
    }

    public void SpawnPopupStringText(Vector3 position, int fontSize, Color color, string strText)
    {
        if (popupTextObject == null || canvas == null) return;
        
        Vector3 spawnPos = position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-0.5f, 0.5f), 0);
        GameObject popupObj = Instantiate(popupTextObject, canvas.transform);
        PopupText popup = popupObj.GetComponent<PopupText>();

        popup.Setup(spawnPos, fontSize, color, strText);
    }


    public void SpawnDeathGhost(Card deadCard, Vector3 direction)
    {
        if (cardGhostVFX == null) return;

        GameObject ghostObj = Instantiate(cardGhostVFX, deadCard.transform.parent);
        ghostObj.transform.position = deadCard.transform.position;

        CardGhost ghostScript = ghostObj.GetComponent<CardGhost>();
        ghostScript.Setup(deadCard.Data.mainIcon, deadCard.Data.cardValueIcon, direction);

        Destroy(deadCard.gameObject);
    }

    public void SpawnParticles(Vector3 spawnPos, bool isPositive)
    {
        if (healVFX == null || bloodVFX == null) return;

        GameObject prefabVFX = isPositive ? healVFX : bloodVFX;

        GameObject healObj = Instantiate(prefabVFX, canvas.transform);
        healObj.transform.position = spawnPos;
    }

}