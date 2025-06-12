namespace Dawnshard.Network
{
    public class AbilityModel
    {
        public string TriggerId { get; set; }

        public string EffectId { get; set; }
        public string EffectParameter { get; set; }


        public AbilityModel() { }

        public AbilityModel(string triggerId, string effectId, string effectParameter)
        {
            TriggerId = triggerId;
            EffectId = effectId;
            EffectParameter = effectParameter;
        }   
        public AbilityModel(AbilityDTO ability)
        {
            EffectId = ability.EffectID;
            TriggerId = ability.TriggerID;
        }

    }
}