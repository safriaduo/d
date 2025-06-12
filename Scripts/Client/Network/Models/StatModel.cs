namespace Dawnshard.Network
{
    public class StatModel
    {

        public string ID { get; set; }
        public int Value { get; set; }
        public int OriginalValue { get; set; }
        public int MaxValue { get; set; }

        public StatModel(StatDTO stat)
        {
            ID = stat.ID;
            OriginalValue = stat.OriginalValue;
            Value = stat.Value;
            MaxValue = stat.MaxValue;
        }

        public StatModel() { }
    }
}