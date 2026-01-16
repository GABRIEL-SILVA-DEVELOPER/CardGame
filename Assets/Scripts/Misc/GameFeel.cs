using DG.Tweening;
using UnityEngine;

public class GameFeel : MonoBehaviour
{
    public static GameFeel Instance { get; private set; }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { Instance = null; }


    public static void TriggerHitStop(float duration)
    {
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0.05f, 0.01f).SetUpdate(true);
        DOVirtual.DelayedCall(duration, () => Time.timeScale = 1f).SetUpdate(true);
    }

}