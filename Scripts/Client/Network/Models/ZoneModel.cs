using System.Collections.Generic;

namespace Dawnshard.Network
{
    public class ZoneModel
    {
        public int InstanceId { get; set; }

        public string ZoneId { get; set; }

        public int NumCards { get; set; }

        public int MaxCards { get; set; }
        
        public bool IsStaticZone { get; set; }

        public List<CardModel> Cards { get; set; }

        public ZoneModel() { }
    }
}