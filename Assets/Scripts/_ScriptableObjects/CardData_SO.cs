using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Data", menuName = "DungeonSolitaire/CardData")]
public class CardData_SO : ScriptableObject
{
    public enum CardType { MONSTER, WEAPON, BUFF }
    public enum BuffType { NONE, HEAL, WEAPON_BUFF, DODGE_BUFF }

    [Header("Identity")]
    public string cardName;
    [TextArea] public string description;
    public Sprite mainIcon;
    public CardType type;
    public BuffType buffType;

    [Header("Attributes")]
    public int cardValue;
    public Sprite cardValueIcon;

    [Header("Trinkets Config")]
    public List<TrinketRollConfig> possibleTrinkets;

    [System.Serializable]
    public class TrinketRollConfig
    {
        public Trinket_SO trinket;
        [Range(0, 1)] public float spawnChance;
        public float minPower;
        public float maxPower;
    }

}