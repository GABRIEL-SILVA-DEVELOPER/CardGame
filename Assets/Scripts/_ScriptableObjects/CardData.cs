using UnityEngine;

[CreateAssetMenu(fileName = "New Card Data", menuName = "DungeonSolitaire/CardData")]
public class CardData : ScriptableObject
{
    public enum CardType { MONSTER, WEAPON, BUFF }
    public enum BuffType { NONE, HEAL, WEAPON_BUFF }

    [Header("Identity")]
    public string cardName;
    [TextArea] public string description;
    public Sprite mainIcon;
    public CardType type;
    public BuffType buffType;

    [Header("Attributes")]
    public int cardValue;
    public Sprite cardValueIcon;

    [Header("Visual")]
    public Color themeColor;
    
}