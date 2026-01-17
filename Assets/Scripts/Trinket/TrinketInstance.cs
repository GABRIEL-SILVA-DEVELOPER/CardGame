[System.Serializable]
public class TrinketInstance
{
    public Trinket_SO source;
    public float finalPower;


    public TrinketInstance(Trinket_SO source, float finalPower)
    {
        this.source = source;
        this.finalPower = finalPower;
    }
}