using Dawnshard.Presenters;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace Dawnshard.Network
{
    public class PlayerModel
    {
        public string ID { get; set; }

        public string Nickname { get; set; }
        public string Deckname { get; set; }

        public bool IsPlayerTurn { get; set; }

        public bool IsLocalPlayer => GameController.Instance.LocalPlayerId == ID;

        public bool IsMulliganCompleted { get; set; }

        public bool IsOpponentMulliganCompleted { get; set; }

        public List<StatModel> Stats { get; set; }

        public List<string> WorldIds { get; set; }

        public bool IsWorldSelected { get; set; }

        public string ActiveWorldId { get; set; }

        //public List<IZonePresenter> Zones { get; set; }
        public List<ZoneModel> Zones { get; set; }
        
        public RankEntryModel Rank { get; set; }
        
        public int RankPosition { get; set; }


        public PlayerModel()
        {

        }
    }
}
