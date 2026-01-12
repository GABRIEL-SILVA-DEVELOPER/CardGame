using UnityEngine;

public class HeroManager : MonoBehaviour
{
    public static HeroManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init() { Instance = null; }

    public int life = 10;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }


    public void ReciveDamage(Card card)
    {
        life -= card.GetCardValue();

        if (life <= 0)
        {
            life = 0;
            Die();
        }

        Destroy(card.gameObject);
    }

    public void Heal(Card card)
    {
        life += card.GetCardValue();

        if (life > 10)
        {
            life = 10;
        }

        Destroy(card.gameObject);
    }

    private void Die()
    {
        Debug.Log("Game over!");
    }

}